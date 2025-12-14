using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using RestoManager.Models;

namespace RestoManager.Services
{
    public class FirestoreListenerService
    {
        private readonly RestaurantService _restaurantService;
        private FirestoreDb _db;
        private FirestoreChangeListener _listener;
        private static int _autoTakeAwayCount = 1;
        private bool _isFirstSnapshot = true;

        public FirestoreListenerService(RestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        public async Task StartListeningAsync(string projectId, string credentialsPath)
        {
            try
            {
                // 1. Verify File Exists
                if (!File.Exists(credentialsPath))
                {
                    MessageBox.Show($"File not found at: {credentialsPath}\n\nPlease ensure 'firestore-project.json' is in the project root and 'Copy to Output Directory' is set to 'Copy if newer'.", 
                        "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 2. Validate JSON Content
                string jsonContent = await File.ReadAllTextAsync(credentialsPath);
                if (string.IsNullOrWhiteSpace(jsonContent) || !jsonContent.Trim().StartsWith("{"))
                {
                    MessageBox.Show("The 'firestore-project.json' file appears to be empty or invalid.", 
                        "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // 3. Authenticate
                // Using GoogleCredential.FromJson is safer as it parses immediately
                GoogleCredential credential = GoogleCredential.FromJson(jsonContent);
                
                FirestoreDbBuilder builder = new FirestoreDbBuilder
                {
                    ProjectId = projectId,
                    DatabaseId = "sisres", // Updated database ID
                    Credential = credential
                };

                _db = await builder.BuildAsync();

                // 4. Test Connection Explicitly
                // This forces a network call. If auth or project ID is wrong, this will fail immediately.
                CollectionReference collection = _db.Collection("orders");
                QuerySnapshot initialSnapshot = await collection.GetSnapshotAsync();
                
                // If we get here, connection is GOOD.
                // MessageBox.Show($"Connected! Found {initialSnapshot.Count} existing documents in 'orders'.", "Connection Test");

                // 5. Start Listener
                _isFirstSnapshot = true;
                _listener = collection.Listen(OnSnapshot);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Connection Failed:\n{ex.Message}", "Firestore Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnSnapshot(QuerySnapshot snapshot)
        {
            if (_isFirstSnapshot)
            {
                _isFirstSnapshot = false;
                return;
            }

            // Debug: Prove this method is called
            // MessageBox.Show($"OnSnapshot called! Changes: {snapshot.Changes.Count}");

            foreach (var change in snapshot.Changes)
            {
                if (change.ChangeType == DocumentChange.Type.Added)
                {
                    Application.Current.Dispatcher.Invoke(() => 
                    {
                        try 
                        {
                            ProcessNewOrder(change.Document);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error processing incoming order: {ex.Message}", "Order Parsing Error");
                        }
                    });
                }
            }
        }

        private List<Order> _cachedAllOrders;
        public event Action<Order> NewOrderReceived;

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            if (_cachedAllOrders != null)
            {
                return _cachedAllOrders;
            }

            if (_db == null)
            {
                return new List<Order>();
            }

            try 
            {
                CollectionReference collection = _db.Collection("orders");
                QuerySnapshot snapshot = await collection.GetSnapshotAsync();
                
                var orders = new List<Order>();
                foreach (var document in snapshot.Documents)
                {
                    try 
                    {
                        var order = ParseOrderFromDocument(document);
                        orders.Add(order);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Error parsing order {document.Id}: {ex.Message}");
                    }
                }

                _cachedAllOrders = orders.OrderByDescending(o => o.OrderTime).ToList();
                return _cachedAllOrders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to fetch orders: {ex.Message}", "Firestore Error");
                return new List<Order>();
            }
        }

        private Order ParseOrderFromDocument(DocumentSnapshot document)
        {
            var order = new Order
            {
                TableId = 0, 
                Status = OrderStatus.Pending,
                FirestoreDocumentId = document.Id
            };

            if (document.ContainsField("createdAt"))
            {
                try
                {
                    Timestamp ts = document.GetValue<Timestamp>("createdAt");
                    order.OrderTime = ts.ToDateTime();
                }
                catch
                {
                    order.OrderTime = DateTime.Now;
                }
            }

            string notes = "App Order";
            if (document.ContainsField("phoneNumber"))
            {
                notes += $" - {document.GetValue<string>("phoneNumber")}";
            }
            order.Notes = notes;

            if (document.ContainsField("items"))
            {
                var rawItems = document.GetValue<object>("items");
                if (rawItems is List<object> itemsList)
                {
                    foreach (var itemObj in itemsList)
                    {
                        if (itemObj is Dictionary<string, object> itemMap)
                        {
                            if (itemMap.TryGetValue("name", out object nameObj) && 
                                itemMap.TryGetValue("quantity", out object qtyObj) &&
                                itemMap.TryGetValue("price", out object priceObj))
                            {
                                string dishName = nameObj?.ToString() ?? "Unknown";
                                int quantity = Convert.ToInt32(qtyObj);
                                decimal price = Convert.ToDecimal(priceObj);

                                AddItemToOrder(order, dishName, quantity, price);
                            }
                        }
                    }
                }
            }
            return order;
        }

        private void ProcessNewOrder(DocumentSnapshot document)
        {
            var table = new Table
            {
                Capacity = 2,
                Location = "Take Away (App)",
                Status = TableStatus.Occupied,
                Name = $"Web_{_autoTakeAwayCount++}"
            };
            _restaurantService.AddTable(table);

            var order = ParseOrderFromDocument(document);
            order.TableId = table.Id;

            if (order.Items.Count > 0)
            {
                _restaurantService.AddOrder(order);
                PlayNotificationSound();

                if (_cachedAllOrders != null)
                {
                    _cachedAllOrders.Insert(0, order);
                }
                NewOrderReceived?.Invoke(order);
            }
        }

        private void PlayNotificationSound()
        {
            try
            {
                string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "notification.mp3");
                
                if (File.Exists(soundPath))
                {
                    var mediaPlayer = new MediaPlayer();
                    mediaPlayer.Open(new Uri(soundPath, UriKind.Absolute));
                    mediaPlayer.Play();
                }
            }
            catch (Exception ex)
            {
                // Silently fail if sound can't be played - don't interrupt order processing
                System.Diagnostics.Debug.WriteLine($"Failed to play notification sound: {ex.Message}");
            }
        }

        private void AddItemToOrder(Order order, string dishName, int quantity, decimal price)
        {
            var dish = _restaurantService.Dishes
                .FirstOrDefault(d => d.Name.Equals(dishName, StringComparison.OrdinalIgnoreCase));

            int dishId;
            if (dish != null)
            {
                dishId = dish.Id;
            }
            else
            {
                // Create temporary dish if it doesn't exist
                var newDish = new Dish 
                { 
                    Name = dishName, 
                    Price = price, 
                    Category = DishCategory.MainCourse,
                    IsAvailable = true 
                };
                _restaurantService.AddDish(newDish);
                dishId = newDish.Id;
            }

            order.Items.Add(new OrderItem
            {
                DishId = dishId,
                DishName = dishName,
                UnitPrice = price,
                Quantity = quantity
            });
        }

        public async Task<bool> DeleteOrdersAsync(List<string> documentIds)
        {
            if (_db == null || documentIds == null || documentIds.Count == 0)
            {
                return false;
            }

            try
            {
                CollectionReference collection = _db.Collection("orders");
                var deleteTasks = documentIds.Select(id => collection.Document(id).DeleteAsync());
                await Task.WhenAll(deleteTasks);

                // Clear cache to force refresh on next load
                _cachedAllOrders = null;

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete orders: {ex.Message}", "Firestore Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}

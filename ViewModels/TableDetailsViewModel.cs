using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using ResManager.Models;
using ResManager.Services;

namespace ResManager.ViewModels
{
    public class TableDetailsViewModel : ObservableObject
    {
        private readonly Table _table;
        private readonly RestaurantService _restaurantService;

        public TableDetailsViewModel(Table table, RestaurantService restaurantService)
        {
            _table = table;
            _restaurantService = restaurantService;
            LoadTableDetails();
        }

        public int TableId => _table.Id;
        public string Location => _table.Location;
        public int Capacity => _table.Capacity;
        public TableStatus Status => _table.Status;

        public ObservableCollection<OrderItem> AllOrderItems { get; } = new();
        public decimal TotalPrice => AllOrderItems.Sum(item => item.Subtotal);

        private void LoadTableDetails()
        {
            AllOrderItems.Clear();
            
            // Get all orders for this table (including paid ones to show complete history)
            var allOrders = _restaurantService.Orders.Where(o => o.TableId == _table.Id).ToList();
            
            foreach (var order in allOrders)
            {
                foreach (var item in order.Items)
                {
                    // Create a copy of the order item to avoid reference issues
                    AllOrderItems.Add(new OrderItem
                    {
                        DishId = item.DishId,
                        DishName = item.DishName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        SpecialInstructions = item.SpecialInstructions
                    });
                }
            }
        }
    }
}

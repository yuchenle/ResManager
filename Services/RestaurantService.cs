using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ResManager.Models;

namespace ResManager.Services
{
    public class RestaurantService
    {
        private ObservableCollection<Table> _tables = new();
        private ObservableCollection<Dish> _dishes = new();
        private ObservableCollection<Order> _orders = new();
        private ObservableCollection<Reservation> _reservations = new();
        private ObservableCollection<Payment> _payments = new();

        private int _nextTableId = 1;
        private int _nextDishId = 1;
        private int _nextOrderId = 1;
        private int _nextReservationId = 1;
        private int _nextPaymentId = 1;

        public RestaurantService()
        {
            InitializeSampleData();
        }

        public ObservableCollection<Table> Tables => _tables;
        public ObservableCollection<Dish> Dishes => _dishes;
        public ObservableCollection<Order> Orders => _orders;
        public ObservableCollection<Reservation> Reservations => _reservations;
        public ObservableCollection<Payment> Payments => _payments;

        public void AddTable(Table table)
        {
            table.Id = _nextTableId++;
            _tables.Add(table);
        }

        public void AddDish(Dish dish)
        {
            dish.Id = _nextDishId++;
            _dishes.Add(dish);
        }

        public void AddOrder(Order order)
        {
            order.Id = _nextOrderId++;
            order.OrderTime = DateTime.Now;
            _orders.Add(order);
            
            // Update table status
            var table = _tables.FirstOrDefault(t => t.Id == order.TableId);
            if (table != null)
            {
                table.Status = TableStatus.Occupied;
            }
        }

        public void AddReservation(Reservation reservation)
        {
            reservation.Id = _nextReservationId++;
            _reservations.Add(reservation);
            
            // Update table status
            var table = _tables.FirstOrDefault(t => t.Id == reservation.TableId);
            if (table != null)
            {
                table.Status = TableStatus.Reserved;
            }
        }

        public void AddPayment(Payment payment)
        {
            payment.Id = _nextPaymentId++;
            payment.PaymentTime = DateTime.Now;
            _payments.Add(payment);
            
            // Update order status
            var order = _orders.FirstOrDefault(o => o.Id == payment.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Paid;
                
                // Update table status
                var table = _tables.FirstOrDefault(t => t.Id == order.TableId);
                if (table != null)
                {
                    table.Status = TableStatus.Available;
                }
            }
        }

        public void UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }

        public void UpdateTableStatus(int tableId, TableStatus status)
        {
            var table = _tables.FirstOrDefault(t => t.Id == tableId);
            if (table != null)
            {
                table.Status = status;
            }
        }

        public List<Order> GetOrdersByTable(int tableId)
        {
            return _orders.Where(o => o.TableId == tableId && o.Status != OrderStatus.Paid).ToList();
        }

        private void InitializeSampleData()
        {
            // Sample tables
            AddTable(new Table { Capacity = 2, Location = "Window", Status = TableStatus.Available });
            AddTable(new Table { Capacity = 4, Location = "Center", Status = TableStatus.Available });
            AddTable(new Table { Capacity = 6, Location = "Private", Status = TableStatus.Available });
            AddTable(new Table { Capacity = 2, Location = "Window", Status = TableStatus.Available });
            AddTable(new Table { Capacity = 4, Location = "Center", Status = TableStatus.Available });

            // Sample dishes
            AddDish(new Dish { Name = "Caesar Salad", Description = "Fresh romaine lettuce with caesar dressing", Price = 12.99m, Category = DishCategory.Appetizer, IsAvailable = true });
            AddDish(new Dish { Name = "Grilled Salmon", Description = "Atlantic salmon with lemon butter sauce", Price = 24.99m, Category = DishCategory.MainCourse, IsAvailable = true });
            AddDish(new Dish { Name = "Chocolate Cake", Description = "Rich chocolate layer cake", Price = 8.99m, Category = DishCategory.Dessert, IsAvailable = true });
            AddDish(new Dish { Name = "Coca Cola", Description = "Carbonated soft drink", Price = 2.99m, Category = DishCategory.Beverage, IsAvailable = true });
            AddDish(new Dish { Name = "French Fries", Description = "Crispy golden fries", Price = 5.99m, Category = DishCategory.Side, IsAvailable = true });
        }
    }
}


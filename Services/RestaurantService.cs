using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestoManager.Models;

namespace RestoManager.Services
{
    public class RestaurantService
    {
        private ObservableCollection<Table> _tables = new();
        private ObservableCollection<Order> _orders = new();
        private ObservableCollection<Reservation> _reservations = new();
        private ObservableCollection<Payment> _payments = new();

        private int _nextTableId = 1;
        private int _nextOrderId = 1;
        private int _nextReservationId = 1;
        private int _nextPaymentId = 1;

        private readonly DataPersistenceService _persistenceService;

        public RestaurantService()
        {
            _persistenceService = new DataPersistenceService();
            // Dishes are no longer loaded from persistence
        }

        public ObservableCollection<Table> Tables => _tables;
        public ObservableCollection<Order> Orders => _orders;
        public ObservableCollection<Reservation> Reservations => _reservations;
        public ObservableCollection<Payment> Payments => _payments;

        public void AddTable(Table table)
        {
            table.Id = _nextTableId++;
            if (string.IsNullOrEmpty(table.Name))
            {
                table.Name = $"Table {table.Id}";
            }
            _tables.Add(table);
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
    }
}


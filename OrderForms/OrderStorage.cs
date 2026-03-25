using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kiosk
{
    public static class OrderStorage
    {
        public static List<Order> Orders { get; } = new List<Order>();

        public static event Action OrdersUpdated;
        public static decimal GetTotalPrice()
        {
            return Orders.Sum(o => o.Price * o.Quantity);
        }

        public static void AddOrder(Order order)
        {
            Orders.Add(order);
            OrdersUpdated?.Invoke(); 
        }
        public static List<Order> GetOrders()
        {
            return Orders;
        }

        public static decimal GetTotal()
        {
            return Orders.Sum(o => o.Price * o.Quantity);
        }

        public static void ClearOrders()
        {
            Orders.Clear();
        }
    }
}

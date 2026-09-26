using System;
using System.Collections.Generic;
using System.Linq;
using kiosk.UI;

namespace kiosk
{
    public static class OrderStorage
    {
        public static List<Order> Orders { get; } = new List<Order>();
        public static event Action OrdersUpdated;
        public static decimal GetTotalPrice() { return GetTotal(); }
        public static decimal GetTotal() { return Orders.Sum(o => o.Total); }
        public static List<Order> GetOrders() { return Orders; }
        public static void Notify() { OrdersUpdated?.Invoke(); }
        public static void Validate(Order order)
        {
            var product = MenuCatalog.Find(order.Product);
            if (product == null || LocalStore.SoldOut.Contains(order.Product)) throw new InvalidOperationException(GuestText.T(order.Product + " is unavailable. Please remove it from your order."));
            if (order.Quantity < 1 || order.Quantity > 99) throw new InvalidOperationException(GuestText.T("Choose between 1 and 99 of each item."));
            if (product.HasSizes && !product.Sizes.Any(s => s.Code == order.Size)) throw new InvalidOperationException(GuestText.T("Choose a valid size."));
            if (product.HasTemperature && order.Temperature != "Hot" && order.Temperature != "Iced") throw new InvalidOperationException(GuestText.T("Choose hot or iced."));
            if (order.Price != product.PriceFor(order.Size) || order.Price <= 0) throw new InvalidOperationException(GuestText.T("The item price has changed. Please add it again."));
        }
        public static void ValidateCart()
        {
            if (Orders.Count == 0) throw new InvalidOperationException(GuestText.T("Add an item before checking out."));
            foreach (var order in Orders) Validate(order);
        }
        // set by AddOrder, read and cleared by the menu when it comes back into view
        public static string LastAdded;

        public static void AddOrder(Order order)
        {
            Validate(order);
            var same = Orders.FirstOrDefault(o => o.Product == order.Product && o.Size == order.Size &&
                o.Temperature == order.Temperature && o.Price == order.Price &&
                (o.Sweetness ?? "100%") == (order.Sweetness ?? "100%"));
            if (same != null)
            {
                if (same.Quantity + order.Quantity > 99) throw new InvalidOperationException(GuestText.T("You can order up to 99 of this combination."));
                same.Quantity += order.Quantity;
            }
            else Orders.Add(order);
            LastAdded = (order.Quantity > 1 ? order.Quantity + " x " : "") + order.Product;
            Notify();
        }
        public static void ClearOrders() { Orders.Clear(); LastAdded = null; Notify(); }
    }
}

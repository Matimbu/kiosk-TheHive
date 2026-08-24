using System;
using System.Drawing;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    public partial class menuPage : Form, IPage
    {
        private readonly CartPill _cartPill = new CartPill();

        public menuPage()
        {
            InitializeComponent();

            header.Logo = MenuCatalog.Logo;
            header.SetAccessory(_cartPill);
            _cartPill.Click += (s, e) => OpenOrder();

            foreach (MenuCategory category in MenuCatalog.Categories)
                rail.Add(category.Name, category.Name);

            rail.ChipSelected += (s, e) => ShowCategory((string)e.Tag);
            grid.ProductChosen += (s, e) => OpenProduct(e.Item);
            cartBar.Action.Click += (s, e) => OpenOrder();

            OrderStorage.OrdersUpdated += RefreshOrderTotals;
            Disposed += (s, e) => OrderStorage.OrdersUpdated -= RefreshOrderTotals;

            Load += (s, e) =>
            {
                rail.Select(0);
                RefreshOrderTotals();
            };
        }

        private void ShowCategory(string category)
        {
            MenuCategory meta = MenuCatalog.CategoryOf(category);
            header.Subtitle = meta != null ? meta.Tagline : null;
            grid.Load(category);
        }

        private void OpenProduct(MenuProduct product)
        {
            Nav.Go(new ProductSheet(product));
        }

        private void OpenOrder()
        {
            if (OrderStorage.Orders.Count == 0) return;
            Nav.Go(new viewOrder());
        }

        public void OnRevealed()
        {
            RefreshOrderTotals();
        }

        private void RefreshOrderTotals()
        {
            int count = 0;
            foreach (Order order in OrderStorage.Orders) count += order.Quantity;

            cartBar.Update(count, OrderStorage.GetTotal());
            _cartPill.Count = count;
        }
    }
}

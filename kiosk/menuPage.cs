using System;
using System.Drawing;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    public partial class menuPage : Form, IPage
    {
        private readonly Toast _added = new Toast();

        public menuPage()
        {
            InitializeComponent();

            header.Logo = MenuCatalog.Logo;

            foreach (MenuCategory category in MenuCatalog.Categories)
                rail.Add(category.Name, category.Name);

            rail.ChipSelected += (s, e) => ShowCategory((string)e.Tag);
            grid.ProductChosen += (s, e) => OpenProduct(e.Item);
            grid.ProductQuickAdd += (s, e) => QuickAdd(e.Item);
            cartBar.Action.Click += (s, e) => OpenOrder();

            Controls.Add(_added);
            _added.Anchor = AnchorStyles.Bottom;

            OrderStorage.OrdersUpdated += RefreshOrderTotals;
            Disposed += (s, e) => OrderStorage.OrdersUpdated -= RefreshOrderTotals;

            Load += (s, e) =>
            {
                rail.Select(0);
                RefreshOrderTotals();
                PlaceToast();
            };
        }

        private void ShowCategory(string category)
        {
            MenuCategory meta = MenuCatalog.CategoryOf(category);
            header.Subtitle = meta != null ? meta.Tagline : null;
            grid.Load(category);
        }

        // Only reached for items with nothing to choose, so this is exactly the
        // order the product page would have built with its defaults.
        private void QuickAdd(MenuProduct product)
        {
            try
            {
                OrderStorage.AddOrder(new Order
                {
                    Product = product.Name,
                    Size = "",
                    Temperature = "",
                    Quantity = 1,
                    Price = product.PriceFor(""),
                    ImagePath = product.ImageKey
                });
            }
            catch (InvalidOperationException ex)
            {
                NoticeForm.Say("Cannot add that item", ex.Message);
                return;
            }
            ConfirmLastAdd();   // the page is already showing, so say it now
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
            PlaceToast();
            ConfirmLastAdd();
        }

        private void RefreshOrderTotals()
        {
            int count = 0;
            foreach (Order order in OrderStorage.Orders) count += order.Quantity;

            cartBar.Update(count, OrderStorage.GetTotal());
        }

        // floats just above the cart bar, clear of the tray
        private void PlaceToast()
        {
            _added.Left = (ClientSize.Width - _added.Width) / 2;
            _added.Top  = cartBar.Top - _added.Height;
            _added.BringToFront();
        }

        private void ConfirmLastAdd()
        {
            string item = OrderStorage.LastAdded;
            if (string.IsNullOrEmpty(item)) return;
            OrderStorage.LastAdded = null;
            _added.Say(string.Format(GuestText.T("{0} added"), item));
            PlaceToast();
        }
    }
}

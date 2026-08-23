using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using kiosk.Payments;
using kiosk.UI;

namespace kiosk
{
    /// <summary>
    /// Order review. Every line is tappable for edits, the total is always in
    /// view, and checkout is one button away.
    /// </summary>
    public partial class viewOrder : Form, IPage
    {
        public viewOrder()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            header.ShowBack((s, e) => Nav.Back());
            footer.Paint += PaintSummary;

            btn_menu.Click += (s, e) => Nav.Back();
            btn_paymentMethod.Click += Checkout;

            OrderStorage.OrdersUpdated += Rebuild;
            Load += (s, e) => Rebuild();

            // Hosted pages are disposed rather than closed, so unhook here.
            Disposed += (s, e) => OrderStorage.OrdersUpdated -= Rebuild;
        }

        private void Rebuild()
        {
            foreach (Control c in list.Content.Controls.Cast<Control>().ToList())
            {
                list.Content.Controls.Remove(c);
                c.Dispose();
            }

            List<Order> orders = OrderStorage.GetOrders();

            if (orders.Count == 0)
            {
                EmptyState empty = new EmptyState("Nothing here yet",
                    "Add something from the menu and it will show up right here.");
                empty.Bounds = new Rectangle(0, 0, list.Width, Math.Max(240, list.Height - 20));
                list.Content.Controls.Add(empty);
            }
            else
            {
                int top = 8;
                foreach (Order order in orders)
                {
                    OrderRow row = new OrderRow(order);
                    row.Bounds = new Rectangle(Hive.Gutter - 4, top, list.Width - (Hive.Gutter - 4) * 2 - 6, 88);
                    row.Edit += (s, e) => EditLine(e.Order);
                    list.Hook(row);
                    list.Content.Controls.Add(row);
                    top += 92;
                }
            }

            list.Measure(Hive.Gap);
            btn_paymentMethod.Enabled = orders.Count > 0;
            footer.Invalidate();
        }

        private void EditLine(Order order)
        {
            int index = OrderStorage.Orders.IndexOf(order);
            if (index < 0) return;

            EditOrderForm editor = new EditOrderForm(order);
            editor.Committed += (s, e) =>
            {
                if (editor.IsRemoved) OrderStorage.Orders.RemoveAt(index);
                else if (editor.UpdatedOrder != null) OrderStorage.Orders[index] = editor.UpdatedOrder;
            };

            Nav.Go(editor);
        }

        private void Checkout(object sender, EventArgs e)
        {
            if (OrderStorage.Orders.Count == 0) return;

            Nav.Go(new PaymentSelectionForm(OrderStorage.GetOrders(), OrderStorage.GetTotalPrice()));
        }

        /// <summary>Refreshes when the guest comes back from editing a line.</summary>
        public void OnRevealed()
        {
            Rebuild();
        }

        private void PaintSummary(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, 0, 0, footer.Width, 0);

            int count = OrderStorage.Orders.Sum(o => o.Quantity);
            decimal total = OrderStorage.GetTotal();

            Hive.Text(g, "Total to pay", Hive.Subhead,
                      new Rectangle(Hive.Gutter, 22, 220, 26), Hive.Ink, Hive.LeftMid);
            Hive.Text(g, count == 1 ? "1 item" : count + " items", Hive.Caption,
                      new Rectangle(Hive.Gutter, 46, 220, 20), Hive.Muted, Hive.LeftMid);

            using (Font big = Hive.Sized(Hive.PriceBig, 23f))
                Hive.Text(g, Hive.Money(total), big,
                          new Rectangle(footer.Width - 240 - Hive.Gutter, 24, 240, 40), Hive.Teal, Hive.RightMid);
        }

    }
}

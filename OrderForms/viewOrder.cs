using kiosk.Payments;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kiosk
{
    public partial class viewOrder : Form
    {
        public viewOrder()
        {
            InitializeComponent();

            SetupListView();

            listBoxOrders.ColumnWidthChanging += listBoxOrders_ColumnWidthChanging;
            listBoxOrders.Click += listBoxOrders_Click;

            OrderStorage.OrdersUpdated += UpdateOrderList;
            UpdateOrderList();

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Your Order — The Hive Cafe";

            // Specific button roles
            ThemeManager.StyleButton(btn_paymentMethod, ButtonRole.Success);
            ThemeManager.StyleButton(button_menu,       ButtonRole.Ghost);

            // Give the list view a subtle alternating row feel via owner draw
            listBoxOrders.OwnerDraw = true;
            listBoxOrders.DrawItem       += ListView_DrawItem;
            listBoxOrders.DrawSubItem    += ListView_DrawSubItem;
            listBoxOrders.DrawColumnHeader += ListView_DrawColumnHeader;
        }

        // ── ListView Setup ─────────────────────────────────────────────────
        private void SetupListView()
        {
            if (listBoxOrders.Columns.Count > 0) return;

            listBoxOrders.View        = View.Details;
            listBoxOrders.FullRowSelect = true;
            listBoxOrders.MultiSelect   = false;
            listBoxOrders.LabelEdit     = false;
            listBoxOrders.HeaderStyle   = ColumnHeaderStyle.Nonclickable;
            listBoxOrders.GridLines     = false;

            listBoxOrders.Columns.Add("Item",        150, HorizontalAlignment.Left);
            listBoxOrders.Columns.Add("Size & Temp", 110, HorizontalAlignment.Left);
            listBoxOrders.Columns.Add("Qty",          45, HorizontalAlignment.Center);
            listBoxOrders.Columns.Add("Total",         90, HorizontalAlignment.Right);
        }

        // ── Owner-Draw Handlers ────────────────────────────────────────────
        private void ListView_DrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            using (var brush = new SolidBrush(ThemeManager.ColorEspresso))
                e.Graphics.FillRectangle(brush, e.Bounds);

            using (var pen = new Pen(ThemeManager.ColorBrown))
                e.Graphics.DrawLine(pen,
                    e.Bounds.Left, e.Bounds.Bottom - 1,
                    e.Bounds.Right, e.Bounds.Bottom - 1);

            TextRenderer.DrawText(e.Graphics, e.Header.Text, ThemeManager.FontBold,
                new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height),
                ThemeManager.ColorTextLight,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter);
        }

        private void ListView_DrawItem(object sender, DrawListViewItemEventArgs e)
        {
            Color bg = e.Item.Selected
                ? ThemeManager.ColorAmber
                : (e.ItemIndex % 2 == 0
                    ? ThemeManager.ColorSurface
                    : Color.FromArgb(250, 240, 225));

            using (var brush = new SolidBrush(bg))
                e.Graphics.FillRectangle(brush, e.Bounds);
        }

        private void ListView_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            Color fg = e.Item.Selected ? Color.White : ThemeManager.ColorTextDark;

            var flags = e.ColumnIndex == 3
                ? TextFormatFlags.Right | TextFormatFlags.VerticalCenter
                : TextFormatFlags.Left  | TextFormatFlags.VerticalCenter;

            var bounds = new Rectangle(
                e.Bounds.X + 4, e.Bounds.Y, e.Bounds.Width - 8, e.Bounds.Height);

            TextRenderer.DrawText(e.Graphics, e.SubItem.Text,
                ThemeManager.FontBase, bounds, fg, flags);
        }

        // ── Data Updates ───────────────────────────────────────────────────
        private void UpdateOrderList()
        {
            if (InvokeRequired) { Invoke((Action)UpdateOrderList); return; }

            listBoxOrders.Items.Clear();

            foreach (var order in OrderStorage.Orders)
            {
                decimal total = order.Price * order.Quantity;
                var item = new ListViewItem(order.Product);
                item.SubItems.Add(string.IsNullOrWhiteSpace(order.SizeTemp.Trim(' ', '-'))
                    ? "—" : order.SizeTemp);
                item.SubItems.Add(order.Quantity.ToString());
                item.SubItems.Add($"₱{total:N2}");
                listBoxOrders.Items.Add(item);
            }

            decimal grandTotal = OrderStorage.GetTotal();
            labelTotal.Text = $"Total: ₱{grandTotal:N2}";

            // Disable pay button if cart is empty
            btn_paymentMethod.Enabled = OrderStorage.Orders.Count > 0;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            OrderStorage.OrdersUpdated -= UpdateOrderList;
            base.OnFormClosed(e);
        }

        // ── Button Handlers ────────────────────────────────────────────────
        private void button_menu_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBoxOrders_Click(object sender, EventArgs e)
        {
            if (listBoxOrders.SelectedItems.Count == 0) return;

            int index = listBoxOrders.SelectedItems[0].Index;
            if (index < 0 || index >= OrderStorage.Orders.Count) return;

            Order selectedOrder = OrderStorage.Orders[index];

            using (var editForm = new EditOrderForm(selectedOrder))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    if (editForm.IsRemoved)
                    {
                        OrderStorage.Orders.RemoveAt(index);
                    }
                    else if (editForm.UpdatedOrder != null)
                    {
                        OrderStorage.Orders[index] = editForm.UpdatedOrder;
                    }

                    UpdateOrderList();
                }
            }
        }

        private void listBoxOrders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel   = true;
            e.NewWidth = listBoxOrders.Columns[e.ColumnIndex].Width;
        }

        private void btn_paymentMethod_Click(object sender, EventArgs e)
        {
            if (OrderStorage.Orders.Count == 0)
            {
                MessageBox.Show("Your cart is empty. Add items before checking out.",
                    "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var paymentForm = new PaymentSelectionForm(
                OrderStorage.GetOrders(),
                OrderStorage.GetTotalPrice());

            paymentForm.Show();
        }
    }
}

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kiosk.UI
{
    public class StaffPage : Form
    {
        private readonly ListBox _history = new ListBox();
        private readonly TextBox _detail = new TextBox();
        public StaffPage()
        {
            BackColor = Hive.Canvas;
            var tabs = new TabControl { Dock = DockStyle.Fill, Font = Hive.Body };
            var history = new TabPage("Order history");
            var availability = new TabPage("Availability");
            tabs.TabPages.Add(history); tabs.TabPages.Add(availability);
            var back = new Button { Text = "Back to welcome", Dock = DockStyle.Bottom, Height = 48 };
            back.Click += (s, e) => Nav.Back();
            Controls.Add(tabs); Controls.Add(back);
            _history.Dock = DockStyle.Top; _history.Height = 230;
            _detail.Dock = DockStyle.Fill; _detail.Multiline = true; _detail.ReadOnly = true; _detail.ScrollBars = ScrollBars.Vertical;
            history.Controls.Add(_detail); history.Controls.Add(_history);
            var status = new Label { Dock = DockStyle.Bottom, Height = 52 };
            history.Controls.Add(status);
            var confirmCash = new Button { Text = "Confirm selected cash payment", Dock = DockStyle.Bottom, Height = 48 };
            history.Controls.Add(confirmCash);
            confirmCash.Click += (s, e) => {
                var order = _history.SelectedItem as SavedOrder;
                if (order == null || order.Method != "Cash" || order.Status != "Pending counter payment") return;
                if (MessageBox.Show("Confirm you received " + Hive.Money(order.Total) + " for order " + order.DisplayNumber + "?", "Confirm cash received", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
                try {
                    order.Status = "Paid at counter";
                    LocalStore.Write(System.IO.Path.Combine(LocalStore.Root, "Orders", order.Id + ".xml"), order);
                    int index = _history.SelectedIndex; _history.Items[index] = order;
                    _detail.Text = "Order " + order.DisplayNumber + "\r\nRecord " + order.Id + "\r\n" + order.Status + "\r\n\r\n" + string.Join("\r\n", order.Lines.Select(o => o.ToString()));
                }
                catch (Exception ex) { order.Status = "Pending counter payment"; MessageBox.Show("Could not save payment status: " + ex.Message); }
            };
            try
            {
                int unreadable;
                foreach (var order in LocalStore.History(out unreadable)) _history.Items.Add(order);
                status.Text = _history.Items.Count + " saved orders. " + unreadable + " unreadable records.\nStored in " + LocalStore.Root;
            }
            catch (Exception ex) { status.Text = "Unable to load history: " + ex.Message; }
            _history.SelectedIndexChanged += (s, e) => {
                var order = _history.SelectedItem as SavedOrder;
                if (order != null) _detail.Text = "Order " + order.DisplayNumber + "\r\nRecord " + order.Id + "\r\n" + order.Status + "\r\n\r\n" + string.Join("\r\n", order.Lines.Select(o => o.ToString()));
            };
            var hint = new Label { Text = "Checked items are SOLD OUT. Changes save immediately.", Dock = DockStyle.Top, Height = 48 };
            var items = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, Font = Hive.Body, IntegralHeight = false };
            foreach (var product in MenuCatalog.All.OrderBy(p => p.Name)) items.Items.Add(product.Name, LocalStore.SoldOut.Contains(product.Name));
            items.ItemCheck += (s, e) => {
                try { LocalStore.SetSoldOut((string)items.Items[e.Index], e.NewValue == CheckState.Checked); }
                catch (Exception ex) { e.NewValue = e.CurrentValue; MessageBox.Show("Could not save availability: " + ex.Message); }
            };
            availability.Controls.Add(items); availability.Controls.Add(hint);
        }
    }
}

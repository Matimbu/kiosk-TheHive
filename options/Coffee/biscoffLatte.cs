using System;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.options
{
    public partial class biscoffLatte : Form
    {
        private const string ProductName = "Biscoff Latte";

        public biscoffLatte()
        {
            InitializeComponent();

            radioSize16L.CheckedChanged       += (s, e) => UpdateTotalPrice();
            radioSize22L.CheckedChanged       += (s, e) => UpdateTotalPrice();
            numericUpDownQuantity.ValueChanged += (s, e) => UpdateTotalPrice();
            radioHot.CheckedChanged           += (s, e) => UpdateTotalPrice();
            radioCold.CheckedChanged          += (s, e) => UpdateTotalPrice();

            numericUpDownQuantity.Minimum = 1;
            numericUpDownQuantity.Maximum = 99;
            radioSize16L.Checked = true;
            radioHot.Checked     = true;

            UpdateTotalPrice();

            ThemeManager.ApplyTheme(this);
            this.Text = ProductName;
            ThemeManager.StyleButton(buttonPlaceOrder, ButtonRole.Accent);
            ThemeManager.StyleButton(buttonBack,       ButtonRole.Ghost);
        }

        private void UpdateTotalPrice()
        {
            string  size  = radioSize16L.Checked ? "16L" : "22L";
            int     qty   = (int)numericUpDownQuantity.Value;
            decimal price = PriceTable.GetPrice(ProductName, size);
            labelTotalPrice.Text = $"Total: ₱{price * qty:N2}";
        }

        private void buttonPlaceOrder_Click(object sender, EventArgs e)
        {
            string size = radioSize16L.Checked ? "16L" :
                          radioSize22L.Checked ? "22L" : "";
            string temp = radioHot.Checked  ? "Hot"  :
                          radioCold.Checked ? "Cold" : "";
            int    qty  = (int)numericUpDownQuantity.Value;

            if (string.IsNullOrEmpty(size) || string.IsNullOrEmpty(temp) || qty < 1)
            {
                MessageBox.Show("Please select a size, temperature, and quantity.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            decimal price = PriceTable.GetPrice(ProductName, size);
            OrderStorage.AddOrder(new Order
            {
                Product     = ProductName,
                Size        = size,
                Temperature = temp,
                Quantity    = qty,
                Price       = price,
            });

            MessageBox.Show($"{qty}× {ProductName} ({size}, {temp}) added!",
                "Added to Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void buttonBack_Click(object sender, EventArgs e) => this.Close();
    }
}

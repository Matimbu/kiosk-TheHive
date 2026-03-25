using System;
using System.Windows.Forms;

namespace kiosk.options.Rice
{
    public partial class waffle : Form
    {
        private const string  ProductName = "Waffle";
        private const decimal UnitPrice   = 80m;

        public waffle()
        {
            InitializeComponent();

            numericUpDownQuantity.Minimum = 1;
            numericUpDownQuantity.Maximum = 99;
            numericUpDownQuantity.Value   = 1;
            numericUpDownQuantity.ValueChanged += (s, e) => UpdateTotalPrice();

            UpdateTotalPrice();

            ThemeManager.ApplyTheme(this);
            ThemeManager.StyleButton(buttonPlaceOrder, ButtonRole.Accent);
            ThemeManager.StyleButton(buttonBack,       ButtonRole.Ghost);
        }

        private void UpdateTotalPrice()
        {
            int qty = (int)numericUpDownQuantity.Value;
            labelTotalPrice.Text = $"Total: ₱{UnitPrice * qty:N2}";
        }

        private void buttonPlaceOrder_Click(object sender, EventArgs e)
        {
            int qty = (int)numericUpDownQuantity.Value;
            if (qty < 1)
            {
                MessageBox.Show("Please select a quantity.",
                    "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderStorage.AddOrder(new Order
            {
                Product     = ProductName,
                Size        = "",
                Temperature = "",
                Quantity    = qty,
                Price       = UnitPrice,
            });

            MessageBox.Show($"{qty}× {ProductName} added to your order!",
                "Added", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void buttonBack_Click(object sender, EventArgs e) => this.Close();

        // Stub for inherited button1 in Designer
        private void button1_Click(object sender, EventArgs e) { }
    }
}

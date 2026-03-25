using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk.options.Rice
{
    public partial class beefTapa: Form
    {
        public int quantity { get; private set; }
        public string product { get; set; }
        public beefTapa()
        {
            InitializeComponent();
            numericUpDownQuantity.ValueChanged += (s, e) => UpdateTotalPrice();

            UpdateTotalPrice();
        }
        private void UpdateTotalPrice()
        {
            decimal pricePerItem = 250m;

            int quantity = (int)numericUpDownQuantity.Value;
            decimal total = pricePerItem * quantity;
            labelTotalPrice.Text = $"Total: ₱{total}";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            decimal pricePerItem = 100m;
            OrderStorage.AddOrder(new Order
            {
                Product = product,
                Price = pricePerItem,
            });
            this.Close();
        }

        private void buttonPlaceOrder_Click(object sender, EventArgs e)
        {
            string product = "Beef Tapa";
            int quantity = (int)numericUpDownQuantity.Value;

            if (quantity < 1)
            {
                MessageBox.Show("Please select quantity.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal pricePerItem = 250m;

            OrderStorage.AddOrder(new Order
            {
                Product = product,
                Quantity = quantity,
                Price = pricePerItem,
            });
            this.Close();
        }
    }
}

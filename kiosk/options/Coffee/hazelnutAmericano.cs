using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk.options
{
    public partial class hazelnutAmericano: Form
    {
        public string selectedSize { get; private set; }
        public string temperature { get; private set; }
        public int quantity { get; private set; }
        public string product { get; set; } // Pass this in
        decimal pricePerItem = 0m;
        

        public hazelnutAmericano()
        {
            InitializeComponent();
        }


        private void buttonPlaceOrder_Click(object sender, EventArgs e)
        {
            string product = "Hazelnut Americano";
            string selectedSize = "";

            // Get selected size
            if (radioSize16L.Checked)
            {
                selectedSize = "16L";
                pricePerItem = 100m; // Price for 16L
            }
            else if (radioSize22L.Checked)
            {
                selectedSize = "22L";
                pricePerItem = 135m; // Price for 22L
            }

            // Get quantity
            int quantity = (int)numericUpDownQuantity.Value;

            // Get temperature
            string temperature = "";
            if (radioHot.Checked)
                temperature = "Hot";
            else if (radioCold.Checked)
                temperature = "Cold";

            string groupBox_Size = radioSize16L.Checked ? "16L" :
                      radioSize22L.Checked ? "22L" : "";

            string groupBox_Temp = radioHot.Checked ? "Hot" :
                                 radioCold.Checked ? "Cold" : "";

            // Validation
            if (string.IsNullOrEmpty(selectedSize) || string.IsNullOrEmpty(temperature) || quantity < 1)
            {
                MessageBox.Show("Please select a size, temperature, and quantity.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OrderStorage.AddOrder(new Order
            {
                Product = product,
                Size = selectedSize,
                Temperature = temperature,
                Quantity = quantity,
                Price = pricePerItem
            });


            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {

            Hide();
        }

        
    }
}

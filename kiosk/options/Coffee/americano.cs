using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk
{
    public partial class americano : Form
    {
        public string selectedSize { get; private set; }
        public string temperature { get; private set; }
        public int quantity { get; private set; }
        public string product { get; set; } 
        


        public americano()
        {
            InitializeComponent();

            radioSize16L.CheckedChanged += (s, e) => UpdateTotalPrice();
            radioSize22L.CheckedChanged += (s, e) => UpdateTotalPrice();
            numericUpDownQuantity.ValueChanged += (s, e) => UpdateTotalPrice();

            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            string selectedSize = radioSize16L.Checked ? "16L" :
                                  radioSize22L.Checked ? "22L" : "";

            int quantity = (int)numericUpDownQuantity.Value;

            decimal pricePerItem = 0;
            if (selectedSize == "16L")
                pricePerItem = 85m;
            else if (selectedSize == "22L")
                pricePerItem = 115m;

            decimal total = pricePerItem * quantity;
            labelTotalPrice.Text = $"Total: ₱{total}";
        }

        private void buttonPlaceOrder_Click(object sender, EventArgs e)
        {
            string product = "Americano";
            string selectedSize = ""; 

            if (radioSize16L.Checked) selectedSize = "16L";
            else if (radioSize22L.Checked) selectedSize = "22L";

            int quantity = (int)numericUpDownQuantity.Value;

            string temperature = "";
            if (radioHot.Checked) temperature = "Hot";
            else if (radioCold.Checked) temperature = "Cold";

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

            decimal pricePerItem = selectedSize == "16L" ? 85m : 115m;

            OrderStorage.AddOrder(new Order
            {
                Product = product,
                Size = selectedSize,
                Temperature = temperature,
                Quantity = quantity,
                Price = pricePerItem,
            });        
            this.Close();
        }

        private void buttonBack_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

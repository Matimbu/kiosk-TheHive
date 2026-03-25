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
    public partial class EditOrderForm : Form
    {
        private Order originalOrder;

        public Order UpdatedOrder { get; private set; }
        public bool IsRemoved { get; private set; } = false;

        private decimal unitPrice;
        public EditOrderForm(Order orderToEdit)
        {
            InitializeComponent();

            originalOrder = orderToEdit;
            unitPrice = orderToEdit.Price;

            txtProduct.Text = orderToEdit.Product;
            numericUpDown_Quantity.Value = orderToEdit.Quantity;

            if (orderToEdit.Size == "16L")
                radio_L16.Checked = true;
            else if (orderToEdit.Size == "22L")
                radio_L22.Checked = true;

            if (orderToEdit.Temperature == "Hot")
                radio_Hot.Checked = true;
            else if (orderToEdit.Temperature == "Iced")
                radio_Iced.Checked = true;

            unitPrice = PriceTable.GetPrice(orderToEdit.Product, orderToEdit.Size);

            radio_L16.CheckedChanged += (s, e) => OnSizeChanged();
            radio_L22.CheckedChanged += (s, e) => OnSizeChanged();

            numericUpDown_Quantity.ValueChanged += (s, e) => UpdatePriceLabel();

            UpdatePriceLabel();

        }
        private void OnSizeChanged()
        {
            string size = radio_L16.Checked ? "16L" : "22L";
            unitPrice = PriceTable.GetPrice(txtProduct.Text, size);
            UpdatePriceLabel();
        }
        private void UpdatePriceLabel()
        {
            int quantity = (int)numericUpDown_Quantity.Value;
            decimal total = unitPrice * quantity;
            label_TotalPrice.Text = $"Total: ₱{total}";
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            UpdatedOrder = new Order
            {
                Product = txtProduct.Text,
                Quantity = (int)numericUpDown_Quantity.Value,
                Size = radio_L16.Checked ? "16L" : "22L",
                Temperature = radio_Hot.Checked ? "Hot" : "Iced",
                Price = unitPrice
            };

            this.DialogResult = DialogResult.OK;
            this.Close();
            
        }
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show("Are you sure you want to remove this order?", "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                IsRemoved = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void numericUpDown_Quantity_ValueChanged(object sender, EventArgs e)
        {
            UpdatePriceLabel();
        }

        private void radio_L16_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_L16.Checked)
            {
                unitPrice = 85m;    //Price form this Size
                UpdatePriceLabel();
            }
        }

        private void radio_L22_CheckedChanged(object sender, EventArgs e)
        {
            if (radio_L22.Checked)
            {
                unitPrice = 115m;   //Price form this Size
                UpdatePriceLabel();
            }
        }

        private void radioh_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}

using System;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk
{
    public partial class EditOrderForm : Form
    {
        private Order   _originalOrder;
        private decimal _unitPrice;

        public Order UpdatedOrder { get; private set; }
        public bool   IsRemoved   { get; private set; } = false;

        public EditOrderForm(Order orderToEdit)
        {
            InitializeComponent();

            _originalOrder = orderToEdit;
            _unitPrice     = orderToEdit.Price;

            // Populate controls from order
            txtProduct.Text = orderToEdit.Product;
            txtProduct.ReadOnly = true;   // Product name should not be editable here

            numericUpDown_Quantity.Minimum = 1;
            numericUpDown_Quantity.Maximum = 99;
            numericUpDown_Quantity.Value   = Math.Max(1, orderToEdit.Quantity);

            // Size radios — only shown for items that have sizes
            bool hasSize = PriceTable.HasSize(orderToEdit.Product);
            radio_L16.Visible  = hasSize;
            radio_L22.Visible  = hasSize;
            lblSize.Visible    = hasSize;

            if (hasSize)
            {
                if (orderToEdit.Size == "16L") radio_L16.Checked = true;
                else                            radio_L22.Checked = true;
            }

            // Temperature radios — only shown for coffee items
            bool hasTemp = hasSize;
            radio_Hot.Visible  = hasTemp;
            radio_Iced.Visible = hasTemp;
            lblTemp.Visible    = hasTemp;

            if (hasTemp)
            {
                if (orderToEdit.Temperature == "Hot")  radio_Hot.Checked  = true;
                else if (orderToEdit.Temperature == "Iced") radio_Iced.Checked = true;
                else if (orderToEdit.Temperature == "Cold") radio_Iced.Checked = true;
            }

            // Price
            _unitPrice = PriceTable.GetPrice(orderToEdit.Product, orderToEdit.Size);
            if (_unitPrice == 0) _unitPrice = orderToEdit.Price; // fall back to stored price

            // Wire events
            radio_L16.CheckedChanged          += (s, e) => OnSizeChanged();
            radio_L22.CheckedChanged          += (s, e) => OnSizeChanged();
            numericUpDown_Quantity.ValueChanged += (s, e) => UpdatePriceLabel();

            UpdatePriceLabel();

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Edit Order Item";

            ThemeManager.StyleButton(btnSave,   ButtonRole.Success);
            ThemeManager.StyleButton(btnCancel, ButtonRole.Ghost);
            ThemeManager.StyleButton(btnRemove, ButtonRole.Danger);
        }

        private void OnSizeChanged()
        {
            string size = radio_L16.Checked ? "16L" : "22L";
            _unitPrice  = PriceTable.GetPrice(txtProduct.Text, size);
            if (_unitPrice == 0) _unitPrice = _originalOrder.Price;
            UpdatePriceLabel();
        }

        private void UpdatePriceLabel()
        {
            int     qty   = (int)numericUpDown_Quantity.Value;
            decimal total = _unitPrice * qty;
            label_TotalPrice.Text = $"Total: ₱{total:N2}";
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string size = radio_L16.Visible
                ? (radio_L16.Checked ? "16L" : "22L")
                : _originalOrder.Size;

            string temp = radio_Hot.Visible
                ? (radio_Hot.Checked ? "Hot" : "Iced")
                : _originalOrder.Temperature;

            UpdatedOrder = new Order
            {
                Product     = txtProduct.Text,
                Quantity    = (int)numericUpDown_Quantity.Value,
                Size        = size,
                Temperature = temp,
                Price       = _unitPrice,
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
            var confirm = MessageBox.Show(
                $"Remove \"{_originalOrder.Product}\" from your order?",
                "Confirm Remove",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm == DialogResult.Yes)
            {
                IsRemoved         = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}

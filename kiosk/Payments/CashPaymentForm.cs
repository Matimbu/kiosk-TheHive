using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class CashPaymentForm: Form
    {

        public CashPaymentForm()
        {
            InitializeComponent();
            
            TextBox hiddenDummy = new TextBox();
            hiddenDummy.Visible = false;
            hiddenDummy.TabStop = false;
            this.Controls.Add(hiddenDummy);

            this.Load += (s, e) =>
            {
                this.ActiveControl = hiddenDummy;
            };

            labelTotal.Text = $"Total: ₱{OrderStorage.GetTotalPrice()}";
        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Please collect cash from the customer.", "Cash Payment", MessageBoxButtons.OK, MessageBoxIcon.Information);

            // 🔥 Show receipt
            var ordersCopy = OrderStorage.GetOrders().ToList(); // clone before clearing
            OrderStorage.ClearOrders();

            var receiptForm = new ReceiptForm(ordersCopy, "Cash","");
            receiptForm.ShowDialog();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

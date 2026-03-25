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
    public partial class EWalletPaymentForm: Form
    {
        private List<Order> _ordersToUse;
        public EWalletPaymentForm(List<Order> orders)
        {
            InitializeComponent();
            _ordersToUse = orders;
            labelTotal.Text = $"Total: ₱{OrderStorage.GetTotalPrice()}";
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            ReceiptForm receiptForm = new ReceiptForm(_ordersToUse, "E-Wallet", "");
            receiptForm.ShowDialog();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}

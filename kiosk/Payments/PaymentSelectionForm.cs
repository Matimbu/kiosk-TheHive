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
    public partial class PaymentSelectionForm: Form
    {
        private decimal _totalAmount;
        private List<Order> _ordersToUse;

        public PaymentSelectionForm(List<Order> orders, decimal totalAmount)
        {
            InitializeComponent();

            // Prevent unwanted auto-focus
            TextBox hiddenDummy = new TextBox();
            hiddenDummy.Visible = false;
            hiddenDummy.TabStop = false;
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => { this.ActiveControl = hiddenDummy; };

            _ordersToUse = orders;
            _totalAmount = totalAmount;
            labelTotal.Text = $"Total: ₱{_totalAmount:0.00}";
        }

        private void btn_cash_Click(object sender, EventArgs e)
        {
            ReceiptForm receiptForm = new ReceiptForm(_ordersToUse, "Cash", "");
            receiptForm.ShowDialog();
        }

        private void btn_eWallet_Click(object sender, EventArgs e)
        {

            EWalletPaymentForm EWalletPaymentForm = new EWalletPaymentForm(_ordersToUse);
            EWalletPaymentForm.Show();
        }

        private void btn_card_Click(object sender, EventArgs e)
        {
            CardPaymentForm cardPaymentForm = new CardPaymentForm();
            cardPaymentForm.ShowDialog();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

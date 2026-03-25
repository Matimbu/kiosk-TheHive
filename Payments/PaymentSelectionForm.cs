using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class PaymentSelectionForm : Form
    {
        private readonly decimal      _totalAmount;
        private readonly List<Order>  _ordersToUse;

        public PaymentSelectionForm(List<Order> orders, decimal totalAmount)
        {
            InitializeComponent();

            _ordersToUse  = orders;
            _totalAmount  = totalAmount;
            labelTotal.Text = $"Total: ₱{_totalAmount:N2}";

            // Prevent unwanted auto-focus
            var hiddenDummy = new TextBox { Visible = false, TabStop = false };
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => ActiveControl = hiddenDummy;

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Select Payment Method";

            ThemeManager.StyleButton(btn_cash,   ButtonRole.Accent);
            ThemeManager.StyleButton(btn_eWallet, ButtonRole.Accent);
            ThemeManager.StyleButton(btn_card,   ButtonRole.Accent);
            ThemeManager.StyleButton(btn_back,   ButtonRole.Ghost);
        }

        private void btn_cash_Click(object sender, EventArgs e)
        {
            var cashForm = new CashPaymentForm();
            cashForm.ShowDialog();
            this.Close();
        }

        private void btn_eWallet_Click(object sender, EventArgs e)
        {
            var eWalletForm = new EWalletPaymentForm(_ordersToUse);
            eWalletForm.Show();
            this.Close();
        }

        private void btn_card_Click(object sender, EventArgs e)
        {
            var cardForm = new CardPaymentForm();
            cardForm.ShowDialog();
            this.Close();
        }

        private void btn_back_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

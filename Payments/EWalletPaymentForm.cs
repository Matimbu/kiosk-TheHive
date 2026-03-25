using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class EWalletPaymentForm : Form
    {
        private readonly List<Order> _ordersToUse;

        public EWalletPaymentForm(List<Order> orders)
        {
            InitializeComponent();

            _ordersToUse    = orders;
            labelTotal.Text = $"Total: ₱{OrderStorage.GetTotalPrice():N2}";

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "GCash / E-Wallet Payment";

            ThemeManager.StyleButton(btnConfirm, ButtonRole.Success);
            ThemeManager.StyleButton(btnBack,    ButtonRole.Ghost);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            btnConfirm.Enabled = false;

            var confirm = MessageBox.Show(
                "Has the customer completed the GCash / e-wallet transfer?\n\nPress Yes to confirm and print the receipt.",
                "Confirm E-Wallet Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                btnConfirm.Enabled = true;
                return;
            }

            // Snapshot & save to DB
            var ordersCopy = _ordersToUse.ToList();
            DatabaseManager.SaveTransaction(ordersCopy, "E-Wallet (GCash)");
            OrderStorage.ClearOrders();

            var receiptForm = new ReceiptForm(ordersCopy, "E-Wallet (GCash)", "");
            receiptForm.ShowDialog();

            this.Close();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class CashPaymentForm : Form
    {
        public CashPaymentForm()
        {
            InitializeComponent();

            var hiddenDummy = new TextBox { Visible = false, TabStop = false };
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => ActiveControl = hiddenDummy;

            labelTotal.Text = $"Total: ₱{OrderStorage.GetTotalPrice():N2}";

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Cash Payment";

            ThemeManager.StyleButton(btnConfirm, ButtonRole.Success);
        }

        private void btnConfirm_Click_1(object sender, EventArgs e)
        {
            btnConfirm.Enabled = false;

            var confirm = MessageBox.Show(
                $"Please collect ₱{OrderStorage.GetTotalPrice():N2} cash from the customer.\n\nConfirm payment?",
                "Cash Payment",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
            {
                btnConfirm.Enabled = true;
                return;
            }

            // Snapshot orders, save to DB, clear storage
            var ordersCopy = OrderStorage.GetOrders().ToList();
            DatabaseManager.SaveTransaction(ordersCopy, "Cash");
            OrderStorage.ClearOrders();

            // Show receipt (opens a new window)
            var receiptForm = new ReceiptForm(ordersCopy, "Cash", "");
            receiptForm.ShowDialog();

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}

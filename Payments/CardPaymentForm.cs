using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class CardPaymentForm : Form
    {
        public CardPaymentForm()
        {
            InitializeComponent();

            // ── CVV privacy ───────────────────────────────────────────────
            txtCVV.ReadOnly             = false;
            txtCVV.Enabled              = true;
            txtCVV.UseSystemPasswordChar = false;
            txtCVV.PasswordChar         = '•';

            // ── Card type detection ───────────────────────────────────────
            txtCardNumber.TextChanged += (s, e) =>
            {
                string raw      = Regex.Replace(txtCardNumber.Text, "[^0-9]", "");
                string cardType = DetectCardType(raw);
                lblCardType.Text = cardType != "Unknown" ? $"Card Type: {cardType}" : "";
            };

            // ── Expiry dropdowns ──────────────────────────────────────────
            comboMonth.Items.AddRange(
                System.Linq.Enumerable.Range(1, 12)
                    .Select(i => i.ToString("D2"))
                    .Cast<object>().ToArray());
            comboYear.Items.AddRange(
                System.Linq.Enumerable.Range(DateTime.Now.Year, 10)
                    .Select(y => y.ToString())
                    .Cast<object>().ToArray());
            comboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            comboYear.DropDownStyle  = ComboBoxStyle.DropDownList;

            // ── Input formatting & validation ─────────────────────────────
            txtCardNumber.MaxLength    = 19;
            txtCardNumber.TextChanged += TxtCardNumber_Format;
            txtCardNumber.KeyPress    += DigitOnly_KeyPress;
            txtCVV.KeyPress           += DigitOnly_KeyPress;
            txtCVV.MaxLength           = 3;

            txtCVV.MouseEnter += (s, e) => txtCVV.PasswordChar = '\0';
            txtCVV.MouseLeave += (s, e) => txtCVV.PasswordChar = '•';
            txtCVV.Leave      += (s, e) => txtCVV.PasswordChar = '•';

            // ── Prevent auto-focus ────────────────────────────────────────
            var hiddenDummy = new TextBox { Visible = false, TabStop = false };
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => ActiveControl = hiddenDummy;

            btn_Pay.Click  += btn_Pay_Click;
            btn_Back.Click += (s, e) => this.Close();

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Card Payment";

            ThemeManager.StyleButton(btn_Pay,  ButtonRole.Success);
            ThemeManager.StyleButton(btn_Back, ButtonRole.Ghost);
        }

        // ── Pay button ─────────────────────────────────────────────────────
        private void btn_Pay_Click(object sender, EventArgs e)
        {
            btn_Pay.Enabled = false;

            string cardNumberRaw = Regex.Replace(txtCardNumber.Text, "[^0-9]", "");
            string cardType      = DetectCardType(cardNumberRaw);

            if (cardType == "Unknown")
            {
                ShowWarning("Unsupported or invalid card.\nOnly Visa and MasterCard are accepted.", "Invalid Card");
                btn_Pay.Enabled = true;
                return;
            }

            if (txtCVV.Text.Length != 3)
            {
                ShowWarning("CVV must be 3 digits.", "Invalid CVV");
                btn_Pay.Enabled = true;
                return;
            }

            if (comboMonth.SelectedIndex == -1 || comboYear.SelectedIndex == -1)
            {
                ShowWarning("Please select a valid expiry date.", "Missing Expiry");
                btn_Pay.Enabled = true;
                return;
            }

            // Validate expiry not in the past
            int month = int.Parse(comboMonth.SelectedItem.ToString());
            int year  = int.Parse(comboYear.SelectedItem.ToString());
            if (year < DateTime.Now.Year || (year == DateTime.Now.Year && month < DateTime.Now.Month))
            {
                ShowWarning("This card has expired.", "Expired Card");
                btn_Pay.Enabled = true;
                return;
            }

            // All good — save and show receipt
            var ordersCopy  = OrderStorage.GetOrders().ToList();
            string cardNum  = txtCardNumber.Text.Replace("-", "");

            DatabaseManager.SaveTransaction(ordersCopy, $"Card ({cardType})");
            OrderStorage.ClearOrders();

            var receiptForm = new ReceiptForm(ordersCopy, "Card", cardNum);
            receiptForm.ShowDialog();

            this.Close();
        }

        // ── Helpers ────────────────────────────────────────────────────────
        private string DetectCardType(string n)
        {
            if (string.IsNullOrWhiteSpace(n)) return "Unknown";
            if (Regex.IsMatch(n, @"^4\d{12}(\d{3})?$"))                              return "Visa";
            if (Regex.IsMatch(n, @"^5[1-5]\d{14}$"))                                  return "MasterCard";
            if (Regex.IsMatch(n, @"^2(2[2-9]\d|[3-6]\d{2}|7[01]\d|720)\d{12}$"))     return "MasterCard";
            if (Regex.IsMatch(n, @"^3[47]\d{13}$"))                                   return "American Express";
            if (Regex.IsMatch(n, @"^62\d{14,17}$"))                                   return "UnionPay";
            return "Unknown";
        }

        private void TxtCardNumber_Format(object sender, EventArgs e)
        {
            var txt = (TextBox)sender;
            int oldCaret = txt.SelectionStart;
            string raw = Regex.Replace(txt.Text, "[^0-9]", "");
            if (raw.Length > 16) raw = raw.Substring(0, 16);

            string formatted = "";
            for (int i = 0; i < raw.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formatted += "-";
                formatted += raw[i];
            }

            int dashBefore       = txt.Text.Take(oldCaret).Count(c => c == '-');
            int rawPos           = oldCaret - dashBefore;
            int newCaret         = rawPos + rawPos / 4;

            txt.TextChanged -= TxtCardNumber_Format;
            txt.Text         = formatted;
            txt.SelectionStart = Math.Min(newCaret, txt.Text.Length);
            txt.TextChanged += TxtCardNumber_Format;
        }

        private void DigitOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void ShowWarning(string msg, string title)
            => MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}

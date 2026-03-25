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

            txtCVV.ReadOnly = false;
            txtCVV.Enabled = true;
            txtCVV.UseSystemPasswordChar = false;
            txtCVV.PasswordChar = '•';

            // detect card type
            txtCardNumber.TextChanged += (s, e) =>
            {
                string raw = Regex.Replace(txtCardNumber.Text, "[^0-9]", "");
                string cardType = DetectCardType(raw);
                lblCardType.Text = cardType != "Unknown" ? $"Card Type: {cardType}" : "";
            };

            // Expiry
            comboMonth.Items.AddRange(Enumerable.Range(1, 12).Select(i => i.ToString("D2")).ToArray());
            comboYear.Items.AddRange(Enumerable.Range(DateTime.Now.Year, 10).Select(y => y.ToString()).ToArray());
            comboMonth.DropDownStyle = ComboBoxStyle.DropDownList;
            comboYear.DropDownStyle = ComboBoxStyle.DropDownList;

            // Card number formatting & validation
            txtCardNumber.MaxLength = 19;
            txtCardNumber.TextChanged += TxtCardNumber_Format;
            txtCardNumber.KeyPress += DigitOnly_KeyPress;
            txtCVV.KeyPress += DigitOnly_KeyPress;
            txtCVV.MaxLength = 3;

            // CVV pricavy
            txtCVV.MouseEnter += TxtCVV_MouseEnter;
            txtCVV.MouseLeave += TxtCVV_Hide;
            txtCVV.Leave += TxtCVV_Hide;

            TextBox hiddenDummy = new TextBox();
            hiddenDummy.Visible = false;
            hiddenDummy.TabStop = false;
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => { this.ActiveControl = hiddenDummy; };

            btn_Pay.Click += btn_Pay_Click;
            btn_Back.Click += (s, e) => this.Close();
        }

        private string DetectCardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) return "Unknown";
            if (Regex.IsMatch(cardNumber, @"^4\d{12}(\d{3})?$")) return "Visa";
            if (Regex.IsMatch(cardNumber, @"^5[1-5]\d{14}$")) return "MasterCard";
            if (Regex.IsMatch(cardNumber, @"^3[47]\d{13}$")) return "American Express";
            if (Regex.IsMatch(cardNumber, @"^62\d{14,17}$")) return "UnionPay";
            if (Regex.IsMatch(cardNumber, @"^2(2[2-9]\d|[3-6]\d{2}|7[01]\d|720)\d{12}$")) return "MasterCard";
            return "Unknown";
        }

        private void TxtCardNumber_Format(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            int oldSelectionStart = txt.SelectionStart;
            string raw = Regex.Replace(txt.Text, "[^0-9]", "");
            if (raw.Length > 16) raw = raw.Substring(0, 16);

            string formatted = "";
            for (int i = 0; i < raw.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formatted += "-";
                formatted += raw[i];
            }

            int dashCountBefore = txt.Text.Take(oldSelectionStart).Count(c => c == '-');
            int cursorPositionInRaw = oldSelectionStart - dashCountBefore;
            int newSelectionStart = cursorPositionInRaw + cursorPositionInRaw / 4;

            txt.TextChanged -= TxtCardNumber_Format;
            txt.Text = formatted;
            txt.SelectionStart = Math.Min(newSelectionStart, txt.Text.Length);
            txt.TextChanged += TxtCardNumber_Format;
        }

        private void DigitOnly_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btn_Pay_Click(object sender, EventArgs e)
        {
            btn_Pay.Enabled = false;
            string cardNumberRaw = Regex.Replace(txtCardNumber.Text, "[^0-9]", "");
            string cardType = DetectCardType(cardNumberRaw);

            if (cardType == "Unknown")
            {
                MessageBox.Show("Unsupported or invalid card type.\nOnly Visa and MasterCard are accepted.", "Invalid Card", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_Pay.Enabled = true;
                return;
            }

            if (txtCVV.Text.Length != 3)
            {
                MessageBox.Show("CVV must be 3 digits.", "Invalid", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_Pay.Enabled = true;
                return;
            }

            if (comboMonth.SelectedIndex == -1 || comboYear.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a valid expiry date.", "Missing Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btn_Pay.Enabled = true;
                return;
            }

            var ordersCopy = OrderStorage.GetOrders().ToList();
            OrderStorage.ClearOrders();

            string cardNumber = txtCardNumber.Text.Replace("-", "");
            ReceiptForm receiptForm = new ReceiptForm(ordersCopy, "Card", cardNumber);
            receiptForm.ShowDialog();

            this.Close();
        }

        private void btn_Back_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TxtCVV_MouseLeave(object sender, EventArgs e)
        {
            txtCVV.PasswordChar = '•';
        }

        private void TxtCVV_MouseEnter(object sender, EventArgs e)
        {
            txtCVV.PasswordChar = '\0';
        }

        private void TxtCVV_Hide(object sender, EventArgs e)
        {
            txtCVV.PasswordChar = '•';
        }
    }
}

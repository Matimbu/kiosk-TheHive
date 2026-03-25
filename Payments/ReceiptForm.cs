using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class ReceiptForm : Form
    {
        private readonly List<Order> _cachedOrders;
        private readonly string      _paymentMethod;
        private readonly string      _fullCardNumber;

        public ReceiptForm(List<Order> ordersToUse, string paymentMethod, string fullCardNumber)
        {
            InitializeComponent();

            _cachedOrders   = ordersToUse.ToList();
            _paymentMethod  = paymentMethod;
            _fullCardNumber = fullCardNumber;

            // ── Theme ─────────────────────────────────────────────────────
            ThemeManager.ApplyTheme(this);
            this.Text = "Receipt — The Hive Cafe";

            ThemeManager.StyleButton(btnClose,    ButtonRole.Primary);
            ThemeManager.StyleButton(btnPrint,    ButtonRole.Accent);

            // Receipt text area — monospaced, light background
            txtReceipt.Font      = ThemeManager.FontMono;
            txtReceipt.BackColor = Color.FromArgb(253, 250, 242);
            txtReceipt.ForeColor = ThemeManager.ColorTextDark;
            txtReceipt.ReadOnly  = true;
            txtReceipt.BorderStyle = BorderStyle.None;

            GenerateReceipt();

            var hiddenDummy = new TextBox { Visible = false, TabStop = false };
            this.Controls.Add(hiddenDummy);
            this.Load += (s, e) => ActiveControl = hiddenDummy;
        }

        // ── Receipt Generation ─────────────────────────────────────────────
        private void GenerateReceipt()
        {
            const int W    = 50;   // total width
            const int IW   = 25;   // item name width
            const int QW   = 4;    // qty width
            const int PW   = 9;    // unit price width
            const int SW   = 9;    // subtotal width

            string Line()   => new string('─', W);
            string DblLine()=> new string('═', W);
            string Centre(string s)
            {
                int pad = Math.Max(0, (W - s.Length) / 2);
                return new string(' ', pad) + s;
            }

            var sb = new StringBuilder();

            sb.AppendLine(Centre("☕  THE HIVE CAFE  ☕"));
            sb.AppendLine(Centre("Official Receipt"));
            sb.AppendLine(Centre($"{DateTime.Now:MMM dd, yyyy  hh:mm tt}"));
            sb.AppendLine(DblLine());

            // ── Column headers ─────────────────────────────────────────────
            sb.AppendLine(string.Format(
                "{0,-" + IW + "}{1," + QW + "}{2," + PW + "}{3," + SW + "}",
                "Item", "Qty", "Price", "Total"));
            sb.AppendLine(Line());

            // ── Line items ─────────────────────────────────────────────────
            decimal grandTotal = 0m;

            foreach (var order in _cachedOrders)
            {
                decimal subtotal = order.Price * order.Quantity;
                grandTotal += subtotal;

                // Build item label — include size/temp if present
                string label = order.Product;
                bool   hasOpts = !string.IsNullOrWhiteSpace(order.Size) ||
                                 !string.IsNullOrWhiteSpace(order.Temperature);

                if (hasOpts)
                    label += $" ({order.SizeTemp.Trim()})";

                // Word-wrap long names
                while (label.Length > IW)
                {
                    sb.AppendLine(string.Format(
                        "{0,-" + IW + "}{1," + QW + "}{2," + PW + "}{3," + SW + "}",
                        label.Substring(0, IW),
                        "", "", ""));
                    label = "  " + label.Substring(IW);
                }

                sb.AppendLine(string.Format(
                    "{0,-" + IW + "}{1," + QW + "}{2," + PW + ":₱0.00}{3," + SW + ":₱0.00}",
                    label,
                    order.Quantity,
                    order.Price,
                    subtotal));
            }

            sb.AppendLine(DblLine());

            // ── Totals & payment ───────────────────────────────────────────
            sb.AppendLine(string.Format("{0,-30}{1,20}", "TOTAL AMOUNT:", $"₱{grandTotal:N2}"));

            sb.AppendLine();
            sb.AppendLine($"Payment Method: {_paymentMethod}");

            if (_paymentMethod == "Card" && !string.IsNullOrEmpty(_fullCardNumber))
            {
                string last4    = _fullCardNumber.Length >= 4
                    ? _fullCardNumber.Substring(_fullCardNumber.Length - 4) : "****";
                string cardType = GetCardType(_fullCardNumber);
                sb.AppendLine($"Card Type:      {cardType}");
                sb.AppendLine($"Card Number:    **** **** **** {last4}");
            }

            sb.AppendLine();
            sb.AppendLine(Line());
            sb.AppendLine(Centre("Thank you for visiting The Hive Cafe!"));
            sb.AppendLine(Centre("See you again soon. ☕"));
            sb.AppendLine(Line());

            string receiptText = sb.ToString();
            txtReceipt.Text    = receiptText;

            SaveReceiptFile(receiptText);
        }

        // ── File Save ──────────────────────────────────────────────────────
        private void SaveReceiptFile(string text)
        {
            try
            {
                string folder   = Path.Combine(Application.StartupPath, "Receipts");
                Directory.CreateDirectory(folder);
                string fileName = $"receipt_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                File.WriteAllText(Path.Combine(folder, fileName), text);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not save receipt file:\n{ex.Message}",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // ── Helpers ────────────────────────────────────────────────────────
        private string GetCardType(string n)
        {
            if (string.IsNullOrWhiteSpace(n) || n.Length < 4) return "Unknown";
            if (System.Text.RegularExpressions.Regex.IsMatch(n, @"^4"))        return "Visa";
            if (System.Text.RegularExpressions.Regex.IsMatch(n, @"^5[1-5]"))   return "MasterCard";
            if (System.Text.RegularExpressions.Regex.IsMatch(n, @"^3[47]"))    return "American Express";
            if (System.Text.RegularExpressions.Regex.IsMatch(n, @"^6(?:011|5)")) return "Discover";
            return "Other";
        }

        // ── Buttons ────────────────────────────────────────────────────────
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Restart();   // Return to start screen
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            txtReceipt.SelectAll();
            txtReceipt.Copy();
            MessageBox.Show(
                "Receipt copied to clipboard!\n\nYou can paste it into Notepad and print from there.",
                "Print Receipt",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}

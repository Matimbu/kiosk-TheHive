using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    /// <summary>
    /// Card entry. Same validation as before - card number grouping, type
    /// detection, masked CVV - presented as a proper payment form.
    /// </summary>
    public partial class CardPaymentForm : Form
    {
        private const int BandH = 116;

        private readonly decimal _total;
        private readonly HiveField _cardNumber = new HiveField();
        private readonly HiveField _cvv = new HiveField();
        private readonly ComboBox _month = new ComboBox();
        private readonly ComboBox _year = new ComboBox();
        private readonly HiveButton _pay = new HiveButton();

        private string _cardType = "";

        public CardPaymentForm()
        {
            _total = OrderStorage.GetTotalPrice();

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            int left = Hive.Gutter;
            int width = ClientSize.Width - left * 2;

            // ---- card number ----
            _cardNumber.Label = "CARD NUMBER";
            _cardNumber.Hint = "0000-0000-0000-0000";
            _cardNumber.Bounds = new Rectangle(left, BandH + 20, width, 74);
            _cardNumber.Input.MaxLength = 19;
            _cardNumber.Input.Font = Hive.Sized(Hive.Mono, 12f);
            _cardNumber.Input.KeyPress += DigitOnly;
            _cardNumber.Input.TextChanged += FormatCardNumber;
            _cardNumber.Input.TextChanged += (s, e) =>
            {
                _cardType = DetectCardType(Digits(_cardNumber.Input.Text));
                Invalidate();
            };
            Controls.Add(_cardNumber);

            // ---- expiry + cvv ----
            int row = _cardNumber.Bottom + 14;

            _month.Items.AddRange(Enumerable.Range(1, 12).Select(i => i.ToString("D2")).Cast<object>().ToArray());
            _year.Items.AddRange(Enumerable.Range(DateTime.Now.Year, 10).Select(y => y.ToString()).Cast<object>().ToArray());
            StyleCombo(_month, new Rectangle(left + 12, row + 32, 86, 28), "MM");
            StyleCombo(_year, new Rectangle(left + 134, row + 32, 86, 28), "YYYY");

            _cvv.Label = "CVV";
            _cvv.Hint = "123";
            _cvv.Bounds = new Rectangle(left + 246, row, width - 246, 74);
            _cvv.Input.MaxLength = 3;
            _cvv.Input.PasswordChar = '•';
            _cvv.Input.KeyPress += DigitOnly;
            _cvv.Input.MouseEnter += (s, e) => _cvv.Input.PasswordChar = '\0';
            _cvv.Input.MouseLeave += (s, e) => _cvv.Input.PasswordChar = '•';
            _cvv.Input.Leave += (s, e) => _cvv.Input.PasswordChar = '•';
            Controls.Add(_cvv);

            _expiryRow = row;

            // ---- actions ----
            HiveButton back = new HiveButton();
            back.Text = "Back";
            back.Style = HiveStyle.Outline;
            back.Bounds = new Rectangle(left, ClientSize.Height - 78, 118, 54);
            back.Click += (s, e) => Nav.Back();
            Controls.Add(back);

            _pay.Text = "Pay " + Hive.Money(_total);
            _pay.Style = HiveStyle.Accent;
            _pay.Bounds = new Rectangle(back.Right + 12, ClientSize.Height - 78,
                                        ClientSize.Width - back.Right - 12 - left, 54);
            _pay.Click += Pay;
            Controls.Add(_pay);
        }

        private readonly int _expiryRow;

        private void StyleCombo(ComboBox combo, Rectangle bounds, string placeholder)
        {
            combo.DropDownStyle = ComboBoxStyle.DropDownList;
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = Hive.Surface;
            combo.ForeColor = Hive.Ink;
            combo.Font = Hive.Subhead;
            combo.Bounds = bounds;
            combo.ItemHeight = 24;
            combo.DrawMode = DrawMode.OwnerDrawFixed;
            combo.Tag = placeholder;
            combo.DrawItem += DrawComboItem;
            combo.SelectedIndexChanged += (s, e) => Invalidate();
            Controls.Add(combo);
        }

        /// <summary>Owner-draws the dropdowns so they match the rest of the kiosk.</summary>
        private void DrawComboItem(object sender, DrawItemEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            bool highlighted = (e.State & DrawItemState.Selected) == DrawItemState.Selected
                            && (e.State & DrawItemState.ComboBoxEdit) != DrawItemState.ComboBoxEdit;

            using (SolidBrush back = new SolidBrush(highlighted ? Hive.SurfaceAlt : Hive.Surface))
                g.FillRectangle(back, e.Bounds);

            string text = e.Index >= 0 ? combo.Items[e.Index].ToString() : (string)combo.Tag;
            Color colour = e.Index >= 0 ? Hive.Ink : Hive.Muted;

            Hive.Text(g, text, combo.Font,
                      new Rectangle(e.Bounds.X + 6, e.Bounds.Y, e.Bounds.Width - 6, e.Bounds.Height),
                      colour, Hive.LeftMid);
        }

        // ---- input helpers -------------------------------------------------

        private static string Digits(string text)
        {
            return Regex.Replace(text ?? "", "[^0-9]", "");
        }

        private void DigitOnly(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
        }

        private void FormatCardNumber(object sender, EventArgs e)
        {
            TextBox box = _cardNumber.Input;
            int caret = box.SelectionStart;
            string raw = Digits(box.Text);
            if (raw.Length > 16) raw = raw.Substring(0, 16);

            string formatted = "";
            for (int i = 0; i < raw.Length; i++)
            {
                if (i > 0 && i % 4 == 0) formatted += "-";
                formatted += raw[i];
            }

            int dashesBefore = box.Text.Take(caret).Count(c => c == '-');
            int caretInRaw = caret - dashesBefore;
            int newCaret = caretInRaw + caretInRaw / 4;

            box.TextChanged -= FormatCardNumber;
            box.Text = formatted;
            box.SelectionStart = Math.Min(newCaret, box.Text.Length);
            box.TextChanged += FormatCardNumber;
        }

        private static string DetectCardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber)) return "";
            if (Regex.IsMatch(cardNumber, @"^4\d{12}(\d{3})?$")) return "Visa";
            if (Regex.IsMatch(cardNumber, @"^5[1-5]\d{14}$")) return "MasterCard";
            if (Regex.IsMatch(cardNumber, @"^3[47]\d{13}$")) return "American Express";
            if (Regex.IsMatch(cardNumber, @"^62\d{14,17}$")) return "UnionPay";
            if (Regex.IsMatch(cardNumber, @"^2(2[2-9]\d|[3-6]\d{2}|7[01]\d|720)\d{12}$")) return "MasterCard";
            return "";
        }

        // ---- payment -------------------------------------------------------

        private void Pay(object sender, EventArgs e)
        {
            string raw = Digits(_cardNumber.Input.Text);

            if (DetectCardType(raw) == "")
            {
                Warn("Check the card number", "We accept Visa, MasterCard, AmEx and UnionPay.");
                return;
            }

            if (_cvv.Input.TextLength != 3)
            {
                Warn("CVV needs 3 digits", "It is printed on the back of the card.");
                return;
            }

            if (_month.SelectedIndex < 0 || _year.SelectedIndex < 0)
            {
                Warn("Choose an expiry date", "Pick the month and year shown on the card.");
                return;
            }

            _pay.Enabled = false;

            List<Order> ordersCopy = OrderStorage.GetOrders().ToList();
            OrderStorage.ClearOrders();

            Nav.Go(new ReceiptForm(ordersCopy, "Card", raw));
        }

        private void Warn(string title, string detail)
        {
            _notice = title + "  ·  " + detail;
            Invalidate();
        }

        private string _notice;

        // ---- painting ------------------------------------------------------
        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Canvas);

            RectangleF band = new RectangleF(0, 0, Width, BandH);
            using (SolidBrush b = new SolidBrush(Hive.Teal))
                g.FillRectangle(b, band);

            using (SolidBrush rule = new SolidBrush(Hive.Honey))
                g.FillRectangle(rule, 0, band.Height - 3, Width, 3);

            int left = Hive.Gutter + 4;

            Hive.Text(g, "PAY WITH CARD", Hive.Overline,
                      new Rectangle(left, 24, 240, 16), Color.FromArgb(175, 255, 255, 255), Hive.LeftMid);
            using (Font big = Hive.Sized(Hive.PriceBig, 24f))
                Hive.Text(g, Hive.Money(_total), big,
                          new Rectangle(left, 46, Width - left * 2, 38), Color.White, Hive.LeftMid);

            if (_cardType != "")
                Hive.Text(g, _cardType.ToUpperInvariant(), Hive.Overline,
                          new Rectangle(Width - 190, 30, 174, 18),
                          Color.FromArgb(210, 255, 255, 255), Hive.RightMid);

            // ---- expiry frame, drawn behind the two combo boxes ----
            Hive.Text(g, "EXPIRES", Hive.Overline,
                      new Rectangle(Hive.Gutter + 2, _expiryRow, 200, 16), Hive.Muted, Hive.LeftMid);

            RectangleF frame = new RectangleF(Hive.Gutter, _expiryRow + 20, 230, 53);
            Hive.Fill(g, frame, 12, Hive.Surface);
            Hive.Stroke(g, frame, 12, Hive.Line, 1.3f);
            Hive.Text(g, "/", Hive.Subhead, new Rectangle(Hive.Gutter + 104, _expiryRow + 20, 22, 53),
                      Hive.Muted, Hive.Centered);

            // ---- notice / reassurance ----
            int noticeTop = _cvv.Bottom + 12;
            if (!string.IsNullOrEmpty(_notice))
            {
                RectangleF pill = new RectangleF(Hive.Gutter, noticeTop, Width - Hive.Gutter * 2, 46);
                Hive.Fill(g, pill, 14, Color.FromArgb(28, Hive.Danger));
                Hive.Text(g, _notice, Hive.Caption,
                          Rectangle.Round(RectangleF.Inflate(pill, -14, 0)), Hive.Danger,
                          TextFormatFlags.WordBreak | TextFormatFlags.VerticalCenter);
            }
            else
            {
                Hive.Text(g, "Card details are used for this order only and are not stored.",
                          Hive.Caption, new Rectangle(Hive.Gutter, noticeTop, Width - Hive.Gutter * 2, 40),
                          Hive.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
            }
        }
    }
}

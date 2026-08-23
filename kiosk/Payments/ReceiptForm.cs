using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    /// <summary>
    /// The order receipt: a laid-out till slip on screen, and the same content
    /// written to bin\Debug\Receipts as 40-column text for a thermal printer.
    /// </summary>
    public partial class ReceiptForm : Form
    {
        private const int BandH = 120;

        private readonly List<Order> _orders;
        private readonly string _paymentMethod;
        private readonly string _fullCardNumber;
        private readonly string _orderNumber;
        private readonly DateTime _issued = DateTime.Now;
        private readonly decimal _total;

        private readonly ScrollHost _scroll = new ScrollHost();
        private string _saveError;
        private readonly int _errorHeight;

        public ReceiptForm(List<Order> ordersToUse, string paymentMethod, string fullCardNumber)
        {
            _orders = ordersToUse.ToList();
            _paymentMethod = paymentMethod;
            _fullCardNumber = fullCardNumber;
            _total = _orders.Sum(o => o.Price * o.Quantity);
            _orderNumber = NextOrderNumber();

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            HiveButton done = new HiveButton();
            done.Text = "Done";
            done.Style = HiveStyle.Accent;
            done.Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 78,
                                        ClientSize.Width - Hive.Gutter * 2, 54);
            done.Click += (s, e) => Nav.Home();
            Controls.Add(done);

            // Written first, so the viewport can leave room if it failed.
            Save(BuildText());
            _errorHeight = _saveError == null ? 0 : 34;

            _scroll.Bounds = new Rectangle(0, BandH, ClientSize.Width,
                                           done.Top - 14 - _errorHeight - BandH);
            Controls.Add(_scroll);

            ReceiptPaper paper = new ReceiptPaper(this);
            paper.Width = ClientSize.Width - Hive.Gutter * 2;
            paper.Left = Hive.Gutter;
            paper.Top = 14;
            paper.Height = paper.Measure();
            _scroll.Hook(paper);
            _scroll.Content.Controls.Add(paper);
            _scroll.Measure(16);

            Load += (s, e) => ActiveControl = done;
        }

        // ---- data the paper draws -------------------------------------------

        internal List<Order> Lines { get { return _orders; } }
        internal string OrderNumber { get { return _orderNumber; } }
        internal DateTime Issued { get { return _issued; } }
        internal decimal Total { get { return _total; } }
        internal string PaymentMethod { get { return _paymentMethod; } }

        internal string CardLine
        {
            get
            {
                if (_paymentMethod != "Card" || string.IsNullOrEmpty(_fullCardNumber)) return null;
                string last4 = _fullCardNumber.Length >= 4
                             ? _fullCardNumber.Substring(_fullCardNumber.Length - 4)
                             : "****";
                return CardType(_fullCardNumber) + " ending " + last4;
            }
        }

        /// <summary>
        /// A per-day sequence kept beside the saved receipts, so the number a
        /// guest is given at the counter does not restart every time the kiosk
        /// is relaunched.
        /// </summary>
        private static string NextOrderNumber()
        {
            try
            {
                string folder = Path.Combine(Application.StartupPath, "Receipts");
                Directory.CreateDirectory(folder);
                string file = Path.Combine(folder, "counter.txt");

                string today = DateTime.Now.ToString("yyyyMMdd");
                int next = 1;

                if (File.Exists(file))
                {
                    string[] parts = File.ReadAllText(file).Split(',');
                    int previous;
                    if (parts.Length == 2 && parts[0] == today &&
                        int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out previous))
                        next = previous + 1;
                }

                File.WriteAllText(file, today + "," + next.ToString(CultureInfo.InvariantCulture));
                return next.ToString("D3", CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                return DateTime.Now.ToString("HHmm");   // still unique enough to call out
            }
        }

        private static string CardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4) return "Card";
            if (Regex.IsMatch(cardNumber, @"^4")) return "Visa";
            if (Regex.IsMatch(cardNumber, @"^5[1-5]")) return "MasterCard";
            if (Regex.IsMatch(cardNumber, @"^3[47]")) return "AmEx";
            if (Regex.IsMatch(cardNumber, @"^6(?:011|5)")) return "Discover";
            return "Card";
        }

        // ---- printable copy --------------------------------------------------

        /// <summary>40 columns, the usual width of a thermal till roll.</summary>
        private string BuildText()
        {
            const int cols = 40;
            StringBuilder r = new StringBuilder();

            Action<string> centre = t => r.AppendLine(new string(' ', Math.Max(0, (cols - t.Length) / 2)) + t);
            Action rule = () => r.AppendLine(new string('-', cols));
            Action<string, string> row = (l, v) =>
                r.AppendLine(l + new string(' ', Math.Max(1, cols - l.Length - v.Length)) + v);

            centre(CafeInfo.Name);
            centre(CafeInfo.Branch);
            centre(CafeInfo.Address);
            centre(CafeInfo.Contact);
            if (!string.IsNullOrEmpty(CafeInfo.TaxId)) centre("TIN " + CafeInfo.TaxId);
            r.AppendLine();

            centre("ORDER No. " + _orderNumber);
            rule();
            row("Date", _issued.ToString("dd MMM yyyy HH:mm"));
            row("Terminal", CafeInfo.Terminal);
            row("Payment", _paymentMethod);
            if (CardLine != null) row("Card", CardLine);
            rule();
            row("ITEM", "AMOUNT");
            rule();

            foreach (Order o in _orders)
            {
                string name = o.Product;
                if (name.Length > cols - 12) name = name.Substring(0, cols - 13) + ".";
                row(name, (o.Price * o.Quantity).ToString("N2"));

                string detail = OrderText.Describe(o);
                string qty = "  " + o.Quantity + " x " + o.Price.ToString("N2");
                r.AppendLine(detail == "Regular" ? qty : qty + "  (" + detail + ")");
            }

            rule();
            row("TOTAL", Hive.Peso + _total.ToString("N2"));
            rule();
            r.AppendLine();
            centre("Thank you, see you again!");
            r.AppendLine();
            if (!CafeInfo.IssuesOfficialReceipts)
                centre("THIS IS NOT AN OFFICIAL RECEIPT");

            return r.ToString();
        }

        private void Save(string text)
        {
            try
            {
                string folder = Path.Combine(Application.StartupPath, "Receipts");
                Directory.CreateDirectory(folder);
                File.WriteAllText(
                    Path.Combine(folder, "receipt_" + _issued.ToString("yyyyMMdd_HHmmss") + "_" + _orderNumber + ".txt"),
                    text);
            }
            catch (Exception ex)
            {
                _saveError = "Receipt could not be saved: " + ex.Message;
            }
        }

        // ---- painting --------------------------------------------------------

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

            RectangleF hex = new RectangleF(Width / 2f - 23, 12, 46, 46);
            Hive.FillHex(g, hex, Color.FromArgb(48, 255, 255, 255));
            Marks.Draw(g, Mark.Check, RectangleF.Inflate(hex, -13, -13), Color.White, 2.2f);

            Hive.Text(g, "Payment received", Hive.Title,
                      new Rectangle(0, 60, Width, 32), Color.White, Hive.Centered);
            Hive.Text(g, Hive.Money(_total) + " paid by " + _paymentMethod.ToLowerInvariant(), Hive.Caption,
                      new Rectangle(0, 92, Width, 20), Color.FromArgb(190, 255, 255, 255), Hive.Centered);

            if (!string.IsNullOrEmpty(_saveError))
                Hive.Text(g, _saveError, Hive.Caption,
                          new Rectangle(Hive.Gutter, Height - 78 - _errorHeight - 8, Width - Hive.Gutter * 2, 32),
                          Hive.Danger, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }
    }

    /// <summary>
    /// The till slip itself. One layout routine both measures and draws, so the
    /// paper is always exactly as tall as its contents.
    /// </summary>
    internal class ReceiptPaper : HiveControl
    {
        private const int Pad = 20;

        private readonly ReceiptForm _receipt;

        public ReceiptPaper(ReceiptForm receipt)
        {
            _receipt = receipt;
            BackColor = Hive.Canvas;
        }

        public int Measure()
        {
            return Compose(null);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Canvas);

            RectangleF paper = new RectangleF(0, 4, Width - 1, Height - 12);
            using (SolidBrush b = new SolidBrush(Hive.Surface))
                g.FillRectangle(b, paper);
            Serrate(g, paper);

            Compose(g);
        }

        /// <summary>Draws when g is supplied, measures when it is null.</summary>
        private int Compose(Graphics g)
        {
            int left = Pad;
            int right = Width - Pad;
            int w = right - left;
            int y = 4 + Pad;

            // ---- masthead ----
            if (g != null)
            {
                RectangleF hex = new RectangleF(Width / 2f - 21, y, 42, 42);
                Hive.FillHex(g, hex, Hive.SurfaceAlt);
                Image logo = MenuCatalog.Logo;
                if (logo != null)
                {
                    using (GraphicsPath clip = Hive.Hexagon(hex))
                    {
                        Region saved = g.Clip;
                        g.SetClip(clip, CombineMode.Intersect);
                        Hive.ImageCover(g, logo, hex, clip);
                        g.Clip = saved;
                    }
                }
            }
            y += 42 + 8;

            if (g != null)
                Hive.TextTracked(g, CafeInfo.Name, Hive.Sized(Hive.Overline, 10.5f),
                                 new Rectangle(left, y, w, 20), Hive.Ink, 2.6f, true);
            y += 20;

            y = Centred(g, CafeInfo.Branch, Hive.Caption, Hive.InkSoft, left, y, w, 15);
            y = Centred(g, CafeInfo.Address, Hive.Caption, Hive.Muted, left, y, w, 15);
            y = Centred(g, CafeInfo.Contact, Hive.Caption, Hive.Muted, left, y, w, 15);
            if (!string.IsNullOrEmpty(CafeInfo.TaxId))
                y = Centred(g, "TIN " + CafeInfo.TaxId, Hive.Caption, Hive.Muted, left, y, w, 15);

            y += 10;
            y = Dashes(g, left, y, w);
            y += 12;

            // ---- order number, the thing the guest shows at the counter ----
            if (g != null)
            {
                Hive.TextTracked(g, "ORDER NUMBER", Hive.Overline,
                                 new Rectangle(left, y, w, 16), Hive.Muted, 2f, true);
                using (Font big = Hive.Sized(Hive.PriceBig, 31f))
                    Hive.Text(g, _receipt.OrderNumber, big,
                              new Rectangle(left, y + 16, w, 42), Hive.Teal, Hive.Centered);
            }
            y += 16 + 42 + 8;

            y = Dashes(g, left, y, w);
            y += 10;

            // ---- meta, kept to three rows so the totals stay above the fold ----
            y = Row(g, "Date", _receipt.Issued.ToString("dd MMM yyyy, HH:mm"), left, y, w);
            y = Row(g, "Terminal", CafeInfo.Terminal, left, y, w);
            y = Row(g, "Payment",
                    _receipt.CardLine == null ? _receipt.PaymentMethod : _receipt.CardLine,
                    left, y, w);

            y += 10;
            y = Dashes(g, left, y, w);
            y += 8;

            if (g != null)
            {
                Hive.TextTracked(g, "ITEM", Hive.Overline, new Rectangle(left, y, w, 16), Hive.Muted, 1.4f, false);
                Hive.Text(g, "AMOUNT", Hive.Overline, new Rectangle(left, y, w, 16), Hive.Muted, Hive.RightMid);
            }
            y += 20;
            y = Hairline(g, left, y, w);
            y += 8;

            // ---- lines ----
            foreach (Order o in _receipt.Lines)
            {
                if (g != null)
                {
                    Hive.Text(g, o.Product, Hive.Serif,
                              new Rectangle(left, y, w - 90, 20), Hive.Ink, Hive.LeftMid);
                    Hive.Text(g, (o.Price * o.Quantity).ToString("N2"), Hive.Price,
                              new Rectangle(left, y, w, 20), Hive.Ink, Hive.RightMid);
                }
                y += 20;

                string detail = OrderText.Describe(o);
                string sub = o.Quantity + " x " + Hive.Money(o.Price);
                if (detail != "Regular") sub += "   ·   " + detail;

                if (g != null)
                    Hive.Text(g, sub, Hive.Caption, new Rectangle(left, y, w, 18), Hive.Muted, Hive.LeftMid);
                y += 20;
            }

            y += 2;
            y = Dashes(g, left, y, w);
            y += 10;

            // ---- totals ----
            if (g != null)
            {
                Hive.Text(g, "TOTAL", Hive.Subhead, new Rectangle(left, y, w, 32), Hive.Ink, Hive.LeftMid);
                using (Font big = Hive.Sized(Hive.PriceBig, 22f))
                    Hive.Text(g, Hive.Money(_receipt.Total), big,
                              new Rectangle(left, y, w, 32), Hive.Ink, Hive.RightMid);
            }
            y += 36;

            y = Dashes(g, left, y, w);
            y += 10;

            y = Centred(g, "Thank you, see you again!", Hive.Body, Hive.InkSoft, left, y, w, 20);

            if (!CafeInfo.IssuesOfficialReceipts)
            {
                y += 8;
                if (g != null)
                    Hive.TextTracked(g, "THIS IS NOT AN OFFICIAL RECEIPT", Hive.Overline,
                                     new Rectangle(left, y, w, 16), Hive.Muted, 1.1f, true);
                y += 18;
            }

            return y + Pad;
        }

        // ---- layout helpers --------------------------------------------------

        private static int Centred(Graphics g, string text, Font font, Color colour,
                                   int left, int y, int w, int h)
        {
            if (g != null) Hive.Text(g, text, font, new Rectangle(left, y, w, h), colour, Hive.Centered);
            return y + h;
        }

        private static int Row(Graphics g, string label, string value, int left, int y, int w)
        {
            if (g != null)
            {
                Hive.Text(g, label, Hive.Caption, new Rectangle(left, y, w, 20), Hive.Muted, Hive.LeftMid);
                Hive.Text(g, value, Hive.BodyBold, new Rectangle(left, y, w, 20), Hive.Ink, Hive.RightMid);
            }
            return y + 18;
        }

        private static int Dashes(Graphics g, int left, int y, int w)
        {
            if (g != null)
            {
                using (Pen p = new Pen(Hive.Line, 1))
                {
                    p.DashStyle = DashStyle.Custom;
                    p.DashPattern = new float[] { 3f, 3f };
                    g.DrawLine(p, left, y, left + w, y);
                }
            }
            return y + 1;
        }

        private static int Hairline(Graphics g, int left, int y, int w)
        {
            if (g != null)
                using (Pen p = new Pen(Hive.LineSoft, 1))
                    g.DrawLine(p, left, y, left + w, y);
            return y + 1;
        }

        /// <summary>Torn-paper edges, so the slip reads as a slip.</summary>
        private void Serrate(Graphics g, RectangleF paper)
        {
            const float tooth = 12f;
            using (SolidBrush b = new SolidBrush(Hive.Canvas))
            {
                for (float x = paper.X; x < paper.Right; x += tooth)
                {
                    g.FillPolygon(b, new[]
                    {
                        new PointF(x, paper.Y - 1),
                        new PointF(x + tooth / 2f, paper.Y + 6),
                        new PointF(x + tooth, paper.Y - 1)
                    });
                    g.FillPolygon(b, new[]
                    {
                        new PointF(x, paper.Bottom + 1),
                        new PointF(x + tooth / 2f, paper.Bottom - 6),
                        new PointF(x + tooth, paper.Bottom + 1)
                    });
                }
            }
        }
    }
}

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
    public partial class ReceiptForm : Form
    {
        private const int BandH = 120;

        private readonly List<Order> _orders;
        private readonly string _paymentMethod;
        private readonly string _orderNumber;
        private readonly string _displayNumber;
        private readonly DateTime _issued = DateTime.Now;
        private readonly decimal _total;

        private readonly ScrollHost _scroll = new ScrollHost();
        private readonly Label _scrollCue = new Label();
        private string _saveError;
        private readonly int _errorHeight;

        public static void Submit(string method)
        {
            try
            {
                var saved = LocalStore.Submit(method);
                var receipt = new ReceiptForm(saved);
                Shell.Current.Complete(receipt);
            }
            catch (Exception ex)
            {
                NoticeForm.Problem("Order not completed",
                                   "Your order is still here, so you can try again.",
                                   ex.Message);
            }
        }

        private ReceiptForm(SavedOrder saved)
        {
            _orders = saved.Lines;
            _paymentMethod = saved.Method;
            _total = saved.Total;
            _orderNumber = saved.Id;
            _displayNumber = saved.DisplayNumber;
            _issued = saved.Created;
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

            // save first so we know whether to leave room for the error line
            Save(BuildText());
            _errorHeight = _saveError == null ? 0 : 34;

            _scroll.Bounds = new Rectangle(0, BandH, ClientSize.Width,
                                           done.Top - 38 - _errorHeight - BandH);
            Controls.Add(_scroll);

            ReceiptPaper paper = new ReceiptPaper(this);
            paper.Width = ClientSize.Width - Hive.Gutter * 2;
            paper.Left = Hive.Gutter;
            paper.Top = 14;
            paper.Height = paper.Measure();
            _scroll.Hook(paper);
            _scroll.Content.Controls.Add(paper);
            _scroll.Measure(16);

            _scrollCue.Text = GuestText.T("SWIPE UP FOR MORE");
            _scrollCue.Font = Hive.Overline;
            _scrollCue.ForeColor = Hive.Muted;
            _scrollCue.BackColor = Hive.Canvas;
            _scrollCue.TextAlign = ContentAlignment.MiddleCenter;
            _scrollCue.Bounds = new Rectangle(0, _scroll.Bottom + 4, ClientSize.Width, 24);
            _scrollCue.Visible = false;
            Controls.Add(_scrollCue);
            _scrollCue.BringToFront();
            _scroll.PositionChanged += (s, e) => UpdateScrollCue();
            UpdateScrollCue();

            Load += (s, e) => ActiveControl = done;
        }

        internal List<Order> Lines { get { return _orders; } }
        internal string OrderNumber { get { return _orderNumber; } }
        internal string DisplayNumber { get { return _displayNumber; } }
        internal DateTime Issued { get { return _issued; } }
        internal decimal Total { get { return _total; } }
        internal string PaymentMethod { get { return _paymentMethod; } }

        internal string CardLine { get { return null; } }

        private void UpdateScrollCue()
        {
            _scrollCue.Visible = _scroll.Content.Bottom > _scroll.Height + 2;
        }

        // Breaks a line to the slip width, preferring a space, and indents the
        // continuation so it still reads as part of the item above it.
        private static IEnumerable<string> Fold(string line, int cols)
        {
            while (line.Length > cols)
            {
                int cut = line.LastIndexOf(' ', cols - 1);
                if (cut <= 4) cut = cols - 1;        // nothing sensible to break on
                yield return line.Substring(0, cut);
                line = "     " + line.Substring(cut).TrimStart();
            }
            yield return line;
        }

        private string BuildText()
        {
            const int cols = 40;
            StringBuilder r = new StringBuilder();

            Action<string> center = t => { t = GuestText.T(t); r.AppendLine(new string(' ', Math.Max(0, (cols - t.Length) / 2)) + t); };
            Action rule = () => r.AppendLine(new string('-', cols));
            Action<string, string> row = (l, v) => {
                l = GuestText.T(l);
                r.AppendLine(l + new string(' ', Math.Max(1, cols - l.Length - v.Length)) + v);
            };

            center(CafeInfo.Name);
            center(CafeInfo.Branch);
            center(CafeInfo.Address);
            center(CafeInfo.Contact);
            if (!string.IsNullOrEmpty(CafeInfo.TaxId)) center("TIN " + CafeInfo.TaxId);
            r.AppendLine();

            center((GuestText.Filipino ? "ORDER BLG. " : "ORDER No. ") + _displayNumber);
            center(_paymentMethod == "Cash" ? "Show this code at the counter" : "Keep this code for reference");
            center(_paymentMethod == "Cash" ? "PENDING COUNTER PAYMENT" : "DEMO - NO PAYMENT TAKEN");
            rule();
            row("Date", _issued.ToString("dd MMM yyyy HH:mm"));
            row("Terminal", CafeInfo.Terminal);
            row("Payment", _paymentMethod);
            if (CardLine != null) row("Card", CardLine);
            rule();
            row("ITEM", GuestText.T("AMOUNT"));
            rule();

            foreach (Order o in _orders)
            {
                string name = o.Product;
                if (name.Length > cols - 12) name = name.Substring(0, cols - 13) + ".";
                row(name, (o.Price * o.Quantity).ToString("N2"));

                string detail = OrderText.Describe(o);
                string qty = "  " + o.Quantity + " x " + o.Price.ToString("N2");

                if (detail == "Regular")
                {
                    r.AppendLine(qty);
                }
                else
                {
                    // Filipino customizations run longer than the English ones and a
                    // 40-column slip has none to spare. Drop the whole customization
                    // to its own line rather than splitting the phrase across two.
                    string full = qty + "  (" + detail + ")";
                    if (full.Length <= cols) r.AppendLine(full);
                    else
                    {
                        r.AppendLine(qty);
                        foreach (string part in Fold("     (" + detail + ")", cols))
                            r.AppendLine(part);
                    }
                }
            }

            rule();
            row("TOTAL", Hive.Peso + _total.ToString("N2"));
            rule();
            r.AppendLine();
            center("Thank you, see you again!");
            r.AppendLine();
            if (!CafeInfo.IssuesOfficialReceipts)
                center("THIS IS NOT AN OFFICIAL RECEIPT");

            return r.ToString();
        }

        private void Save(string text)
        {
            try
            {
                string folder = LocalStore.ReceiptFolder;
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

            Hive.Text(g, _paymentMethod == "Cash" ? "Order placed" : "Demo order created", Hive.Title,
                      new Rectangle(0, 60, Width, 32), Color.White, Hive.Centered);
            Hive.Text(g, _paymentMethod == "Cash" ? "Please pay " + Hive.Money(_total) + " at the counter" : "No payment taken — " + _paymentMethod, Hive.Caption,
                      new Rectangle(0, 92, Width, 20), Color.FromArgb(190, 255, 255, 255), Hive.Centered);

            if (!string.IsNullOrEmpty(_saveError))
                Hive.Text(g, _saveError, Hive.Caption,
                          new Rectangle(Hive.Gutter, Height - 78 - _errorHeight - 8, Width - Hive.Gutter * 2, 32),
                          Hive.Danger, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);

        }
    }

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

        private int Compose(Graphics g)
        {
            int left = Pad;
            int right = Width - Pad;
            int w = right - left;
            int y = Pad - 4;

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
            y += 42;

            if (g != null)
                Hive.TextTracked(g, CafeInfo.Name, Hive.Sized(Hive.Overline, 10.5f),
                                 new Rectangle(left, y, w, 20), Hive.Ink, 2.6f, true);
            y += 20;

            y = CenterLine(g, CafeInfo.Branch, Hive.Caption, Hive.InkSoft, left, y, w, 14);
            y = CenterLine(g, CafeInfo.Address, Hive.Caption, Hive.Muted, left, y, w, 14);
            y = CenterLine(g, CafeInfo.Contact, Hive.Caption, Hive.Muted, left, y, w, 14);
            if (!string.IsNullOrEmpty(CafeInfo.TaxId))
                y = CenterLine(g, "TIN " + CafeInfo.TaxId, Hive.Caption, Hive.Muted, left, y, w, 15);

            y += 4;
            y = Hairline(g, left, y, w);
            y += 6;

            if (g != null)
            {
                Hive.TextTracked(g, "ORDER NUMBER", Hive.Overline,
                                 new Rectangle(left, y, w, 18), Hive.Muted, 1.3f, true);
                Hive.Text(g, _receipt.DisplayNumber, Hive.PriceBig,
                          new Rectangle(left, y + 17, w, 40), Hive.Teal, Hive.Centered);
                Hive.Text(g, _receipt.PaymentMethod == "Cash" ? "Show this code at the counter" : "Keep this code for reference",
                          Hive.Caption, new Rectangle(left, y + 55, w, 20), Hive.Muted, Hive.Centered);
            }
            y += 17 + 40 + 20 + 4;

            y = Hairline(g, left, y, w);
            y += 10;

            y = Row(g, "Date", _receipt.Issued.ToString("dd MMM yyyy, HH:mm"), left, y, w);
            y = Row(g, "Terminal", CafeInfo.Terminal, left, y, w);
            y = Row(g, "Status", _receipt.PaymentMethod == "Cash" ? "Pending counter payment" : "Demo — no payment taken", left, y, w);
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

            y = CenterLine(g, "Thank you, see you again!", Hive.Body, Hive.InkSoft, left, y, w, 20);

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

        private static int CenterLine(Graphics g, string text, Font font, Color color,
                                   int left, int y, int w, int h)
        {
            if (g != null) Hive.Text(g, text, font, new Rectangle(left, y, w, h), color, Hive.Centered);
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

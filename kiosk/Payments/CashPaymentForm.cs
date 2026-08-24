using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    public partial class CashPaymentForm : Form
    {
        private const int BandH = 116;

        private readonly decimal _total;

        public CashPaymentForm()
        {
            _total = OrderStorage.GetTotalPrice();

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            HiveButton back = new HiveButton();
            back.Text = "Back";
            back.Style = HiveStyle.Outline;
            back.Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 78, 128, 54);
            back.Click += (s, e) => Nav.Back();
            Controls.Add(back);

            HiveButton confirm = new HiveButton();
            confirm.Text = "Payment handed over";
            confirm.Style = HiveStyle.Accent;
            confirm.Bounds = new Rectangle(back.Right + 12,
                                           ClientSize.Height - 78,
                                           ClientSize.Width - back.Right - 12 - Hive.Gutter, 54);
            confirm.Click += Confirm;
            Controls.Add(confirm);
        }

        private void Confirm(object sender, EventArgs e)
        {
            List<Order> ordersCopy = OrderStorage.GetOrders().ToList();
            OrderStorage.ClearOrders();

            Nav.Go(new ReceiptForm(ordersCopy, "Cash", ""));
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

            int left = Hive.Gutter + 4;

            Hive.Text(g, "PAY WITH CASH", Hive.Overline,
                      new Rectangle(left, 24, 240, 16), Color.FromArgb(175, 255, 255, 255), Hive.LeftMid);
            using (Font big = Hive.Sized(Hive.PriceBig, 24f))
                Hive.Text(g, Hive.Money(_total), big,
                          new Rectangle(left, 46, Width - left * 2, 38), Color.White, Hive.LeftMid);

            int free = Height - 78 - BandH;
            RectangleF card = new RectangleF(Hive.Gutter, BandH + (free - 250) / 2f,
                                             Width - Hive.Gutter * 2, 250);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Surface);
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.Line, 1.2f);

            RectangleF disc = new RectangleF(card.X + (card.Width - 72) / 2f, card.Y + 34, 72, 72);
            Hive.FillHex(g, disc, Hive.HoneyWash);
            using (Font f = Hive.Sized(Hive.PriceBig, 22f))
                Hive.Text(g, Hive.Peso, f, Rectangle.Round(disc), Hive.Honey, Hive.Centered);

            int tx = (int)card.X + 28;
            int tw = (int)card.Width - 56;

            Hive.Text(g, "Please pay at the counter", Hive.Title,
                      new Rectangle(tx, (int)disc.Bottom + 16, tw, 32), Hive.Ink, Hive.Centered);
            Hive.Text(g, "Hand the exact amount to our cashier, then tap the button below to print your receipt.",
                      Hive.Body, new Rectangle(tx, (int)disc.Bottom + 52, tw, 60), Hive.Muted,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);

            Hive.TextTracked(g, "YOUR ORDER IS HELD UNTIL PAYMENT IS CONFIRMED", Hive.Overline,
                             new Rectangle(Hive.Gutter, (int)card.Bottom + 16, Width - Hive.Gutter * 2, 20),
                             Hive.Muted, 1.1f, true);
        }
    }
}

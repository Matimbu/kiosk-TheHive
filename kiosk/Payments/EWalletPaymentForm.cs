using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    public partial class EWalletPaymentForm : Form
    {
        private const int BandH = 116;

        private readonly List<Order> _ordersToUse;
        private readonly decimal _total;

        public EWalletPaymentForm(List<Order> orders)
        {
            _ordersToUse = orders;
            _total = OrderStorage.GetTotalPrice();

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            HiveButton back = new HiveButton();
            back.Text = "Back";
            back.Style = HiveStyle.Outline;
            back.Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 78, 118, 54);
            back.Click += (s, e) => Nav.Back();
            Controls.Add(back);

            HiveButton confirm = new HiveButton();
            confirm.Text = "Create demo order";
            confirm.Style = HiveStyle.Accent;
            confirm.Bounds = new Rectangle(back.Right + 12, ClientSize.Height - 78,
                                           ClientSize.Width - back.Right - 12 - Hive.Gutter, 54);
            confirm.Click += Confirm;
            Controls.Add(confirm);
        }

        private void Confirm(object sender, EventArgs e)
        {
            ReceiptForm.Submit("E-Wallet");
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

            Hive.Text(g, "PAY WITH E-WALLET", Hive.Overline,
                      new Rectangle(left, 24, 260, 16), Color.FromArgb(175, 255, 255, 255), Hive.LeftMid);
            using (Font big = Hive.Sized(Hive.PriceBig, 24f))
                Hive.Text(g, Hive.Money(_total), big,
                          new Rectangle(left, 46, Width - left * 2, 38), Color.White, Hive.LeftMid);

            ProgressGuide.Draw(g, new Rectangle(Hive.Gutter, ClientSize.Height - 130, Width - Hive.Gutter * 2, 36), 3);

            RectangleF card = new RectangleF(Hive.Gutter, BandH + 22, Width - Hive.Gutter * 2, 318);
            Hive.Shadow(g, card, 20, 4, 44);
            Hive.Fill(g, card, 20, Hive.Surface);

            Hive.Text(g, "E-WALLET DEMO", Hive.Overline,
                      new Rectangle((int)card.X, (int)card.Y + 20, (int)card.Width, 18), Hive.Muted, Hive.Centered);

            RectangleF qr = new RectangleF(card.X + (card.Width - 208) / 2f, card.Y + 48, 208, 208);
            Hive.Fill(g, RectangleF.Inflate(qr, 10, 10), 16, Hive.Canvas);

            Image code = null;
            if (code != null)
            {
                using (GraphicsPath clip = Hive.Rounded(qr, 8))
                    Hive.ImageCover(g, code, qr, clip);
            }
            else
            {
                Hive.Text(g, "Demo - no payment QR", Hive.Body, Rectangle.Round(qr), Hive.Muted, Hive.Centered);
            }

            Hive.Text(g, "No wallet provider is connected. No money will be transferred.", Hive.Caption,
                      new Rectangle((int)card.X + 20, (int)qr.Bottom + 20, (int)card.Width - 40, 36), Hive.Muted,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);

            Hive.Text(g, "This creates a demonstration order only.", Hive.Caption,
                      new Rectangle(Hive.Gutter, (int)card.Bottom + 16, Width - Hive.Gutter * 2, 40), Hive.Muted,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);
        }
    }
}

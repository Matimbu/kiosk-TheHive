using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    public partial class PaymentSelectionForm : Form
    {
        private const int BandH = 128;

        private readonly decimal _totalAmount;
        private readonly List<Order> _ordersToUse;

        public PaymentSelectionForm(List<Order> orders, decimal totalAmount)
        {
            _ordersToUse = orders;
            _totalAmount = totalAmount;

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            int y = BandH + 26;
            y = AddMethod(y, Mark.Cash, "Cash", "Pay our cashier at the counter", PayCash);
            y = AddMethod(y, Mark.Card, "Card", "Visa or MasterCard, tap or insert", PayCard);
            y = AddMethod(y, Mark.Wallet, "E-wallet", "GCash and other QR wallets", PayEWallet);

            HiveButton cancel = new HiveButton();
            cancel.Text = "Not yet, go back";
            cancel.Style = HiveStyle.Ghost;
            cancel.Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 74,
                                          ClientSize.Width - Hive.Gutter * 2, 50);
            cancel.Click += (s, e) => Nav.Back();
            Controls.Add(cancel);
        }

        private int AddMethod(int top, Mark glyph, string title, string hint, EventHandler onPick)
        {
            MethodTile tile = new MethodTile(glyph, title, hint);
            tile.Bounds = new Rectangle(Hive.Gutter - 3, top, ClientSize.Width - (Hive.Gutter - 3) * 2, 86);
            tile.Click += onPick;
            Controls.Add(tile);
            return tile.Bottom + 8;
        }

        private void Settle(Form paymentForm)
        {
            Nav.Go(paymentForm);
        }

        private void PayCash(object sender, EventArgs e)
        {
            Settle(new CashPaymentForm());
        }

        private void PayCard(object sender, EventArgs e)
        {
            Settle(new CardPaymentForm());
        }

        private void PayEWallet(object sender, EventArgs e)
        {
            Settle(new EWalletPaymentForm(_ordersToUse));
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

            Hive.Text(g, "AMOUNT DUE", Hive.Overline,
                      new Rectangle(left, 26, 240, 16), Color.FromArgb(175, 255, 255, 255), Hive.LeftMid);

            using (Font big = Hive.Sized(Hive.PriceBig, 26f))
                Hive.Text(g, Hive.Money(_totalAmount), big,
                          new Rectangle(left, 48, Width - left * 2, 40), Color.White, Hive.LeftMid);

            Hive.Text(g, "HOW WOULD YOU LIKE TO PAY?", Hive.Overline,
                      new Rectangle(left, BandH + 4, Width - left * 2, 18), Hive.Muted, Hive.LeftMid);
        }
    }
}

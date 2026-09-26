using System;
using System.Drawing;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk.Payments
{
    public partial class CardPaymentForm : Form
    {
        public CardPaymentForm()
        {
            InitializeComponent();
            BackColor = Hive.Canvas;
            var back = new HiveButton { Text = "Back", Bounds = new Rectangle(24, 620, 120, 54), Style = HiveStyle.Outline };
            var demo = new HiveButton { Text = "Create demo order", Bounds = new Rectangle(156, 620, 300, 54), Style = HiveStyle.Accent };
            back.Click += (s, e) => Nav.Back();
            demo.Click += (s, e) => ReceiptForm.Submit("Card");
            Controls.Add(back); Controls.Add(demo);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Canvas);
            using (var brush = new SolidBrush(Hive.Teal)) g.FillRectangle(brush, 0, 0, Width, 116);
            using (var brush = new SolidBrush(Hive.Honey)) g.FillRectangle(brush, 0, 113, Width, 3);
            Hive.Text(g, "CARD PAYMENT DEMO", Hive.Overline, new Rectangle(24, 24, 420, 20), Color.White, Hive.LeftMid);
            Hive.Text(g, Hive.Money(OrderStorage.GetTotal()), Hive.PriceBig, new Rectangle(24, 50, 420, 40), Color.White, Hive.LeftMid);
            ProgressGuide.Draw(g, new Rectangle(Hive.Gutter, ClientSize.Height - 142, Width - Hive.Gutter * 2, 36), 3);
            var card = new RectangleF(24, 164, 432, 300);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Surface);
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.Line, 1);
            Marks.Draw(g, Mark.Card, new RectangleF(212, 192, 56, 48), Hive.Teal, 2);
            Hive.Text(g, "Try the card order flow", Hive.Title, new Rectangle(44, 260, 392, 34), Hive.Ink, Hive.Centered);
            Hive.Text(g, "No payment terminal is connected.\nNo money will be charged.\n\nNo card details are needed.", Hive.Body,
                new Rectangle(54, 310, 372, 120), Hive.Muted, TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
        }
    }
}

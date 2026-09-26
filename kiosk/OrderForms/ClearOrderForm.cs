using System;
using System.Drawing;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    public sealed class ClearOrderForm : Form
    {
        private readonly HiveButton _keep;
        private readonly HiveButton _confirm;

        public ClearOrderForm()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            ClientSize = new Size(Hive.ScreenW, Hive.ScreenH);
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            AppHeader header = new AppHeader { Title = "Clear order" };
            header.ShowBack((s, e) => Nav.Back());
            Controls.Add(header);

            _keep = new HiveButton
            {
                Text = "Keep order",
                Style = HiveStyle.Outline,
                Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 88, 198, 58)
            };
            _keep.Click += (s, e) => Nav.Back();
            Controls.Add(_keep);

            _confirm = new HiveButton
            {
                Text = "Clear order",
                Style = HiveStyle.Danger,
                Bounds = new Rectangle(226, ClientSize.Height - 88, 238, 58)
            };
            _confirm.Click += (s, e) =>
            {
                OrderStorage.ClearOrders();
                Nav.Back();
            };
            Controls.Add(_confirm);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Canvas);

            RectangleF card = new RectangleF(Hive.Gutter, 198, Width - Hive.Gutter * 2, 316);
            Hive.Shadow(g, card, Hive.RadiusCard, 3, 22);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Surface);
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.LineSoft, 1);

            RectangleF mark = new RectangleF(Width / 2f - 32, 232, 64, 64);
            Hive.FillHex(g, mark, Hive.HoneyWash);
            Marks.Draw(g, Mark.Bag, RectangleF.Inflate(mark, -19, -19), Hive.Honey, 1.6f);

            Hive.Text(g, "Clear your order?", Hive.Title,
                      new Rectangle(40, 316, Width - 80, 38), Hive.Ink, Hive.Centered);
            Hive.Text(g, "This removes every item from your order.", Hive.Body,
                      new Rectangle(48, 368, Width - 96, 28), Hive.InkSoft, Hive.Centered);
            Hive.Text(g, "You can add items again from the menu.", Hive.Caption,
                      new Rectangle(48, 401, Width - 96, 26), Hive.Muted, Hive.Centered);

            Hive.TextTracked(g, "CHOOSE AN OPTION BELOW", Hive.Overline,
                             new Rectangle(32, 466, Width - 64, 18), Hive.Muted, 1.0f, true);
        }
    }
}

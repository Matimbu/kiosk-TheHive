using System;
using System.Drawing;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    // A kiosk-sized way of saying "that did not work". Stock Windows dialogs put
    // 20px system buttons on a touchscreen a guest is standing in front of, so
    // anything the guest has to read and dismiss comes through here instead.
    public sealed class NoticeForm : Form
    {
        private readonly string _heading;
        private readonly string _message;
        private readonly string _detail;
        private readonly bool _warn;

        public NoticeForm(string title, string heading, string message, string detail, bool warn)
        {
            // Hive.Text translates at draw time, so these stay in English here
            _heading = heading;
            _message = message;
            _detail = detail;
            _warn = warn;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            ClientSize = new Size(Hive.ScreenW, Hive.ScreenH);
            FormBorderStyle = FormBorderStyle.None;
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            ShowInTaskbar = false;

            AppHeader header = new AppHeader { Title = title };
            header.ShowBack((s, e) => Nav.Back());
            Controls.Add(header);

            HiveButton ok = new HiveButton
            {
                Text = "Got it",
                Style = HiveStyle.Primary,
                Bounds = new Rectangle(Hive.Gutter, ClientSize.Height - 88,
                                       ClientSize.Width - Hive.Gutter * 2, 58)
            };
            ok.Click += (s, e) => Nav.Back();
            Controls.Add(ok);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Canvas);

            int cardH = _detail == null ? 234 : 284;
            RectangleF card = new RectangleF(Hive.Gutter, 198, Width - Hive.Gutter * 2, cardH);
            Hive.Shadow(g, card, Hive.RadiusCard, 3, 22);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Surface);
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.LineSoft, 1);

            RectangleF mark = new RectangleF(Width / 2f - 32, 232, 64, 64);
            Hive.FillHex(g, mark, _warn ? Color.FromArgb(0xF7, 0xE7, 0xE5) : Hive.HoneyWash);
            Marks.Draw(g, Mark.Cross, RectangleF.Inflate(mark, -21, -21),
                       _warn ? Hive.Danger : Hive.Honey, 1.8f);

            Hive.Text(g, _heading, Hive.Title,
                      new Rectangle(40, 316, Width - 80, 38), Hive.Ink, Hive.Centered);
            Hive.Text(g, _message, Hive.Body,
                      new Rectangle(44, 358, Width - 88, 46), Hive.InkSoft,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);

            if (_detail != null)
                Hive.Text(g, _detail, Hive.Caption,
                          new Rectangle(44, 406, Width - 88, 46), Hive.Muted,
                          TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);
        }

        // The two shapes the kiosk actually needs.
        public static void Say(string heading, string message)
        {
            Nav.Go(new NoticeForm("Just so you know", heading, message, null, false));
        }

        public static void Problem(string heading, string message, string detail)
        {
            Nav.Go(new NoticeForm("Something went wrong", heading, message, detail, true));
        }
    }
}

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    public partial class Form1 : Form, IPage
    {
        private readonly Timer _startDelay = new Timer { Interval = 180 };
        private readonly HiveButton _languageButton = new HiveButton();
        private bool _starting;

        public Form1()
        {
            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.TealDeep;

            _languageButton.Bounds = new Rectangle(342, 18, 120, 48);
            _languageButton.Style = HiveStyle.Light;
            _languageButton.Tracked = false;
            _languageButton.AccessibleName = "Change language between English and Filipino";
            _languageButton.Click += (s, e) => {
                GuestText.SetFilipino(!GuestText.Filipino);
                UpdatePreferences();
            };
            Controls.Add(_languageButton);
            UpdatePreferences();
            _startDelay.Tick += (s, e) =>
            {
                _startDelay.Stop();
                OrderStorage.ClearOrders();
                Nav.Go(new menuPage());
            };
            Disposed += (s, e) => _startDelay.Dispose();
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            if (_starting) return;
            _starting = true;
            btn_Start.Text = "OPENING MENU...";
            btn_Start.HoldPressed();
            _startDelay.Start();
        }

        public void OnRevealed()
        {
            _startDelay.Stop();
            _starting = false;
            btn_Start.Text = "Start your order";
            btn_Start.ReleasePressed();
            UpdatePreferences();
        }

        private void UpdatePreferences()
        {
            _languageButton.Text = GuestText.Filipino ? "FILIPINO" : "ENGLISH";
            Invalidate(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            using (SolidBrush b = new SolidBrush(Hive.Teal))
                g.FillRectangle(b, ClientRectangle);
            DrawHoneycomb(g);

            Hive.Text(g, GuestText.Filipino ? "WIKA" : "LANGUAGE", Hive.Overline,
                new Rectangle(342, 70, 120, 18), Color.FromArgb(190, 255, 255, 255), Hive.Centered);

            RectangleF plate = new RectangleF(Width / 2f - 68, 156, 136, 136);
            using (GraphicsPath clip = Hive.Hexagon(plate))
            {
                Region saved = g.Clip;
                g.SetClip(clip, CombineMode.Intersect);
                using (SolidBrush b = new SolidBrush(Color.FromArgb(38, 255, 255, 255)))
                    g.FillRectangle(b, plate);

                Image logo = MenuCatalog.Logo;
                if (logo != null) Hive.ImageCover(g, logo, plate, clip);
                g.Clip = saved;
            }

            Hive.TextTracked(g, "THE HIVE CAFE", Hive.Overline,
                             new Rectangle(0, 326, Width, 20), Hive.HoneyLight, 3.6f, true);

            // Sitka descenders need the extra height
            using (Font display = Hive.Sized(Hive.Display, 33f))
                Hive.Text(g, "Good day.", display,
                          new Rectangle(0, 350, Width, 64), Color.White, Hive.Centered);

            // hairline, not another color block
            using (Pen p = new Pen(Color.FromArgb(70, 255, 255, 255), 1))
                g.DrawLine(p, Width / 2 - 26, 428, Width / 2 + 26, 428);

            Hive.Text(g, "Freshly brewed coffee and hot meals,\nmade to order while you wait.", Hive.Body,
                      new Rectangle(60, 448, Width - 120, 52), Color.FromArgb(200, 255, 255, 255),
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);

            Hive.TextTracked(g, "CASH AT COUNTER  ·  PAYMENT DEMOS", Hive.Overline,
                             new Rectangle(0, 644, Width, 20), Color.FromArgb(120, 255, 255, 255), 2.2f, true);
        }

        private void DrawHoneycomb(Graphics g)
        {
            const float radius = 30f;
            float stepX = radius * 1.5f;
            float stepY = (float)(Math.Sqrt(3) * radius);

            using (Pen pen = new Pen(Color.FromArgb(14, 255, 255, 255), 1.2f))
            {
                int col = 0;
                for (float x = -radius; x < Width + radius; x += stepX, col++)
                {
                    float offset = (col % 2 == 0) ? 0 : stepY / 2f;
                    for (float y = -radius + offset; y < Height + radius; y += stepY)
                        g.DrawPolygon(pen, Hexagon(x, y, radius));
                }
            }
        }

        private static PointF[] Hexagon(float cx, float cy, float r)
        {
            PointF[] points = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 180 * (60 * i);
                points[i] = new PointF(cx + r * (float)Math.Cos(angle), cy + r * (float)Math.Sin(angle));
            }
            return points;
        }
    }
}

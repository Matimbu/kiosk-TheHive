using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace kiosk.UI
{
    public static class Hive
    {
        // teal off the logo, honey only for the one action that matters
        public static readonly Color Ink        = Color.FromArgb(0x14, 0x21, 0x22);
        public static readonly Color InkSoft    = Color.FromArgb(0x45, 0x55, 0x56);
        public static readonly Color Muted      = Color.FromArgb(0x8A, 0x93, 0x93);

        public static readonly Color Teal       = Color.FromArgb(0x0B, 0x54, 0x57);
        public static readonly Color TealDeep   = Color.FromArgb(0x07, 0x38, 0x3B);
        public static readonly Color TealDarker = Color.FromArgb(0x04, 0x22, 0x24);
        public static readonly Color TealLight  = Color.FromArgb(0x13, 0x74, 0x78);
        public static readonly Color TealWash   = Color.FromArgb(0xEE, 0xF6, 0xF6);

        public static readonly Color Honey      = Color.FromArgb(0xC8, 0x86, 0x12);
        public static readonly Color HoneyLight = Color.FromArgb(0xE3, 0xA5, 0x2E);
        public static readonly Color HoneyGold  = Color.FromArgb(0xD8, 0x93, 0x1A);
        public static readonly Color HoneyWash  = Color.FromArgb(0xF7, 0xEE, 0xDC);

        public static readonly Color Canvas     = Color.FromArgb(0xF3, 0xF7, 0xF7);
        public static readonly Color Surface    = Color.White;
        public static readonly Color SurfaceAlt = Color.FromArgb(0xE6, 0xEF, 0xEF);
        public static readonly Color Line       = Color.FromArgb(0xCD, 0xDF, 0xDF);
        public static readonly Color LineSoft   = Color.FromArgb(0xE6, 0xEF, 0xEF);

        public static readonly Color Success    = Color.FromArgb(0x1E, 0x7A, 0x50);
        public static readonly Color Danger     = Color.FromArgb(0xAE, 0x3B, 0x33);

        // Modern tactile squircle radii for premium feel
        public const int ScreenW      = 480;
        public const int ScreenH      = 720;
        public const int Gutter       = 16;   // page padding
        public const int Gap          = 12;   // space between siblings
        public const int RadiusCard   = 8;
        public const int RadiusButton = 10;
        public const int RadiusLarge  = 12;   // floating panels, e.g. the cart tray
        public const int RadiusTag    = 4;    // badges - a printed tag, not a pill
        public const int TapTarget    = 52;   // minimum comfortable touch height
        public const float Hairline   = 1f;

        public const string Peso = "₱";

        public static string Money(decimal amount)
        {
            return Peso + amount.ToString("N2");
        }

        public static string MoneyShort(decimal amount)
        {
            return Peso + (amount == decimal.Truncate(amount)
                         ? decimal.Truncate(amount).ToString("0")
                         : amount.ToString("0.00"));
        }

        // Sitka for the cafe name + food, Bahnschrift for prices and labels,
        // Segoe for descriptions
        private static readonly string Banner  = Resolve("Sitka Banner", "Georgia", "Cambria", "Times New Roman");
        private static readonly string SerifUI = Resolve("Sitka Heading", "Sitka Text", "Georgia", "Cambria");
        private static readonly string Grotesk = Resolve("Bahnschrift SemiBold", "Bahnschrift", "Franklin Gothic Medium", "Segoe UI");
        private static readonly string Plain   = Resolve("Segoe UI", "Tahoma");

        public static readonly Font Display  = Make(Banner,  27f, FontStyle.Bold);
        public static readonly Font Title    = Make(SerifUI, 16f, FontStyle.Bold);
        public static readonly Font Heading  = Make(SerifUI, 12.5f, FontStyle.Bold);
        public static readonly Font Serif    = Make(SerifUI, 11f, FontStyle.Bold);
        public static readonly Font Subhead  = Make(Grotesk, 11.5f, FontStyle.Regular);
        public static readonly Font Body     = Make(Plain,   10.5f, FontStyle.Regular);
        public static readonly Font BodyBold = Make(Plain,   10.5f, FontStyle.Bold);
        public static readonly Font Caption  = Make(Plain,   9f, FontStyle.Regular);
        public static readonly Font Overline = Make(Grotesk, 8.5f, FontStyle.Regular);
        public static readonly Font Tab      = Make(Grotesk, 10f, FontStyle.Regular);
        public static readonly Font Price    = Make(Grotesk, 13.5f, FontStyle.Regular);
        public static readonly Font PriceBig = Make(Grotesk, 21f, FontStyle.Regular);
        public static readonly Font Mono     = Make("Consolas", 9.5f, FontStyle.Regular);

        private static string Resolve(params string[] candidates)
        {
            foreach (string name in candidates)
            {
                try
                {
                    using (new FontFamily(name)) return name;
                }
                catch (ArgumentException) { }
            }
            return FontFamily.GenericSansSerif.Name;
        }

        private static Font Make(string family, float size, FontStyle style)
        {
            try
            {
                FontFamily f = new FontFamily(family);
                if (!f.IsStyleAvailable(style))
                {
                    if (f.IsStyleAvailable(FontStyle.Regular)) style = FontStyle.Regular;
                    else if (f.IsStyleAvailable(FontStyle.Bold)) style = FontStyle.Bold;
                    else return new Font(Plain ?? "Segoe UI", size, FontStyle.Regular);
                }
                return new Font(f, size, style);
            }
            catch (ArgumentException)
            {
                return new Font("Segoe UI", size, style);
            }
        }

        public static Font Sized(Font basis, float size)
        {
            return new Font(basis.FontFamily, size, basis.Style);
        }

        public static void Smooth(Graphics g)
        {
            g.SmoothingMode     = SmoothingMode.AntiAlias;
            g.PixelOffsetMode   = PixelOffsetMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
        }

        public static GraphicsPath Rounded(RectangleF r, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            if (r.Width <= 0 || r.Height <= 0) { path.AddRectangle(r); return path; }

            float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
            if (d <= 0.5f) { path.AddRectangle(r); return path; }

            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath RoundedTop(RectangleF r, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float d = Math.Min(radius * 2f, Math.Min(r.Width, r.Height));
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddLine(r.Right, r.Bottom, r.X, r.Bottom);
            path.CloseFigure();
            return path;
        }

        public static GraphicsPath Hexagon(RectangleF r)
        {
            float cx = r.X + r.Width / 2f;
            float cy = r.Y + r.Height / 2f;
            float rx = r.Width / 2f;
            float ry = r.Height / 2f;

            PointF[] points = new PointF[6];
            for (int i = 0; i < 6; i++)
            {
                double angle = Math.PI / 180.0 * (60 * i);
                points[i] = new PointF(cx + rx * (float)Math.Cos(angle),
                                       cy + ry * (float)Math.Sin(angle));
            }

            GraphicsPath path = new GraphicsPath();
            path.AddPolygon(points);
            return path;
        }

        public static void FillHex(Graphics g, RectangleF r, Color color)
        {
            using (GraphicsPath p = Hexagon(r))
            using (SolidBrush b = new SolidBrush(color))
                g.FillPath(b, p);
        }

        public static void Fill(Graphics g, RectangleF r, float radius, Color color)
        {
            using (GraphicsPath p = Hive.Rounded(r, radius))
            using (SolidBrush b = new SolidBrush(color))
                g.FillPath(b, p);
        }

        public static void Stroke(Graphics g, RectangleF r, float radius, Color color, float width)
        {
            RectangleF inner = RectangleF.Inflate(r, -width / 2f, -width / 2f);
            using (GraphicsPath p = Hive.Rounded(inner, radius))
            using (Pen pen = new Pen(color, width))
                g.DrawPath(pen, p);
        }

        public static LinearGradientBrush Gradient(RectangleF r, Color from, Color to, float angle)
        {
            RectangleF safe = RectangleF.Inflate(r, 2, 2);
            LinearGradientBrush brush = new LinearGradientBrush(Rectangle.Round(safe), from, to, angle);
            brush.WrapMode = WrapMode.TileFlipXY;
            return brush;
        }

        public static void FillGradient(Graphics g, RectangleF r, float radius, Color from, Color to, float angle)
        {
            if (r.Width <= 0 || r.Height <= 0) return;
            using (GraphicsPath p = Hive.Rounded(r, radius))
            using (LinearGradientBrush b = Gradient(r, from, to, angle))
                g.FillPath(b, p);
        }

        public static void Shadow(Graphics g, RectangleF r, float radius, int depth, int strength)
        {
            if (depth <= 0 || strength <= 0) return;
            for (int i = depth; i >= 1; i--)
            {
                RectangleF rr = RectangleF.Inflate(r, i * 0.75f, i * 0.75f);
                rr.Offset(0, i * 0.8f);
                int alpha = Math.Max(1, Math.Min(255, strength / (i * 2 + 1)));
                using (GraphicsPath p = Hive.Rounded(rr, radius + i * 0.5f))
                using (SolidBrush b = new SolidBrush(Color.FromArgb(alpha, 10, 34, 36)))
                    g.FillPath(b, p);
            }
        }

        // photos are up to 1200x1200. rescaling on every paint killed scrolling,
        // so cache one bitmap per size
        private static readonly System.Collections.Generic.Dictionary<string, Image> Thumbs =
            new System.Collections.Generic.Dictionary<string, Image>();

        private static Image Scaled(Image source, int width, int height, bool cover)
        {
            if (source == null || width <= 0 || height <= 0) return null;

            string key = source.GetHashCode() + "|" + width + "x" + height + "|" + (cover ? "c" : "f");
            Image cached;
            if (Thumbs.TryGetValue(key, out cached)) return cached;

            Bitmap thumb = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            using (Graphics g = Graphics.FromImage(thumb))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode   = PixelOffsetMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;

                float scale = cover
                            ? Math.Max((float)width / source.Width, (float)height / source.Height)
                            : Math.Min((float)width / source.Width, (float)height / source.Height);

                float w = source.Width * scale;
                float h = source.Height * scale;
                g.DrawImage(source, (width - w) / 2f, (height - h) / 2f, w, h);
            }

            Thumbs[key] = thumb;
            return thumb;
        }

        public static void ImageCover(Graphics g, Image img, RectangleF dest, GraphicsPath clip)
        {
            if (img == null || dest.Width <= 0 || dest.Height <= 0) return;

            Rectangle target = Rectangle.Round(dest);
            Image thumb = Scaled(img, target.Width, target.Height, true);
            if (thumb == null) return;

            Region saved = g.Clip;
            if (clip != null) g.SetClip(clip, CombineMode.Intersect);
            g.DrawImageUnscaled(thumb, target.X, target.Y);
            g.Clip = saved;
        }

        public static void ImageContain(Graphics g, Image img, RectangleF dest, float inset)
        {
            if (img == null || dest.Width <= 0 || dest.Height <= 0) return;

            Rectangle area = Rectangle.Round(RectangleF.Inflate(dest, -inset, -inset));
            Image thumb = Scaled(img, area.Width, area.Height, false);
            if (thumb == null) return;

            g.DrawImageUnscaled(thumb, area.X, area.Y);
        }

        private static readonly System.Collections.Generic.Dictionary<Image, Bitmap> CroppedPhotos =
            new System.Collections.Generic.Dictionary<Image, Bitmap>();

        public static void ImageContainProduct(Graphics g, Image img, RectangleF dest, float inset)
        {
            if (img == null) return;
            Bitmap cropped;
            if (!CroppedPhotos.TryGetValue(img, out cropped))
            {
                using (var sample = new Bitmap(img, new Size(240, 240)))
                {
                    int minX = 240, minY = 240, maxX = -1, maxY = -1;
                    for (int y = 0; y < 240; y += 2)
                    for (int x = 0; x < 240; x += 2)
                    {
                        Color pixel = sample.GetPixel(x, y);
                        if (pixel.A < 30 || (pixel.R > 244 && pixel.G > 244 && pixel.B > 244)) continue;
                        minX = Math.Min(minX, x); minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x); maxY = Math.Max(maxY, y);
                    }
                    if (maxX < minX) { minX = 0; minY = 0; maxX = 239; maxY = 239; }
                    int pad = 5;
                    Rectangle crop = Rectangle.FromLTRB(
                        Math.Max(0, (minX - pad) * img.Width / 240),
                        Math.Max(0, (minY - pad) * img.Height / 240),
                        Math.Min(img.Width, (maxX + pad + 1) * img.Width / 240),
                        Math.Min(img.Height, (maxY + pad + 1) * img.Height / 240));
                    if (crop.Width < 1 || crop.Height < 1) crop = new Rectangle(0, 0, img.Width, img.Height);
                    cropped = new Bitmap(crop.Width, crop.Height);
                    using (Graphics cg = Graphics.FromImage(cropped))
                        cg.DrawImage(img, new Rectangle(0, 0, crop.Width, crop.Height), crop, GraphicsUnit.Pixel);
                }
                CroppedPhotos.Add(img, cropped);
            }
            ImageContain(g, cropped, dest, inset);
        }

        public static Color Mix(Color a, Color b, float t)
        {
            t = Math.Max(0f, Math.Min(1f, t));
            return Color.FromArgb(
                (int)(a.A + (b.A - a.A) * t),
                (int)(a.R + (b.R - a.R) * t),
                (int)(a.G + (b.G - a.G) * t),
                (int)(a.B + (b.B - a.B) * t));
        }

        public static Color Lighten(Color c, float t) { return Mix(c, Color.White, t); }
        public static Color Darken(Color c, float t)  { return Mix(c, Color.Black, t); }

        public static void Text(Graphics g, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            text = GuestText.T(text) ?? string.Empty;
            TextRenderer.DrawText(g, text, font, bounds, color, flags | TextFormatFlags.NoPrefix);
        }

        public static void TextTracked(Graphics g, string text, Font font, Rectangle bounds,
                                       Color color, float tracking, bool center)
        {
            if (string.IsNullOrEmpty(text)) return;
            text = TrackedLabel(text);

            const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

            float[] widths;
            float total = TrackedWidth(g, text, font, tracking, out widths);

            if (total > bounds.Width)
            {
                TextRenderer.DrawText(g, text, font, bounds, color,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix |
                    (center ? TextFormatFlags.HorizontalCenter : TextFormatFlags.Left));
                return;
            }

            float x = center ? bounds.X + (bounds.Width - total) / 2f : bounds.X;
            int y = bounds.Y + (bounds.Height - font.Height) / 2;

            for (int i = 0; i < text.Length; i++)
            {
                TextRenderer.DrawText(g, text[i].ToString(), font,
                                      new Point((int)Math.Round(x), y), color, flags);
                x += widths[i] + tracking;
            }
        }
        // Callers pass tracked labels in caps, but the dictionary matches without
        // regard to case and hands back the Filipino in mixed case. Letter-spaced
        // lowercase looks broken ("Mga p a borito"), so keep the caps the caller
        // asked for.
        public static string TrackedLabel(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            bool caps = text == text.ToUpperInvariant();
            text = GuestText.T(text);
            return caps ? text.ToUpperInvariant() : text;
        }

        // The width TextTracked will actually use. Shared so anything checking
        // whether a label fits measures it the same way it gets drawn.
        public static float TrackedWidth(Graphics g, string label, Font font, float tracking)
        {
            float[] unused;
            return TrackedWidth(g, label, font, tracking, out unused);
        }

        private static float TrackedWidth(Graphics g, string label, Font font, float tracking, out float[] widths)
        {
            const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            float total = 0;
            widths = new float[label.Length];
            for (int i = 0; i < label.Length; i++)
            {
                widths[i] = TextRenderer.MeasureText(g, label[i].ToString(), font, Size.Empty, flags).Width;
                total += widths[i] + (i < label.Length - 1 ? tracking : 0);
            }
            return total;
        }

        public const TextFormatFlags Centered =
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
        public const TextFormatFlags LeftMid =
            TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;
        public const TextFormatFlags RightMid =
            TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        public static void Monogram(Graphics g, string name, RectangleF r, GraphicsPath clip, string caption = null)
        {
            Region saved = g.Clip;
            if (clip != null) g.SetClip(clip, CombineMode.Intersect);

            // keep it quiet, it should not fight the real photos
            using (SolidBrush b = new SolidBrush(SurfaceAlt))
                g.FillRectangle(b, r);

            bool showCaption = !string.IsNullOrEmpty(caption) && r.Height >= 70;
            RectangleF badge = showCaption
                              ? new RectangleF(r.X, r.Y, r.Width, r.Height - 22)
                              : r;

            float side = Math.Min(badge.Width, badge.Height) * 0.6f;
            RectangleF hex = new RectangleF(badge.X + (badge.Width - side) / 2f,
                                            badge.Y + (badge.Height - side) / 2f, side, side);
            using (GraphicsPath p = Hexagon(hex))
            using (Pen pen = new Pen(Mix(Line, SurfaceAlt, 0.35f), 1.4f))
            {
                pen.LineJoin = LineJoin.Round;
                g.DrawPath(pen, p);
            }

            using (Font f = Sized(Display, Math.Max(10f, badge.Height * 0.24f)))
                Text(g, Initials(name), f, Rectangle.Round(badge), Mix(Muted, Line, 0.35f), Centered);

            if (showCaption)
                TextTracked(g, caption.ToUpperInvariant(), Overline,
                           new Rectangle((int)r.X, (int)r.Bottom - 20, (int)r.Width, 16),
                           Mix(Muted, SurfaceAlt, 0.15f), 1.1f, true);

            g.Clip = saved;
        }

        private static string Initials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "TH";
            string[] parts = name.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return parts[0].Substring(0, 1).ToUpperInvariant();
            return (parts[0].Substring(0, 1) + parts[1].Substring(0, 1)).ToUpperInvariant();
        }
    }

    public enum Mark { ArrowLeft, Cross, Check, Plus, Bag, Chevron, Cash, Card, Wallet }

    public static class Marks
    {
        public static void Draw(Graphics g, Mark mark, RectangleF box, Color color, float weight)
        {
            float cx = box.X + box.Width / 2f;
            float cy = box.Y + box.Height / 2f;
            float r = Math.Min(box.Width, box.Height) / 2f;

            using (Pen pen = new Pen(color, weight))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                pen.LineJoin = LineJoin.Round;

                switch (mark)
                {
                    case Mark.ArrowLeft:
                        g.DrawLine(pen, cx - r * 0.8f, cy, cx + r * 0.8f, cy);
                        g.DrawLines(pen, new[]
                        {
                            new PointF(cx - r * 0.2f, cy - r * 0.6f),
                            new PointF(cx - r * 0.8f, cy),
                            new PointF(cx - r * 0.2f, cy + r * 0.6f)
                        });
                        break;

                    case Mark.Cross:
                        g.DrawLine(pen, cx - r * 0.55f, cy - r * 0.55f, cx + r * 0.55f, cy + r * 0.55f);
                        g.DrawLine(pen, cx + r * 0.55f, cy - r * 0.55f, cx - r * 0.55f, cy + r * 0.55f);
                        break;

                    case Mark.Check:
                        g.DrawLines(pen, new[]
                        {
                            new PointF(cx - r * 0.62f, cy),
                            new PointF(cx - r * 0.16f, cy + r * 0.46f),
                            new PointF(cx + r * 0.64f, cy - r * 0.44f)
                        });
                        break;

                    case Mark.Plus:
                        g.DrawLine(pen, cx - r * 0.5f, cy, cx + r * 0.5f, cy);
                        g.DrawLine(pen, cx, cy - r * 0.5f, cx, cy + r * 0.5f);
                        break;

                    case Mark.Chevron:
                        g.DrawLines(pen, new[]
                        {
                            new PointF(cx - r * 0.3f, cy - r * 0.55f),
                            new PointF(cx + r * 0.3f, cy),
                            new PointF(cx - r * 0.3f, cy + r * 0.55f)
                        });
                        break;

                    case Mark.Bag:
                        {
                            RectangleF body = new RectangleF(cx - r * 0.62f, cy - r * 0.2f, r * 1.24f, r * 1.1f);
                            using (GraphicsPath p = Hive.Rounded(body, 2f)) g.DrawPath(pen, p);
                            g.DrawArc(pen, cx - r * 0.32f, cy - r * 0.78f, r * 0.64f, r * 0.72f, 180, 180);
                        }
                        break;

                    case Mark.Cash:
                        {
                            RectangleF note = new RectangleF(cx - r * 0.75f, cy - r * 0.45f, r * 1.5f, r * 0.9f);
                            using (GraphicsPath p = Hive.Rounded(note, 2f)) g.DrawPath(pen, p);
                            g.DrawEllipse(pen, cx - r * 0.2f, cy - r * 0.2f, r * 0.4f, r * 0.4f);
                        }
                        break;

                    case Mark.Card:
                        {
                            RectangleF plate = new RectangleF(cx - r * 0.78f, cy - r * 0.52f, r * 1.56f, r * 1.04f);
                            using (GraphicsPath p = Hive.Rounded(plate, 2f)) g.DrawPath(pen, p);
                            g.DrawLine(pen, plate.X, cy - r * 0.16f, plate.Right, cy - r * 0.16f);
                        }
                        break;

                    case Mark.Wallet:
                        {
                            // three corner finders, like a QR code
                            float s = r * 0.44f;
                            float o = r * 0.62f;
                            g.DrawRectangle(pen, cx - o, cy - o, s, s);
                            g.DrawRectangle(pen, cx + o - s, cy - o, s, s);
                            g.DrawRectangle(pen, cx - o, cy + o - s, s, s);
                            g.DrawLine(pen, cx + o - s * 0.4f, cy + o - s * 0.9f, cx + o - s * 0.4f, cy + o);
                        }
                        break;
                }
            }
        }
    }

    public sealed class Anim : IDisposable
    {
        public const int FrameMs = 15;

        private const float ReferenceFps = 60f;

        private static readonly Stopwatch Clock = Stopwatch.StartNew();

        private readonly Control _owner;
        private readonly Timer _timer;
        private readonly float _ratePerSecond;
        private float _value;
        private float _target;
        private long _lastMs;

        public Anim(Control owner, float step = 0.22f, int intervalMs = FrameMs)
        {
            _owner = owner;
            _ratePerSecond = step * ReferenceFps;
            _timer = new Timer();
            _timer.Interval = intervalMs;
            _timer.Tick += Tick;
        }

        public float Value { get { return _value; } }

        public void To(float target)
        {
            _target = target;
            if (Math.Abs(_target - _value) < 0.001f) return;
            if (_timer.Enabled) return;

            _lastMs = Clock.ElapsedMilliseconds;
            _timer.Start();
        }

        public void Set(float value)
        {
            _value = _target = value;
            _timer.Stop();
            _owner.Invalidate();
        }

        private void Tick(object sender, EventArgs e)
        {
            long now = Clock.ElapsedMilliseconds;
            float dt = (now - _lastMs) / 1000f;
            _lastMs = now;

            if (dt <= 0f) dt = FrameMs / 1000f;
            else if (dt > 0.1f) dt = 0.1f;     // don't leap after a stall

            float step = _ratePerSecond * dt;
            float delta = _target - _value;

            if (Math.Abs(delta) <= step)
            {
                _value = _target;
                _timer.Stop();
            }
            else
            {
                _value += Math.Sign(delta) * step;
            }

            _owner.Invalidate();
        }

        public void Dispose()
        {
            _timer.Tick -= Tick;
            _timer.Dispose();
        }
    }

}

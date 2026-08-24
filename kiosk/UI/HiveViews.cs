using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace kiosk.UI
{

    public class AppHeader : Panel
    {
        private Image _mark;
        private string _title = "The Hive Cafe";
        private string _subtitle;
        private HiveButton _back;
        private Control _accessory;

        public AppHeader()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
            Height = 92;
            Dock = DockStyle.Top;
        }

        public Image Logo      { get { return _mark; }     set { _mark = value; Invalidate(); } }
        public string Title    { get { return _title; }    set { _title = value; Invalidate(); } }
        public string Subtitle { get { return _subtitle; } set { _subtitle = value; Invalidate(); } }

        public void ShowBack(EventHandler onBack)
        {
            if (_back == null)
            {
                _back = new HiveButton();
                _back.Icon = Mark.ArrowLeft;
                _back.Style = HiveStyle.Ghost;
                _back.TextColor = Color.White;
                _back.Size = new Size(42, 42);
                _back.Location = new Point(Hive.Gutter - 6, (Height - 42) / 2);
                Controls.Add(_back);
            }
            _back.Click += onBack;
            Invalidate();
        }

        public void SetAccessory(Control control)
        {
            if (_accessory != null) Controls.Remove(_accessory);
            _accessory = control;
            if (control == null) return;
            control.Location = new Point(Width - control.Width - Hive.Gutter, (Height - control.Height) / 2);
            control.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Controls.Add(control);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            using (SolidBrush b = new SolidBrush(Hive.Teal))
                g.FillRectangle(b, ClientRectangle);

            // honey line along the bottom
            using (SolidBrush b = new SolidBrush(Hive.Honey))
                g.FillRectangle(b, 0, Height - 3, Width, 3);

            int left = Hive.Gutter;

            if (_back != null)
            {
                left = _back.Right + 6;
            }
            else if (_mark != null)
            {
                RectangleF logo = new RectangleF(left, (Height - 48) / 2f - 1, 48, 48);
                using (GraphicsPath clip = Hive.Hexagon(logo))
                {
                    Region saved = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    Hive.ImageCover(g, _mark, logo, clip);
                    g.Clip = saved;
                }
                left = (int)logo.Right + 12;
            }

            int right = _accessory != null ? _accessory.Left - 10 : Width - Hive.Gutter;
            int textWidth = Math.Max(40, right - left);

            if (string.IsNullOrEmpty(_subtitle))
            {
                Hive.Text(g, _title, Hive.Title,
                          new Rectangle(left, 0, textWidth, Height - 3), Color.White, Hive.LeftMid);
            }
            else
            {
                Hive.Text(g, _title, Hive.Title,
                          new Rectangle(left, 13, textWidth, 34), Color.White, Hive.LeftMid);
                Hive.TextTracked(g, _subtitle.ToUpperInvariant(), Hive.Overline,
                                 new Rectangle(left, 47, textWidth, 18),
                                 Color.FromArgb(165, 255, 255, 255), 1.3f, false);
            }

            base.OnPaint(e);
        }
    }

    public class CartBar : Panel
    {
        private readonly HiveButton _action;
        private int _count;
        private decimal _total;

        public CartBar()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
            Height = 88;
            Dock = DockStyle.Bottom;
            BackColor = Hive.Surface;

            _action = new HiveButton();
            _action.Text = "View order";
            _action.Style = HiveStyle.Accent;
            _action.Size = new Size(164, 54);
            _action.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Controls.Add(_action);
        }

        public HiveButton Action { get { return _action; } }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            if (_action == null) return;   // fires while the constructor is still running
            _action.Location = new Point(Width - _action.Width - Hive.Gutter, (Height - _action.Height) / 2);
        }

        public void Update(int count, decimal total)
        {
            _count = count;
            _total = total;
            _action.Enabled = count > 0;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Surface);

            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, 0, 0, Width, 0);

            string label = _count == 0 ? "Your order is empty"
                         : _count == 1 ? "1 item"
                         : _count + " items";

            Hive.TextTracked(g, label.ToUpperInvariant(), Hive.Overline,
                             new Rectangle(Hive.Gutter, 22, 220, 18), Hive.Muted, 1.2f, false);
            Hive.Text(g, Hive.Money(_total), Hive.PriceBig,
                      new Rectangle(Hive.Gutter, 40, 220, 30),
                      _count == 0 ? Hive.Muted : Hive.Ink, Hive.LeftMid);

            base.OnPaint(e);
        }
    }

    public class CartPill : HiveControl
    {
        private readonly Anim _hover;
        private int _count;

        public CartPill()
        {
            _hover = new Anim(this, 0.3f);
            Cursor = Cursors.Hand;
            Size = new Size(84, 42);
            Font = Hive.Price;
        }

        public int Count { get { return _count; } set { _count = value; Invalidate(); } }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            RectangleF body = new RectangleF(0, 0, Width - 1, Height - 1);
            Hive.Fill(g, body, Hive.RadiusButton, Color.FromArgb(28 + (int)(34 * _hover.Value), 255, 255, 255));
            Hive.Stroke(g, body, Hive.RadiusButton, Color.FromArgb(90, 255, 255, 255), 1.2f);

            Marks.Draw(g, Mark.Bag, new RectangleF(12, Height / 2f - 11, 22, 22), Color.White, 1.6f);
            Hive.Text(g, _count.ToString(), Font, new Rectangle(38, 0, Width - 48, Height), Color.White, Hive.Centered);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hover.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ProductTile : HiveControl
    {
        private readonly Anim _hover;
        private MenuProduct _item;
        private Point _pressedAt;
        private bool _moved;

        public event EventHandler<ProductEventArgs> Chosen;

        public ProductTile(MenuProduct item)
        {
            _hover = new Anim(this, 0.24f);
            _item = item;
            Cursor = Cursors.Hand;
            BackColor = Hive.Canvas;
            Size = new Size(226, 214);
        }

        public MenuProduct Item { get { return _item; } }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _pressedAt = e.Location;
            _moved = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.Button == MouseButtons.Left && Math.Abs(e.Y - _pressedAt.Y) > 5) _moved = true;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (_moved) return;   // the guest was scrolling, not choosing
            EventHandler<ProductEventArgs> handler = Chosen;
            if (handler != null) handler(this, new ProductEventArgs(_item));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            float lift = _hover.Value;
            RectangleF card = new RectangleF(2, 2, Width - 5, Height - 8);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Surface);

            RectangleF photo = new RectangleF(card.X, card.Y, card.Width, 118);
            using (GraphicsPath clip = Hive.RoundedTop(photo, Hive.RadiusCard))
            {
                Image img = _item.Image;
                if (img == null)
                {
                    Hive.Monogram(g, _item.Name, photo, clip, "Photo coming soon");
                }
                else if (_item.ShowWhole)
                {
                    Region saved = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    using (SolidBrush b = new SolidBrush(Color.White)) g.FillRectangle(b, photo);
                    Hive.ImageContain(g, img, photo, 4f);
                    g.Clip = saved;
                }
                else
                {
                    Hive.ImageCover(g, img, photo, clip);
                }
            }

            if (_item.Badge != null)
            {
                string badgeText = _item.Badge.ToUpperInvariant();
                Size size = TextRenderer.MeasureText(badgeText, Hive.Overline);
                RectangleF badge = new RectangleF(photo.X, photo.Y + 12, size.Width + 22, 21);
                using (SolidBrush b = new SolidBrush(Hive.Honey)) g.FillRectangle(b, badge);
                Hive.TextTracked(g, badgeText, Hive.Overline, Rectangle.Round(badge), Color.White, 1.1f, true);
            }

            Rectangle name = new Rectangle((int)card.X + 12, (int)photo.Bottom + 10, (int)card.Width - 24, 38);
            Hive.Text(g, _item.Name, Hive.Serif, name, Hive.Ink,
                      TextFormatFlags.WordBreak | TextFormatFlags.Top | TextFormatFlags.EndEllipsis);

            using (Pen p = new Pen(Hive.LineSoft, 1))
                g.DrawLine(p, card.X + 12, card.Bottom - 38, card.Right - 12, card.Bottom - 38);

            Rectangle price = new Rectangle((int)card.X + 12, (int)card.Bottom - 34, (int)card.Width - 58, 28);
            Hive.Text(g, _item.PriceLabel, Hive.Price, price, Hive.Ink, Hive.LeftMid);

            RectangleF hex = new RectangleF(card.Right - 40, card.Bottom - 33, 26, 26);
            Hive.FillHex(g, hex, Hive.Mix(Hive.HoneyWash, Hive.Honey, lift));
            Marks.Draw(g, Mark.Plus, hex, lift > 0.5f ? Color.White : Hive.Honey, 1.8f);

            Hive.Stroke(g, card, Hive.RadiusCard, Hive.Mix(Hive.Line, Hive.Teal, lift * 0.8f), 1.2f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hover.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ProductEventArgs : EventArgs
    {
        public ProductEventArgs(MenuProduct item) { Item = item; }
        public MenuProduct Item { get; private set; }
    }

    public class BufferedPanel : Panel
    {
        public BufferedPanel()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
        }
    }

    public class ScrollHost : Panel
    {
        private readonly BufferedPanel _content;
        private bool _dragging;
        private int _originY;
        private int _contentTop;

        public ScrollHost()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Canvas;

            // opaque on purpose. transparent makes every child repaint through the
            // parent and the text tears while scrolling
            _content = new BufferedPanel();
            _content.BackColor = Hive.Canvas;
            _content.Location = Point.Empty;
            Controls.Add(_content);

            MouseWheel += (s, e) => ScrollBy(e.Delta > 0 ? 70 : -70);
            MouseEnter += (s, e) => { if (CanFocus) Focus(); };
            Hook(this);
        }

        public Panel Content { get { return _content; } }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);

            // empty (designer case) - let it fill so the preview has somewhere to draw
            if (_content != null && _content.Controls.Count == 0)
                _content.Bounds = new Rectangle(0, 0, Width, Height);
        }

        public void Measure(int padding)
        {
            int bottom = 0;
            foreach (Control c in _content.Controls) bottom = Math.Max(bottom, c.Bottom);
            _content.Width = Width;
            _content.Height = bottom + padding;
            _content.Top = 0;
            Invalidate();
        }

        public void Hook(Control c)
        {
            c.MouseDown += (s, e) => { _dragging = true; _originY = Cursor.Position.Y; _contentTop = _content.Top; };
            c.MouseMove += (s, e) =>
            {
                if (!_dragging) return;
                int delta = Cursor.Position.Y - _originY;
                if (Math.Abs(delta) > 4) SetTop(_contentTop + delta);
            };
            c.MouseUp += (s, e) => { _dragging = false; SetTop(_content.Top); };
            c.MouseWheel += (s, e) => ScrollBy(e.Delta > 0 ? 70 : -70);
        }

        private void ScrollBy(int delta) { SetTop(_content.Top + delta); }

        private void SetTop(int top)
        {
            int min = Math.Min(0, Height - _content.Height);
            _content.Top = Math.Max(min, Math.Min(0, top));
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_content.Height <= Height) return;

            Graphics g = e.Graphics;
            Hive.Smooth(g);

            float visible = (float)Height / _content.Height;
            float thumb = Math.Max(40, Height * visible);
            float travel = Height - thumb;
            float progress = _content.Height == Height ? 0 : -_content.Top / (float)(_content.Height - Height);

            RectangleF bar = new RectangleF(Width - 7, 4 + travel * progress, 4, thumb - 8);
            Hive.Fill(g, bar, 2, Color.FromArgb(60, Hive.Teal));
        }
    }

    public class EmptyState : HiveControl
    {
        public EmptyState(string title, string body)
        {
            Title = title;
            Body = body;
        }

        public string Title { get; set; }
        public string Body { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            int mid = Height / 2;
            RectangleF hex = new RectangleF(Width / 2f - 36, mid - 96, 72, 72);
            Hive.FillHex(g, hex, Hive.SurfaceAlt);
            using (GraphicsPath p = Hive.Hexagon(RectangleF.Inflate(hex, -18, -18)))
            using (Pen pen = new Pen(Hive.Muted, 1.6f))
            {
                pen.LineJoin = LineJoin.Round;
                g.DrawPath(pen, p);
            }

            Hive.Text(g, Title, Hive.Title, new Rectangle(24, mid - 16, Width - 48, 34), Hive.Ink, Hive.Centered);
            Hive.Text(g, Body, Hive.Body, new Rectangle(32, mid + 16, Width - 64, 44), Hive.Muted,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak | TextFormatFlags.Top);
        }
    }

    public class OrderRow : HiveControl
    {
        private readonly Anim _hover;
        private readonly Order _order;

        public event EventHandler<OrderEventArgs> Edit;

        public OrderRow(Order order)
        {
            _hover = new Anim(this, 0.3f);
            _order = order;
            Cursor = Cursors.Hand;
            BackColor = Hive.Canvas;
            Height = 88;
        }

        public Order Order { get { return _order; } }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            EventHandler<OrderEventArgs> handler = Edit;
            if (handler != null) handler(this, new OrderEventArgs(_order));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            RectangleF card = new RectangleF(0, 0, Width - 1, Height - 9);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Mix(Hive.Surface, Hive.HoneyWash, _hover.Value * 0.6f));
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.Mix(Hive.Line, Hive.Teal, _hover.Value * 0.6f), 1.2f);

            RectangleF photo = new RectangleF(card.X + 11, card.Y + 11, 57, 57);
            using (GraphicsPath clip = Hive.Rounded(photo, 2))
            {
                MenuProduct product = MenuCatalog.Find(_order.Product);
                Image img = product == null ? null : product.Image;

                if (img == null)
                {
                    Hive.Monogram(g, _order.Product, photo, clip);
                }
                else if (product.ShowWhole)
                {
                    Region saved = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    using (SolidBrush b = new SolidBrush(Color.White)) g.FillRectangle(b, photo);
                    Hive.ImageContain(g, img, photo, 2f);
                    g.Clip = saved;
                }
                else
                {
                    Hive.ImageCover(g, img, photo, clip);
                }
            }

            int left = (int)photo.Right + 12;
            int width = Width - left - 96;

            Hive.Text(g, _order.Product, Hive.Serif,
                      new Rectangle(left, (int)card.Y + 16, width, 20), Hive.Ink, Hive.LeftMid);

            Hive.Text(g, OrderText.Describe(_order) + "   ·   " + Hive.Money(_order.Price) + " each", Hive.Caption,
                      new Rectangle(left, (int)card.Y + 38, width, 18), Hive.Muted, Hive.LeftMid);

            RectangleF qty = new RectangleF(left, card.Bottom - 27, 54, 20);
            Hive.Fill(g, qty, 2, Hive.SurfaceAlt);
            Hive.TextTracked(g, "QTY " + _order.Quantity, Hive.Overline, Rectangle.Round(qty), Hive.InkSoft, 0.9f, true);

            Hive.Text(g, Hive.Money(_order.Total), Hive.Price,
                      new Rectangle(Width - 96, 0, 84, Height - 8), Hive.Ink, Hive.RightMid);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hover.Dispose();
            base.Dispose(disposing);
        }
    }

    public static class OrderText
    {
        public static string Describe(Order order)
        {
            List<string> parts = new List<string>();

            string size = MenuCatalog.SizeLabel(order.Product, order.Size);
            if (!string.IsNullOrWhiteSpace(size)) parts.Add(size);
            if (!string.IsNullOrWhiteSpace(order.Temperature)) parts.Add(order.Temperature);

            return parts.Count == 0 ? "Regular" : string.Join(" / ", parts.ToArray());
        }
    }

    public class OrderEventArgs : EventArgs
    {
        public OrderEventArgs(Order order) { Order = order; }
        public Order Order { get; private set; }
    }

    public static class Summary
    {
        public static void Row(Graphics g, Rectangle bounds, string label, string amount, bool emphasise)
        {
            Font labelFont  = emphasise ? Hive.Subhead : Hive.Body;
            Font amountFont = emphasise ? Hive.PriceBig : Hive.BodyBold;
            Color color     = emphasise ? Hive.Ink : Hive.InkSoft;

            Hive.Text(g, label, labelFont, bounds, color, Hive.LeftMid);
            Hive.Text(g, amount, amountFont, bounds, emphasise ? Hive.Teal : Hive.Ink, Hive.RightMid);
        }
    }
}

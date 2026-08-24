using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace kiosk.UI
{
    public class ProductSheet : Form
    {
        private const int SheetW = Hive.ScreenW;

        private readonly MenuProduct _product;
        private readonly Segmented _size;
        private readonly Segmented _temp;
        private readonly Stepper _qty;
        private readonly HiveButton _add;
        private readonly HiveButton _close;

        private int _heroHeight = 268;

        public ProductSheet(string productName) : this(Resolve(productName)) { }

        private static MenuProduct Resolve(string name)
        {
            MenuProduct found = MenuCatalog.Find(name);
            if (found != null) return found;

            return new MenuProduct
            {
                Name = name,
                Description = "This item is not on the menu yet.",
                BasePrice = 0m
            };
        }

        public ProductSheet(MenuProduct product)
        {
            _product = product;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Hive.Surface;
            Font = Hive.Body;
            ShowInTaskbar = false;

            // fewer options = more room for the photo
            int optionRows = (_product.HasSizes ? 1 : 0) + (_product.HasTemperature ? 1 : 0);
            _heroHeight += (2 - optionRows) * 46;

            int y = _heroHeight + 74;   // name + description sit here, painted directly

            if (_product.HasSizes)
            {
                y += 22;
                _size = new Segmented();
                _size.Options = Array.ConvertAll(_product.Sizes, s => s.Label);
                _size.Bounds = new Rectangle(Hive.Gutter + 4, y, SheetW - (Hive.Gutter + 4) * 2, Hive.TapTarget);
                _size.SelectionChanged += (s, e) => Refresh();
                Controls.Add(_size);
                y += Hive.TapTarget + 16;
            }

            if (_product.HasTemperature)
            {
                y += 22;
                _temp = new Segmented();
                _temp.Options = new[] { "Hot", "Iced" };
                _temp.Bounds = new Rectangle(Hive.Gutter + 4, y, SheetW - (Hive.Gutter + 4) * 2, Hive.TapTarget);
                Controls.Add(_temp);
                y += Hive.TapTarget + 16;
            }

            y += 22;
            _qty = new Stepper();
            _qty.Bounds = new Rectangle(Hive.Gutter + 4, y, 170, Hive.TapTarget);
            _qty.ValueChanged += (s, e) => Refresh();
            Controls.Add(_qty);
            y += Hive.TapTarget + 20;

            int footerTop = Hive.ScreenH - 98;
            _add = new HiveButton();
            _add.Text = "Add to order";
            _add.Style = HiveStyle.Accent;
            _add.Size = new Size(212, 58);
            _add.Location = new Point(SheetW - 212 - Hive.Gutter - 4, footerTop + 18);
            _add.Click += AddToOrder;
            Controls.Add(_add);

            _close = new HiveButton();
            _close.Icon = Mark.Cross;
            _close.Style = HiveStyle.Light;
            _close.Size = new Size(40, 40);
            _close.Location = new Point(SheetW - 40 - 14, 14);
            _close.Click += (s, e) => Nav.Back();
            Controls.Add(_close);

            ClientSize = new Size(SheetW, Hive.ScreenH);

            // preselect so Add to order works in one tap
            if (_size != null) _size.SelectedIndex = 0;
            if (_temp != null) _temp.SelectedIndex = 0;
        }

        private decimal UnitPrice
        {
            get
            {
                if (!_product.HasSizes) return _product.BasePrice;
                int i = _size == null || _size.SelectedIndex < 0 ? 0 : _size.SelectedIndex;
                return _product.Sizes[i].Price;
            }
        }

        private decimal Total { get { return UnitPrice * _qty.Value; } }

        private void AddToOrder(object sender, EventArgs e)
        {
            string sizeCode = "";
            if (_product.HasSizes)
            {
                int i = _size.SelectedIndex < 0 ? 0 : _size.SelectedIndex;
                sizeCode = _product.Sizes[i].Code;
            }

            OrderStorage.AddOrder(new Order
            {
                Product = _product.Name,
                Size = sizeCode,
                Temperature = _temp != null ? (_temp.SelectedOption ?? "Hot") : "",
                Quantity = _qty.Value,
                Price = UnitPrice,
                ImagePath = _product.ImageKey
            });

            Nav.Back();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Surface);

            RectangleF hero = new RectangleF(0, 0, Width, _heroHeight);
            using (GraphicsPath clip = Hive.Rounded(hero, 0))
            {
                if (_product.Image != null)
                {
                    // fit the whole thing, dont crop
                    Region saved = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    using (SolidBrush wash = new SolidBrush(Color.White))
                        g.FillRectangle(wash, hero);
                    Hive.ImageContain(g, _product.Image, hero, 12f);
                    g.Clip = saved;
                }
                else
                {
                    Hive.Monogram(g, _product.Name, hero, clip, "Photo coming soon");
                }
            }

            if (_product.Badge != null)
            {
                string badgeText = _product.Badge.ToUpperInvariant();
                Size size = TextRenderer.MeasureText(badgeText, Hive.Overline);
                RectangleF badge = new RectangleF(0, 18, size.Width + 26, 24);
                using (SolidBrush b = new SolidBrush(Hive.Honey)) g.FillRectangle(b, badge);
                Hive.TextTracked(g, badgeText, Hive.Overline, Rectangle.Round(badge), Color.White, 1.1f, true);
            }

            int left = Hive.Gutter + 4;
            int width = Width - left * 2;

            Hive.Text(g, _product.Name, Hive.Title,
                      new Rectangle(left, _heroHeight + 10, width, 34), Hive.Ink, Hive.LeftMid);

            Hive.Text(g, _product.Description, Hive.Body,
                      new Rectangle(left, _heroHeight + 42, width, 36), Hive.Muted,
                      TextFormatFlags.WordBreak | TextFormatFlags.Top);

            if (_size != null) SectionLabel(g, "SIZE", _size.Top);
            if (_temp != null) SectionLabel(g, "SERVED", _temp.Top);
            SectionLabel(g, "QUANTITY", _qty.Top);

            int footerTop = _add.Top - 18;
            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, Hive.Gutter, footerTop, Width - Hive.Gutter, footerTop);

            Hive.Text(g, "Total", Hive.Caption,
                      new Rectangle(left, footerTop + 18, 140, 18), Hive.Muted, Hive.LeftMid);
            Hive.Text(g, Hive.Money(Total), Hive.PriceBig,
                      new Rectangle(left, footerTop + 36, 160, 30), Hive.Ink, Hive.LeftMid);
        }

        private void SectionLabel(Graphics g, string text, int controlTop)
        {
            Hive.TextTracked(g, text, Hive.Overline,
                             new Rectangle(Hive.Gutter + 4, controlTop - 22, 200, 18), Hive.Muted, 1.4f, false);
        }
    }
}

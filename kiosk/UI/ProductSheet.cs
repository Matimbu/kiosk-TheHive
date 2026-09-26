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
        private readonly Segmented _sweetness;
        private readonly Stepper _qty;
        private readonly HiveButton _add;
        private readonly HiveButton _close;

        private int _heroHeight = 175;

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

            bool isBeverage = _product.HasTemperature || _product.Category == "Coffee" || _product.Category == "Non-Coffee" || _product.Category == "Classics" || _product.Category == "GentleTea";

            // If not a drink with many options, expand the hero photo
            if (!_product.HasSizes && !_product.HasTemperature)
                _heroHeight = 250;
            else if (!_product.HasSizes || !_product.HasTemperature)
                _heroHeight = 210;

            int left = Hive.Gutter + 4;
            int width = SheetW - left * 2;
            int y = _heroHeight + 66;

            if (_product.HasSizes)
            {
                y += 20;
                _size = new Segmented();
                _size.Options = Array.ConvertAll(_product.Sizes, s => s.Label + " (" + Hive.MoneyShort(s.Price) + ")");
                _size.Bounds = new Rectangle(left, y, width, 44);
                _size.SelectionChanged += (s, e) => Refresh();
                Controls.Add(_size);
                y += 44 + 14;
            }

            if (_product.HasTemperature)
            {
                y += 20;
                _temp = new Segmented();
                _temp.Options = new[] { "Hot", "Iced" };
                _temp.Bounds = new Rectangle(left, y, width, 44);
                Controls.Add(_temp);
                y += 44 + 14;
            }

            if (isBeverage)
            {
                y += 20;
                _sweetness = new Segmented();
                _sweetness.Options = new[] { "100%", "75%", "50%", "25%", "0%" };
                _sweetness.Bounds = new Rectangle(left, y, width, 40);
                _sweetness.SelectedIndex = 0;
                Controls.Add(_sweetness);
                y += 40 + 14;
            }

            y += 20;
            _qty = new Stepper();
            _qty.Bounds = new Rectangle(left, y, 150, 44);
            _qty.ValueChanged += (s, e) => Refresh();
            Controls.Add(_qty);

            int footerTop = Hive.ScreenH - 96;
            _add = new HiveButton();
            _add.Text = "Add to order";
            _add.Style = HiveStyle.Accent;
            _add.Size = new Size(218, 58);
            _add.Location = new Point(SheetW - 218 - Hive.Gutter - 4, footerTop + 18);
            _add.Click += AddToOrder;
            Controls.Add(_add);

            _close = new HiveButton();
            _close.Icon = Mark.Cross;
            _close.Style = HiveStyle.Light;
            _close.Size = new Size(42, 42);
            _close.Location = new Point(SheetW - 42 - 14, 14);
            _close.Click += (s, e) => Nav.Back();
            Controls.Add(_close);

            ClientSize = new Size(SheetW, Hive.ScreenH);

            if (_size != null) _size.SelectedIndex = 0;
            if (_temp != null) _temp.SelectedIndex = 1;
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

            string temp = "";
            if (_product.HasTemperature)
            {
                temp = (_temp != null && _temp.SelectedIndex == 1) ? "Iced" : "Hot";
            }

            string sweetness = _sweetness != null ? _sweetness.SelectedOption : null;

            try
            {
                OrderStorage.AddOrder(new Order
                {
                    Product = _product.Name,
                    Size = sizeCode,
                    Temperature = temp,
                    Sweetness = sweetness,
                    Quantity = _qty.Value,
                    Price = UnitPrice,
                    ImagePath = _product.ImageKey
                });

                Nav.Back();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Surface);

            RectangleF hero = new RectangleF(12, 8, Width - 24, _heroHeight - 14);
            using (GraphicsPath clip = Hive.Rounded(hero, 20))
            {
                if (_product.Image != null)
                {
                    Region saved = g.Clip;
                    g.SetClip(clip, CombineMode.Intersect);
                    using (SolidBrush wash = new SolidBrush(Color.White))
                        g.FillRectangle(wash, hero);
                    Hive.ImageContainProduct(g, _product.Image, hero, 10f);

                    // Soft bottom gradient
                    using (LinearGradientBrush vig = Hive.Gradient(new RectangleF(hero.X, hero.Bottom - 26, hero.Width, 26),
                                                                  Color.FromArgb(0, 0, 0, 0), Color.FromArgb(24, 10, 34, 36), 90f))
                        g.FillRectangle(vig, hero.X, hero.Bottom - 26, hero.Width, 26);

                    g.Clip = saved;
                }
                else
                {
                    Hive.Monogram(g, _product.Name, hero, clip, "Artisan Recipe");
                }
            }

            if (_product.Badge != null)
            {
                string badgeText = _product.Badge.ToUpperInvariant();
                Size size = TextRenderer.MeasureText(badgeText, Hive.Overline);
                RectangleF badge = new RectangleF(22, 18, size.Width + 24, 24);
                Hive.Fill(g, badge, 12, Hive.Honey);
                Hive.TextTracked(g, badgeText, Hive.Overline, Rectangle.Round(badge), Color.White, 1.2f, true);
            }

            int left = Hive.Gutter + 4;
            int width = Width - left * 2;

            Hive.Text(g, _product.Name, Hive.Title,
                      new Rectangle(left, _heroHeight + 8, width, 32), Hive.TealDarker, Hive.LeftMid);

            Hive.Text(g, _product.Description, Hive.Body,
                      new Rectangle(left, _heroHeight + 38, width, 34), Hive.Muted,
                      TextFormatFlags.WordBreak | TextFormatFlags.Top);

            if (_size != null) SectionLabel(g, "SELECT SIZE", _size.Top);
            if (_temp != null) SectionLabel(g, "TEMPERATURE", _temp.Top);
            if (_sweetness != null) SectionLabel(g, "SWEETNESS LEVEL", _sweetness.Top);
            SectionLabel(g, "QUANTITY", _qty.Top);

            int noteTop = _qty.Bottom + 16;
            int detailLeft = left + 100;
            using (Pen p = new Pen(Hive.LineSoft, 1))
                g.DrawLine(p, left, noteTop, left + width, noteTop);
            Hive.TextTracked(g, GuestText.T("CAFFEINE"), Hive.Overline,
                             new Rectangle(left, noteTop + 9, 92, 20), Hive.Muted, 0.7f, false);
            Hive.Text(g, GuestText.T(_product.DisplayCaffeineNote), Hive.Caption,
                      new Rectangle(detailLeft, noteTop + 9, width - 100, 20), Hive.InkSoft, Hive.LeftMid);
            Hive.TextTracked(g, GuestText.T("ALLERGENS"), Hive.Overline,
                             new Rectangle(left, noteTop + 35, 92, 20), Hive.Muted, 0.7f, false);
            Hive.Text(g, GuestText.T(_product.DisplayAllergenNote), Hive.Caption,
                      new Rectangle(detailLeft, noteTop + 35, width - 100, 20), Hive.InkSoft, Hive.LeftMid);

            int footerTop = _add.Top - 18;
            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, Hive.Gutter, footerTop, Width - Hive.Gutter, footerTop);

            Hive.Text(g, "SUBTOTAL", Hive.Overline,
                      new Rectangle(left, footerTop + 14, 140, 16), Hive.Muted, Hive.LeftMid);
            Hive.Text(g, Hive.Money(Total), Hive.PriceBig,
                      new Rectangle(left, footerTop + 30, 160, 34), Hive.TealDeep, Hive.LeftMid);
        }

        private void SectionLabel(Graphics g, string text, int controlTop)
        {
            Hive.TextTracked(g, text, Hive.Overline,
                             new Rectangle(Hive.Gutter + 4, controlTop - 20, 200, 18), Hive.Muted, 1.4f, false);
        }
    }
}

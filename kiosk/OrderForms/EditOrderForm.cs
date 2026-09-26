using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using kiosk.UI;

namespace kiosk
{
    public partial class EditOrderForm : Form
    {
        private const int SheetW = Hive.ScreenW;

        private readonly Order _original;
        private readonly MenuProduct _product;
        private readonly Segmented _size;
        private readonly Segmented _temp;
        private readonly Segmented _sweetness;
        private readonly Stepper _qty;
        private readonly HiveButton _save;
        private readonly HiveButton _cancel;
        private readonly HiveButton _remove;

        private bool _removeArmed;
        private readonly int _totalRowTop;

        public Order UpdatedOrder { get; private set; }
        public bool IsRemoved { get; private set; }

        public event EventHandler Committed;

        public EditOrderForm(Order orderToEdit)
        {
            _original = orderToEdit;
            _product = MenuCatalog.Find(orderToEdit.Product);

            InitializeComponent();

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Hive.Surface;
            Font = Hive.Body;
            ShowInTaskbar = false;

            int left = Hive.Gutter + 4;
            int width = SheetW - left * 2;
            int y = 108;

            if (_product != null && _product.HasSizes)
            {
                y += 22;
                _size = new Segmented();
                _size.Options = Array.ConvertAll(_product.Sizes, s => s.Label);
                _size.Bounds = new Rectangle(left, y, width, Hive.TapTarget);
                _size.SelectionChanged += (s, e) => Refresh();
                Controls.Add(_size);
                y += Hive.TapTarget + 14;
            }

            if (_product != null && _product.HasTemperature)
            {
                y += 22;
                _temp = new Segmented();
                _temp.Options = new[] { "Hot", "Iced" };
                _temp.Bounds = new Rectangle(left, y, width, Hive.TapTarget);
                Controls.Add(_temp);
                y += Hive.TapTarget + 14;
            }

            if (_product != null && !string.IsNullOrWhiteSpace(_original.Sweetness))
            {
                y += 22;
                _sweetness = new Segmented { Options = new[] { "100%", "75%", "50%", "25%", "0%" } };
                _sweetness.Bounds = new Rectangle(left, y, width, 44);
                Controls.Add(_sweetness);
                y += 58;
            }

            y += 22;
            _qty = new Stepper();
            _qty.Bounds = new Rectangle(left, y, 170, Hive.TapTarget);
            _qty.ValueChanged += (s, e) => Refresh();
            Controls.Add(_qty);
            y += Hive.TapTarget + 14;

            // options from the top, buttons pinned to the bottom
            int footerTop = Hive.ScreenH - 92;
            _totalRowTop = footerTop - 108;

            _remove = new HiveButton();
            _remove.Text = "Remove item";
            _remove.Style = HiveStyle.Ghost;
            _remove.TextColor = Hive.Danger;
            _remove.Bounds = new Rectangle(left, footerTop - 60, width, 44);
            _remove.Click += RemoveClicked;
            Controls.Add(_remove);

            _cancel = new HiveButton();
            _cancel.Text = "Cancel";
            _cancel.Style = HiveStyle.Outline;
            _cancel.Bounds = new Rectangle(left, footerTop + 14, 150, 54);
            _cancel.Click += (s, e) => Nav.Back();
            Controls.Add(_cancel);

            _save = new HiveButton();
            _save.Text = "Save changes";
            _save.Style = HiveStyle.Accent;
            _save.Bounds = new Rectangle(left + 158, footerTop + 14, width - 158, 54);
            _save.Click += SaveClicked;
            Controls.Add(_save);

            ClientSize = new Size(SheetW, Hive.ScreenH);

            PreselectFrom(orderToEdit);
        }

        private void PreselectFrom(Order order)
        {
            _qty.Value = Math.Max(1, order.Quantity);

            if (_size != null)
            {
                int index = Array.FindIndex(_product.Sizes, s => s.Code == order.Size);
                _size.SelectedIndex = index < 0 ? 0 : index;
            }

            if (_temp != null)
                _temp.SelectedIndex = string.Equals(order.Temperature, "Iced", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            if (_sweetness != null)
            {
                int index = Array.IndexOf(_sweetness.Options, order.Sweetness ?? "100%");
                _sweetness.SelectedIndex = index < 0 ? 0 : index;
            }
        }

        private decimal UnitPrice
        {
            get
            {
                if (_product == null) return _original.Price;
                if (!_product.HasSizes) return _product.BasePrice;
                int i = _size == null || _size.SelectedIndex < 0 ? 0 : _size.SelectedIndex;
                return _product.Sizes[i].Price;
            }
        }

        private void RemoveClicked(object sender, EventArgs e)
        {
            if (!_removeArmed)
            {
                _removeArmed = true;
                _remove.Text = "Tap again to remove";
                _remove.Style = HiveStyle.Danger;
                return;
            }

            IsRemoved = true;
            Commit();
        }

        private void Commit()
        {
            EventHandler handler = Committed;
            if (handler != null) handler(this, EventArgs.Empty);
            Nav.Back();
        }

        private void SaveClicked(object sender, EventArgs e)
        {
            string sizeCode = _original.Size;
            if (_size != null && _product != null)
                sizeCode = _product.Sizes[Math.Max(0, _size.SelectedIndex)].Code;

            UpdatedOrder = new Order
            {
                Product = _original.Product,
                Size = sizeCode,
                Temperature = _temp != null ? (_temp.SelectedOption ?? _original.Temperature) : _original.Temperature,
                Quantity = _qty.Value,
                Price = UnitPrice,
                ImagePath = _original.ImagePath,
                Sweetness = _sweetness != null ? _sweetness.SelectedOption : _original.Sweetness
            };

            try { OrderStorage.Validate(UpdatedOrder); }
            catch (InvalidOperationException ex) { NoticeForm.Say("Cannot save that change", ex.Message); return; }
            Commit();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            g.Clear(Hive.Surface);

            RectangleF band = new RectangleF(0, 0, Width, 88);
            using (SolidBrush b = new SolidBrush(Hive.Teal))
                g.FillRectangle(b, band);

            using (SolidBrush rule = new SolidBrush(Hive.Honey))
                g.FillRectangle(rule, 0, band.Height - 3, Width, 3);

            int left = Hive.Gutter + 4;

            Hive.Text(g, "EDIT ITEM", Hive.Overline,
                      new Rectangle(left, 18, 240, 16), Color.FromArgb(170, 255, 255, 255), Hive.LeftMid);
            Hive.Text(g, _original.Product, Hive.Title,
                      new Rectangle(left, 36, Width - left * 2, 34), Color.White, Hive.LeftMid);

            if (_size != null) SectionLabel(g, "SIZE", _size.Top);
            if (_temp != null) SectionLabel(g, "SERVED", _temp.Top);
            if (_sweetness != null) SectionLabel(g, "SWEETNESS", _sweetness.Top);
            SectionLabel(g, "QUANTITY", _qty.Top);

            int footerTop = _save.Top - 14;
            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, Hive.Gutter, footerTop, Width - Hive.Gutter, footerTop);

            Summary.Row(g, new Rectangle(left, _totalRowTop, Width - left * 2, 34),
                        "Line total", Hive.Money(UnitPrice * _qty.Value), true);
        }

        private void SectionLabel(Graphics g, string text, int controlTop)
        {
            Hive.TextTracked(g, text, Hive.Overline,
                             new Rectangle(Hive.Gutter + 4, controlTop - 22, 200, 18), Hive.Muted, 1.4f, false);
        }
    }
}

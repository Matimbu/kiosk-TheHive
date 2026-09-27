using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace kiosk.UI
{

    public abstract class HiveControl : Control
    {
        protected HiveControl()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = Hive.Body;
            ForeColor = Hive.Ink;
        }
    }

    public enum HiveStyle
    {
        Primary,   // teal fill - the main action on a screen
        Accent,    // honey fill - "add to order", "pay"
        Outline,   // hairline border - secondary
        Ghost,     // text only - tertiary / cancel
        Light,     // white fill - action sitting on a coloured surface
        Danger,
        Success
    }

    public class HiveButton : HiveControl, IButtonControl
    {
        private readonly Anim _hover;
        private readonly Anim _press;
        private HiveStyle _style = HiveStyle.Primary;
        private int _radius = Hive.RadiusButton;
        private Mark? _icon;
        private bool _tracked = true;
        private string _caption;

        public HiveButton()
        {
            _hover = new Anim(this, 0.25f);
            _press = new Anim(this, 0.4f);
            Cursor = Cursors.Hand;
            Size = new Size(160, Hive.TapTarget);
            Font = Hive.Subhead;
        }

        [System.ComponentModel.DefaultValue(HiveStyle.Primary)]
        public HiveStyle Style
        {
            get { return _style; }
            set { _style = value; Invalidate(); }
        }

        public int Radius
        {
            get { return _radius; }
            set { _radius = value; UpdateShape(); Invalidate(); }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateShape();
        }

        private void UpdateShape()
        {
            if (Width < 2 || Height < 2) return;
            using (GraphicsPath path = Hive.Rounded(new RectangleF(0, 0, Width, Height), _radius))
            {
                Region old = Region;
                Region = new Region(path);
                if (old != null) old.Dispose();
            }
        }
        public Mark? Icon
        {
            get { return _icon; }
            set { _icon = value; Invalidate(); }
        }

        public bool Tracked
        {
            get { return _tracked; }
            set { _tracked = value; Invalidate(); }
        }

        public Color TextColor { get; set; }

        public string Caption
        {
            get { return _caption; }
            set { _caption = value; Invalidate(); }
        }

        public DialogResult DialogResult { get; set; }
        public void NotifyDefault(bool value) { }
        public void PerformClick() { if (Enabled) OnClick(EventArgs.Empty); }

        public void HoldPressed() { _press.Set(1f); }
        public void ReleasePressed() { _press.To(0f); }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (DialogResult == DialogResult.None) return;
            Form host = FindForm();
            if (host != null) host.DialogResult = DialogResult;
        }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); _press.To(0f); }
        protected override void OnMouseDown(MouseEventArgs e) { base.OnMouseDown(e); _press.Set(1f); }
        protected override void OnMouseUp(MouseEventArgs e) { base.OnMouseUp(e); _press.To(0f); }
        protected override void OnEnabledChanged(EventArgs e) { base.OnEnabledChanged(e); Invalidate(); }

        private void Palette(out Color fill, out Color text, out Color border)
        {
            switch (_style)
            {
                // White on honey is 3.05:1, under WCAG's 4.5:1. Kept on purpose: the
                // cafe chose the bright honey look over dark lettering (Sept 2026).
                // Ask before changing it.
                case HiveStyle.Accent:  fill = Hive.Honey;   text = Color.White;   border = Color.Empty; break;
                case HiveStyle.Outline: fill = Color.Empty;  text = Hive.Teal;     border = Hive.Line;   break;
                case HiveStyle.Ghost:   fill = Color.Empty;  text = Hive.InkSoft;  border = Color.Empty; break;
                case HiveStyle.Light:   fill = Color.White;  text = Hive.Teal;     border = Color.Empty; break;
                case HiveStyle.Danger:  fill = Hive.Danger;  text = Color.White;   border = Color.Empty; break;
                case HiveStyle.Success: fill = Hive.Success; text = Color.White;   border = Color.Empty; break;
                default:                fill = Hive.Teal;    text = Color.White;   border = Color.Empty; break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            float radius = _radius;
            float pressOffset = _press.Value * 1.5f;
            RectangleF body = new RectangleF(1, 1 + pressOffset, Width - 3, Height - 3 - pressOffset);

            Color fill, text, border;
            Palette(out fill, out text, out border);

            float lift = _hover.Value * (1f - _press.Value * 0.8f);

            if (!Enabled)
            {
                fill = fill.IsEmpty ? Color.Empty : Hive.SurfaceAlt;
                text = Hive.Disabled;
                border = Hive.Line;
            }
            else
            {
                if (!TextColor.IsEmpty) text = TextColor;
                if (!fill.IsEmpty)
                {
                    fill = Hive.Lighten(fill, lift * 0.10f);
                    fill = Hive.Darken(fill, _press.Value * 0.12f);
                }
            }

            if (Enabled && lift > 0.02f && (!fill.IsEmpty || _style == HiveStyle.Outline))
                Hive.Shadow(g, body, radius, 2, (int)(15 * lift));

            if (!fill.IsEmpty)
            {
                Hive.Fill(g, body, radius, fill);
                if (Enabled && (_style == HiveStyle.Accent || _style == HiveStyle.Primary))
                    Hive.Stroke(g, RectangleF.Inflate(body, -2, -2), radius - 2,
                        Color.FromArgb(60, 255, 255, 255), 1);
            }
            else if (Enabled && lift > 0.01f)
            {
                Hive.Fill(g, body, radius, Color.FromArgb((int)(26 * lift), Hive.Teal));
            }

            if (!border.IsEmpty)
                Hive.Stroke(g, body, radius, Enabled ? Hive.Mix(border, Hive.Teal, lift * 0.7f) : border, 1.2f);

            if (Focused && Enabled)
                Hive.Stroke(g, RectangleF.Inflate(body, -4, -4), Math.Max(2, radius - 4),
                    Color.FromArgb(140, 255, 255, 255), 1.2f);

            string label = Text ?? string.Empty;

            if (_icon.HasValue && label.Length == 0)
            {
                Marks.Draw(g, _icon.Value, RectangleF.Inflate(body, -Width * 0.3f, -Height * 0.3f), text, 1.9f);
                return;
            }

            Rectangle inner = Inner();

            if (!string.IsNullOrEmpty(_caption))
            {
                Rectangle top = new Rectangle(inner.X, 6, inner.Width, Height / 2 - 2);
                Rectangle bot = new Rectangle(inner.X, Height / 2 - 2, inner.Width, Height / 2 - 4);
                Hive.Text(g, label, Font, top, text, Hive.Centered);
                Hive.Text(g, _caption, Hive.Caption, bot, Color.FromArgb(190, text), Hive.Centered);
            }
            else if (_tracked)
            {
                Hive.TextTracked(g, label.ToUpperInvariant(), Font, inner, text, 1.1f, true);
            }
            else
            {
                Hive.Text(g, label, Font, inner, text, Hive.Centered);
            }
        }

        // the label area, shared by paint and LabelFits so they cannot disagree
        private Rectangle Inner()
        {
            int pad = Math.Min(12, Math.Max(2, Width / 8));
            return new Rectangle(pad, 0, Width - pad * 2, Height);
        }

        // Widens the button until its label fits, for buttons whose text changes
        // with the language. Measures off-screen, so it works before the button
        // has a parent.
        public void FitToLabel(int minWidth)
        {
            string label = Text ?? string.Empty;
            using (Bitmap scratch = new Bitmap(1, 1))
            using (Graphics g = Graphics.FromImage(scratch))
            {
                float need = _tracked && string.IsNullOrEmpty(_caption)
                    ? Hive.TrackedWidth(g, Hive.TrackedLabel(label.ToUpperInvariant()), Font, 1.1f)
                    : TextRenderer.MeasureText(g, GuestText.T(label), Font).Width;
                // Inner() pads 12px a side once the button is 96 or wider
                Width = Math.Max(minWidth, (int)Math.Ceiling(need) + 24 + 2);
            }
        }
        // False when the label would be cut off with an ellipsis. Tracked labels
        // are measured exactly as TextTracked draws them, caps and all.
        public bool LabelFits(Graphics g)
        {
            string label = Text ?? string.Empty;
            if (label.Length == 0) return true;
            int room = Inner().Width;
            if (_tracked && string.IsNullOrEmpty(_caption))
                return Hive.TrackedWidth(g, Hive.TrackedLabel(label.ToUpperInvariant()), Font, 1.1f) <= room;
            return TextRenderer.MeasureText(g, GuestText.T(label), Font).Width <= room;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing) { _hover.Dispose(); _press.Dispose(); }
            base.Dispose(disposing);
        }
    }

    public class HiveCard : Panel
    {
        private int _radius = Hive.RadiusCard;
        private int _elevation = 0;
        private Color _fill = Hive.Surface;
        private Color _border = Hive.Line;

        public HiveCard()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Font = Hive.Body;
            ForeColor = Hive.Ink;
        }

        public int Radius     { get { return _radius; }    set { _radius = value; Invalidate(); } }
        public int Elevation  { get { return _elevation; } set { _elevation = value; Invalidate(); } }
        public Color Fill     { get { return _fill; }      set { _fill = value; Invalidate(); } }
        public Color Border   { get { return _border; }    set { _border = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);
            RectangleF body = new RectangleF(_elevation, _elevation,
                                             Width - _elevation * 2 - 1,
                                             Height - _elevation * 2 - 1);
            if (_elevation > 0) Hive.Shadow(g, body, _radius, _elevation, 40);
            Hive.Fill(g, body, _radius, _fill);
            if (!_border.IsEmpty) Hive.Stroke(g, body, _radius, _border, 1.2f);
            base.OnPaint(e);
        }
    }

    public class HiveBand : Panel
    {
        private Color _from = Hive.Teal;
        private Color _to = Hive.TealDeep;
        private float _angle = 90f;

        public HiveBand()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw, true);
        }

        public Color From  { get { return _from; }  set { _from = value; Invalidate(); } }
        public Color To    { get { return _to; }    set { _to = value; Invalidate(); } }
        public float Angle { get { return _angle; } set { _angle = value; Invalidate(); } }

        protected override void OnPaint(PaintEventArgs e)
        {
            Hive.Smooth(e.Graphics);
            Hive.FillGradient(e.Graphics, ClientRectangle, 0, _from, _to, _angle);
            base.OnPaint(e);
        }
    }

    public class HiveChip : HiveControl
    {
        private readonly Anim _hover;
        private bool _selected;

        public HiveChip()
        {
            _hover = new Anim(this, 0.3f);
            Cursor = Cursors.Hand;
            Font = Hive.Tab;
            BackColor = Hive.Surface;
            Height = 52;
        }

        public bool Selected
        {
            get { return _selected; }
            set { if (_selected == value) return; _selected = value; Invalidate(); }
        }

        public object Tagged { get; set; }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            g.Clear(Hive.Surface);
            Color ink = _selected ? Color.White : Hive.InkSoft;
            // Equal margins top and bottom, with the icon and label centred inside.
            RectangleF surface = new RectangleF(2, 4, Width - 4, Height - 8);
            if (_selected) Hive.Fill(g, surface, Hive.RadiusButton, Hive.Teal);
            else if (_hover.Value > 0.01f) Hive.Fill(g, surface, Hive.RadiusButton, Hive.SurfaceAlt);
            CategoryIcons.Draw(g, Text, new RectangleF((Width - 24) / 2f, 12, 24, 24),
                _selected ? Hive.HoneyLight : ink);
            Hive.Text(g, Text, Hive.BodyBold, new Rectangle(4, 41, Width - 8, 22), ink, Hive.Centered);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hover.Dispose();
            base.Dispose(disposing);
        }
    }

    public class ChipRail : Panel
    {
        private readonly Panel _strip;
        private bool _dragging;
        private bool _dragMoved;
        private int _dragOrigin;
        private int _stripOrigin;
        private HiveChip _selected;

        public event EventHandler<ChipEventArgs> ChipSelected;

        // Tuned so four English categories fit and the fifth is cut by the screen
        // edge - the only sign a guest gets that the rail scrolls.
        private const int ChipPadX = 15;
        private const int ChipGap = 6;
        private const int ChipMinWidth = 56;   // a floor for touch, not for looks

        public ChipRail()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.ResizeRedraw, true);
            BackColor = Hive.Surface;
            Height = 74;

            _strip = new Panel();
            _strip.Location = new Point(Hive.Gutter, 2);   // centres the chips above the hairline
            _strip.BackColor = Color.Transparent;
            _strip.Height = 70;
            Controls.Add(_strip);

            MouseWheel += (s, e) => ScrollBy(e.Delta > 0 ? 60 : -60);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.Clear(Hive.Surface);
            using (Pen p = new Pen(Hive.Line, 1))
                g.DrawLine(p, 0, Height - 1, Width, Height - 1);

            // tabs are added at runtime, so fake them for the designer
            if (DesignMode && _strip.Controls.Count == 0)
                PaintDesignPreview(g);

            base.OnPaint(e);
        }

        private void PaintDesignPreview(Graphics g)
        {
            Hive.Smooth(g);
            int x = Hive.Gutter;

            for (int i = 0; i < MenuCatalog.Categories.Length && x < Width; i++)
            {
                string label = MenuCatalog.Categories[i].Name.ToUpperInvariant();
                int w = TextRenderer.MeasureText(label, Hive.Tab).Width + (int)(label.Length * 1.0f) + 18;

                Hive.TextTracked(g, label, Hive.Tab, new Rectangle(x, 0, w, Height - 5),
                                 i == 0 ? Hive.Ink : Hive.Muted, 1.0f, true);

                if (i == 0)
                    using (SolidBrush b = new SolidBrush(Hive.Teal))
                        g.FillRectangle(b, x, Height - 5, w, 3);

                x += w + 4;
            }
        }

        public void Add(string label, object tag)
        {
            HiveChip chip = new HiveChip();
            chip.Text = label;
            chip.Tagged = tag;
            chip.Top = 0;
            chip.Height = _strip.Height;
            chip.AccessibleName = label;
            chip.AccessibleRole = AccessibleRole.PageTab;

            // Each chip is its own label plus the same padding, so the gap between
            // labels - which is what reads as the spacing - is constant all along.
            // A fixed minimum width padded short labels out and made it uneven.
            // Measure the translated label, since that is what the chip draws.
            int textW = TextRenderer.MeasureText(GuestText.T(label), Hive.BodyBold).Width;
            chip.Width = Math.Max(ChipMinWidth, textW + ChipPadX * 2);

            chip.Left = _strip.Controls.Count == 0 ? 0 : LastRight() + ChipGap;
            chip.Click += (s, e) => { if (!_dragMoved) Select(chip); };
            AttachDrag(chip);
            _strip.Controls.Add(chip);
            _strip.Width = LastRight() + Hive.Gutter;
        }

        private int LastRight()
        {
            int right = 0;
            foreach (Control c in _strip.Controls) right = Math.Max(right, c.Right);
            return right;
        }

        public void Select(int index)
        {
            if (index >= 0 && index < _strip.Controls.Count)
                Select(_strip.Controls[index] as HiveChip);
        }

        public void Select(HiveChip chip)
        {
            if (chip == null) return;
            foreach (Control c in _strip.Controls)
            {
                HiveChip other = c as HiveChip;
                if (other != null) other.Selected = ReferenceEquals(other, chip);
            }
            _selected = chip;
            BringIntoView(chip);
            EventHandler<ChipEventArgs> handler = ChipSelected;
            if (handler != null) handler(this, new ChipEventArgs(chip.Text, chip.Tagged));
        }

        public HiveChip SelectedChip { get { return _selected; } }

        private void BringIntoView(Control chip)
        {
            int left = _strip.Left + chip.Left;
            int right = left + chip.Width;
            if (left < Hive.Gutter) ScrollBy(Hive.Gutter - left);
            else if (right > Width - Hive.Gutter) ScrollBy(Width - Hive.Gutter - right);
        }

        private void ScrollBy(int delta)
        {
            SetLeft(_strip.Left + delta);
        }

        private void SetLeft(int left)
        {
            int min = Math.Min(Hive.Gutter, Width - _strip.Width);
            _strip.Left = Math.Max(min, Math.Min(Hive.Gutter, left));
        }

        private void AttachDrag(Control c)
        {
            c.MouseDown += (s, e) =>
            {
                _dragging = true;
                _dragMoved = false;
                _dragOrigin = Cursor.Position.X;
                _stripOrigin = _strip.Left;
            };

            c.MouseMove += (s, e) =>
            {
                if (!_dragging) return;
                int delta = Cursor.Position.X - _dragOrigin;
                if (Math.Abs(delta) <= 3) return;
                _dragMoved = true;
                SetLeft(_stripOrigin + delta);
            };

            c.MouseUp += (s, e) => { _dragging = false; SetLeft(_strip.Left); };
        }
    }

    public class ChipEventArgs : EventArgs
    {
        public ChipEventArgs(string label, object tag) { Label = label; Tag = tag; }
        public string Label { get; private set; }
        public object Tag { get; private set; }
    }

    public class Segmented : HiveControl
    {
        private readonly Anim _slide;
        private string[] _options = new string[0];
        private int _index = -1;
        private int _hot = -1;
        private float _from;

        public event EventHandler SelectionChanged;

        public Segmented()
        {
            _slide = new Anim(this, 0.28f);
            Cursor = Cursors.Hand;
            Font = Hive.Subhead;
            Height = Hive.TapTarget;
        }

        public string[] Options
        {
            get { return _options; }
            set { _options = value ?? new string[0]; _index = -1; Invalidate(); }
        }

        public int SelectedIndex
        {
            get { return _index; }
            set
            {
                if (value == _index || value < -1 || value >= _options.Length) return;
                _from = _index < 0 ? value : _index;
                _index = value;
                _slide.Set(0f);
                _slide.To(1f);
                EventHandler handler = SelectionChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        public string SelectedOption
        {
            get { return _index >= 0 && _index < _options.Length ? _options[_index] : null; }
        }

        private RectangleF SlotOf(float index)
        {
            float w = (Width - 6f) / Math.Max(1, _options.Length);
            return new RectangleF(3 + w * index, 3, w, Height - 7);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int hot = HitTest(e.X);
            if (hot != _hot) { _hot = hot; Invalidate(); }
        }

        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hot = -1; Invalidate(); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            int hit = HitTest(e.X);
            if (hit >= 0) SelectedIndex = hit;
        }

        private int HitTest(int x)
        {
            if (_options.Length == 0) return -1;
            float w = (Width - 8f) / _options.Length;
            int i = (int)((x - 4) / w);
            return i >= 0 && i < _options.Length ? i : -1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            RectangleF track = new RectangleF(0, 0, Width - 1, Height - 1);
            Hive.Fill(g, track, Hive.RadiusButton, Hive.SurfaceAlt);

            if (_index >= 0)
            {
                float pos = _from + (_index - _from) * _slide.Value;
                RectangleF slot = RectangleF.Inflate(SlotOf(pos), -2f, -2f);
                Hive.Fill(g, slot, Hive.RadiusButton - 2, Hive.Teal);   // inset 2px, so 2px tighter
            }

            for (int i = 0; i < _options.Length; i++)
            {
                Rectangle slot = Rectangle.Round(SlotOf(i));
                Color color = i == _index ? Color.White
                            : i == _hot   ? Hive.TealDeep
                            : Hive.InkSoft;
                Font font = i == _index ? Hive.BodyBold : Font;
                Hive.Text(g, _options[i], font, slot, color, Hive.Centered);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _slide.Dispose();
            base.Dispose(disposing);
        }
    }

    public class Stepper : HiveControl
    {
        private int _value = 1;
        private int _min = 1;
        private int _max = 99;
        private int _hot;   // -1 minus, +1 plus, 0 none

        public event EventHandler ValueChanged;

        public Stepper()
        {
            Cursor = Cursors.Hand;
            Font = Hive.Price;
            Size = new Size(168, Hive.TapTarget);
        }

        public int Minimum { get { return _min; } set { _min = value; Value = _value; } }
        public int Maximum { get { return _max; } set { _max = value; Value = _value; } }

        public int Value
        {
            get { return _value; }
            set
            {
                int clamped = Math.Max(_min, Math.Min(_max, value));
                if (clamped == _value) { Invalidate(); return; }
                _value = clamped;
                Invalidate();
                EventHandler handler = ValueChanged;
                if (handler != null) handler(this, EventArgs.Empty);
            }
        }

        private Rectangle MinusRect { get { return new Rectangle(0, 0, Height, Height); } }
        private Rectangle PlusRect  { get { return new Rectangle(Width - Height, 0, Height, Height); } }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int hot = MinusRect.Contains(e.Location) ? -1 : PlusRect.Contains(e.Location) ? 1 : 0;
            if (hot != _hot) { _hot = hot; Invalidate(); }
        }

        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hot = 0; Invalidate(); }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (MinusRect.Contains(e.Location)) Value = _value - 1;
            else if (PlusRect.Contains(e.Location)) Value = _value + 1;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            RectangleF track = new RectangleF(0, 0, Width - 1, Height - 1);
            Hive.Fill(g, track, Hive.RadiusButton, Hive.SurfaceAlt);

            DrawKnob(g, MinusRect, false, _value > _min, _hot == -1);
            DrawKnob(g, PlusRect,  true,  _value < _max, _hot == 1);

            Rectangle mid = new Rectangle(Height, 0, Width - Height * 2, Height);
            Hive.Text(g, _value.ToString(), Hive.Price, mid, Hive.TealDeep, Hive.Centered);
        }

        // the knob sits 5px inside the track, so its corners run 5px tighter
        // to stay concentric with the track's
        private const float KnobRadius = Hive.RadiusButton - 5;

        private void DrawKnob(Graphics g, Rectangle bounds, bool plus, bool enabled, bool hot)
        {
            RectangleF knob = RectangleF.Inflate(bounds, -5, -5);
            if (enabled && hot)
                Hive.Fill(g, knob, KnobRadius, Hive.HoneyWash);
            else if (enabled)
                Hive.Fill(g, knob, KnobRadius, Hive.SurfaceAlt);

            Color ink = !enabled ? Hive.Mix(Hive.Disabled, Color.White, 0.5f)
                      : hot      ? Hive.Honey
                                 : Hive.Teal;

            float cx = bounds.X + bounds.Width / 2f;
            float cy = bounds.Y + bounds.Height / 2f;
            const float arm = 6f;

            using (Pen pen = new Pen(ink, 2.2f))
            {
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawLine(pen, cx - arm, cy, cx + arm, cy);
                if (plus) g.DrawLine(pen, cx, cy - arm, cx, cy + arm);
            }
        }
    }

    public class HiveField : Panel
    {
        private readonly TextBox _input = new TextBox();
        private string _label;
        private string _hint;

        public HiveField()
        {
            SetStyle(ControlStyles.UserPaint
                   | ControlStyles.AllPaintingInWmPaint
                   | ControlStyles.OptimizedDoubleBuffer
                   | ControlStyles.ResizeRedraw
                   | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
            Size = new Size(240, 74);

            _input.BorderStyle = BorderStyle.None;
            _input.Font = Hive.Subhead;
            _input.ForeColor = Hive.Ink;
            _input.BackColor = Hive.Surface;
            _input.GotFocus += (s, e) => Invalidate();
            _input.LostFocus += (s, e) => Invalidate();
            Controls.Add(_input);
        }

        public TextBox Input { get { return _input; } }
        public string Label { get { return _label; } set { _label = value; Invalidate(); } }

        public string Hint
        {
            get { return _hint; }
            set { _hint = value; ApplyHint(); Invalidate(); }
        }

        private const int EM_SETCUEBANNER = 0x1501;

        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Unicode)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

        private void ApplyHint()
        {
            // EM_SETCUEBANNER is a window message, needs a handle first
            if (_hint == null || !IsHandleCreated) return;
            SendMessage(_input.Handle, EM_SETCUEBANNER, (IntPtr)1, _hint);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyHint();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            _input.Bounds = new Rectangle(16, 20 + (Height - 20 - _input.Height) / 2, Width - 32, _input.Height);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            if (!string.IsNullOrEmpty(_label))
                Hive.Text(g, _label, Hive.Overline, new Rectangle(2, 0, Width, 16), Hive.Muted, Hive.LeftMid);

            RectangleF box = new RectangleF(0, 20, Width - 1, Height - 21);
            Hive.Fill(g, box, Hive.RadiusButton, Hive.Surface);
            Hive.Stroke(g, box, Hive.RadiusButton, _input.Focused ? Hive.Teal : Hive.Line, _input.Focused ? 1.8f : 1.3f);

            base.OnPaint(e);
        }
    }

    public class MethodTile : HiveControl
    {
        private readonly Anim _hover;

        public MethodTile(Mark glyph, string title, string hint)
        {
            _hover = new Anim(this, 0.26f);
            Glyph = glyph;
            Title = title;
            Hint = hint;
            Cursor = Cursors.Hand;
            Height = 86;
        }

        public Mark Glyph { get; set; }
        public string Title { get; set; }
        public string Hint { get; set; }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); _hover.To(1f); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); _hover.To(0f); }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Hive.Smooth(g);

            float lift = _hover.Value;
            RectangleF card = new RectangleF(0, 0, Width - 1, Height - 7);
            Hive.Fill(g, card, Hive.RadiusCard, Hive.Mix(Hive.Surface, Hive.HoneyWash, lift * 0.7f));
            Hive.Stroke(g, card, Hive.RadiusCard, Hive.Mix(Hive.Line, Hive.Honey, lift), 1.2f);

            RectangleF hex = new RectangleF(card.X + 16, card.Y + 16, 48, 48);
            Hive.FillHex(g, hex, Hive.Mix(Hive.HoneyWash, Hive.Honey, 0.15f + lift * 0.3f));
            Marks.Draw(g, Glyph, RectangleF.Inflate(hex, -13, -13), Hive.Mix(Hive.Honey, Hive.Ink, 0.35f), 1.7f);

            int left = (int)hex.Right + 16;
            int width = (int)card.Right - left - 34;

            Hive.Text(g, Title, Hive.Heading, new Rectangle(left, (int)card.Y + 16, width, 26), Hive.Ink, Hive.LeftMid);
            Hive.Text(g, Hint, Hive.Caption, new Rectangle(left, (int)card.Y + 42, width, 20), Hive.Muted, Hive.LeftMid);

            Marks.Draw(g, Mark.Chevron, new RectangleF(card.Right - 34, card.Y + card.Height / 2f - 10, 20, 20),
                       Hive.Mix(Hive.Muted, Hive.Teal, lift), 1.8f);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _hover.Dispose();
            base.Dispose(disposing);
        }
    }
}

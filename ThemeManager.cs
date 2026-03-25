using System;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk
{
    /// <summary>
    /// Centralized theme manager for The Hive Cafe kiosk.
    /// Call ThemeManager.ApplyTheme(this) at the END of each Form constructor
    /// (after InitializeComponent()) to apply the cafe aesthetic.
    /// </summary>
    public static class ThemeManager
    {
        // ── Colour Palette ────────────────────────────────────────────────
        public static readonly Color ColorBackground  = Color.FromArgb(255, 248, 235); // warm cream
        public static readonly Color ColorSurface     = Color.FromArgb(255, 255, 252); // near-white card
        public static readonly Color ColorEspresso    = Color.FromArgb(35, 18, 6);     // deepest brown (btn bg)
        public static readonly Color ColorAmber       = Color.FromArgb(193, 118, 18);  // golden accent
        public static readonly Color ColorAmberLight  = Color.FromArgb(230, 165, 60);  // hover / highlight
        public static readonly Color ColorBrown       = Color.FromArgb(100, 58, 18);   // mid brown
        public static readonly Color ColorTextDark    = Color.FromArgb(30, 15, 5);     // near-black text
        public static readonly Color ColorTextMid     = Color.FromArgb(100, 70, 40);   // secondary text
        public static readonly Color ColorTextLight   = Color.FromArgb(255, 248, 235); // text on dark bg
        public static readonly Color ColorBorder      = Color.FromArgb(210, 175, 120); // input border
        public static readonly Color ColorDanger      = Color.FromArgb(180, 50, 40);   // remove / error
        public static readonly Color ColorSuccess     = Color.FromArgb(40, 140, 80);   // confirm / pay

        // ── Fonts ─────────────────────────────────────────────────────────
        public static readonly Font FontBase    = new Font("Segoe UI", 10f, FontStyle.Regular);
        public static readonly Font FontBold    = new Font("Segoe UI", 10f, FontStyle.Bold);
        public static readonly Font FontSmall   = new Font("Segoe UI", 8.5f, FontStyle.Regular);
        public static readonly Font FontLarge   = new Font("Segoe UI", 13f, FontStyle.Bold);
        public static readonly Font FontTitle   = new Font("Segoe UI", 18f, FontStyle.Bold);
        public static readonly Font FontMono    = new Font("Consolas",  10f, FontStyle.Regular);

        // ── Public Entry Point ────────────────────────────────────────────
        /// <summary>
        /// Recursively applies The Hive Cafe theme to a form and all its controls.
        /// </summary>
        public static void ApplyTheme(Form form)
        {
            form.BackColor  = ColorBackground;
            form.ForeColor  = ColorTextDark;
            form.Font       = FontBase;

            // Centre on screen if not already set
            if (form.StartPosition == FormStartPosition.WindowsDefaultLocation)
                form.StartPosition = FormStartPosition.CenterScreen;

            StyleControls(form.Controls);
        }

        // ── Recursive Styler ─────────────────────────────────────────────
        private static void StyleControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                switch (ctrl)
                {
                    case Button btn:
                        StyleButton(btn);
                        break;

                    case Label lbl:
                        StyleLabel(lbl);
                        break;

                    case TextBox txt:
                        StyleTextBox(txt);
                        break;

                    case NumericUpDown nud:
                        StyleNumericUpDown(nud);
                        break;

                    case ComboBox cmb:
                        StyleComboBox(cmb);
                        break;

                    case ListView lv:
                        StyleListView(lv);
                        break;

                    case GroupBox gb:
                        StyleGroupBox(gb);
                        break;

                    case Panel pnl:
                        // Keep panel transparent so form bg shows through
                        pnl.BackColor = Color.Transparent;
                        break;

                    case RadioButton rb:
                        StyleRadioButton(rb);
                        break;

                    case CheckBox cb:
                        cb.ForeColor = ColorTextDark;
                        cb.Font      = FontBase;
                        cb.BackColor = Color.Transparent;
                        break;

                    case PictureBox _:
                        // Leave picture boxes untouched
                        break;
                }

                // Recurse into children
                if (ctrl.HasChildren)
                    StyleControls(ctrl.Controls);
            }
        }

        // ── Individual Control Styles ─────────────────────────────────────

        public static void StyleButton(Button btn, ButtonRole role = ButtonRole.Primary)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font      = FontBold;
            btn.Cursor    = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            btn.TextAlign = ContentAlignment.MiddleCenter;

            switch (role)
            {
                case ButtonRole.Primary:
                    btn.BackColor = ColorEspresso;
                    btn.ForeColor = ColorTextLight;
                    btn.FlatAppearance.MouseOverBackColor = ColorBrown;
                    break;

                case ButtonRole.Accent:
                    btn.BackColor = ColorAmber;
                    btn.ForeColor = ColorTextLight;
                    btn.FlatAppearance.MouseOverBackColor = ColorAmberLight;
                    break;

                case ButtonRole.Success:
                    btn.BackColor = ColorSuccess;
                    btn.ForeColor = ColorTextLight;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 160, 90);
                    break;

                case ButtonRole.Danger:
                    btn.BackColor = ColorDanger;
                    btn.ForeColor = ColorTextLight;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 70, 60);
                    break;

                case ButtonRole.Ghost:
                    btn.BackColor = Color.Transparent;
                    btn.ForeColor = ColorBrown;
                    btn.FlatAppearance.BorderSize  = 1;
                    btn.FlatAppearance.BorderColor = ColorBorder;
                    btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 220, 190);
                    break;
            }
        }

        private static void StyleLabel(Label lbl)
        {
            lbl.ForeColor = ColorTextDark;
            lbl.BackColor = Color.Transparent;

            // Detect "total" labels and make them pop
            string name = lbl.Name.ToLower();
            if (name.Contains("total") || name.Contains("price"))
            {
                lbl.Font      = FontLarge;
                lbl.ForeColor = ColorAmber;
            }
            else if (name.Contains("title") || name.Contains("header"))
            {
                lbl.Font      = FontTitle;
                lbl.ForeColor = ColorEspresso;
            }
        }

        private static void StyleTextBox(TextBox txt)
        {
            txt.BackColor   = ColorSurface;
            txt.ForeColor   = ColorTextDark;
            txt.Font        = FontBase;
            txt.BorderStyle = BorderStyle.FixedSingle;
        }

        private static void StyleNumericUpDown(NumericUpDown nud)
        {
            nud.BackColor = ColorSurface;
            nud.ForeColor = ColorTextDark;
            nud.Font      = FontBold;
        }

        private static void StyleComboBox(ComboBox cmb)
        {
            cmb.BackColor    = ColorSurface;
            cmb.ForeColor    = ColorTextDark;
            cmb.Font         = FontBase;
            cmb.FlatStyle    = FlatStyle.Flat;
        }

        private static void StyleListView(ListView lv)
        {
            lv.BackColor       = ColorSurface;
            lv.ForeColor       = ColorTextDark;
            lv.Font            = FontBase;
            lv.BorderStyle     = BorderStyle.FixedSingle;
            lv.OwnerDraw       = false;
            lv.GridLines       = false;
            lv.FullRowSelect   = true;
        }

        private static void StyleGroupBox(GroupBox gb)
        {
            gb.BackColor = Color.Transparent;
            gb.ForeColor = ColorBrown;
            gb.Font      = FontBold;
        }

        private static void StyleRadioButton(RadioButton rb)
        {
            rb.ForeColor = ColorTextDark;
            rb.BackColor = Color.Transparent;
            rb.Font      = FontBase;
            rb.Cursor    = Cursors.Hand;
        }
    }

    public enum ButtonRole
    {
        Primary,   // Dark espresso – default action
        Accent,    // Golden amber  – highlight / order
        Success,   // Green         – confirm / pay
        Danger,    // Red           – remove / cancel
        Ghost,     // Outlined      – back / secondary
    }
}

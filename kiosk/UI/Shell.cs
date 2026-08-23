using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.UI
{
    /// <summary>Implemented by pages that need to refresh when navigated back to.</summary>
    public interface IPage
    {
        void OnRevealed();
    }

    /// <summary>
    /// The kiosk's only real window. Every screen is a borderless form hosted
    /// inside it, so moving between the menu, payment and the receipt swaps the
    /// contents instead of opening another window.
    /// </summary>
    public class Shell : Form
    {
        private const int WS_EX_COMPOSITED = 0x02000000;

        private readonly Panel _host = new Panel();
        private readonly List<Form> _stack = new List<Form>();

        internal static Shell Current;

        public Shell()
        {
            Current = this;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(Hive.ScreenW, Hive.ScreenH);
            BackColor = Hive.Canvas;
            Font = Hive.Body;
            Text = "The Hive Cafe";
            KeyPreview = true;

            _host.Dock = DockStyle.Fill;
            _host.BackColor = Hive.Canvas;
            Controls.Add(_host);

            Load += (s, e) => Go(new Form1());
        }

        /// <summary>
        /// Paints the whole window bottom-up into one buffer. Without this the
        /// hand-painted controls tear against each other while a list scrolls.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= WS_EX_COMPOSITED;
                return cp;
            }
        }

        private Form Active
        {
            get { return _stack.Count == 0 ? null : _stack[_stack.Count - 1]; }
        }

        /// <summary>Opens a page on top of the current one.</summary>
        public void Go(Form page)
        {
            if (page == null) return;

            Form previous = Active;
            if (previous != null) previous.Visible = false;

            page.TopLevel = false;
            page.FormBorderStyle = FormBorderStyle.None;
            page.Dock = DockStyle.Fill;

            _stack.Add(page);
            _host.Controls.Add(page);
            page.Show();
            page.BringToFront();
        }

        /// <summary>Returns to the previous page, discarding the current one.</summary>
        public void Back()
        {
            if (_stack.Count <= 1) return;

            Form leaving = Active;
            _stack.RemoveAt(_stack.Count - 1);
            _host.Controls.Remove(leaving);
            leaving.Dispose();

            Reveal(Active);
        }

        /// <summary>Unwinds to the welcome screen, ready for the next guest.</summary>
        public void Home()
        {
            while (_stack.Count > 1)
            {
                Form leaving = Active;
                _stack.RemoveAt(_stack.Count - 1);
                _host.Controls.Remove(leaving);
                leaving.Dispose();
            }

            OrderStorage.ClearOrders();
            Reveal(Active);
        }

        private void Reveal(Form page)
        {
            if (page == null) return;

            page.Visible = true;
            page.BringToFront();

            IPage refreshable = page as IPage;
            if (refreshable != null) refreshable.OnRevealed();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Back();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    /// <summary>Navigation, reachable from any page without passing the shell around.</summary>
    public static class Nav
    {
        public static void Go(Form page)
        {
            if (Shell.Current != null) Shell.Current.Go(page);
        }

        public static void Back()
        {
            if (Shell.Current != null) Shell.Current.Back();
        }

        public static void Home()
        {
            if (Shell.Current != null) Shell.Current.Home();
        }
    }
}

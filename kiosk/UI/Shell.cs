using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.UI
{
    public interface IPage
    {
        void OnRevealed();
    }

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

        public void Back()
        {
            if (_stack.Count <= 1) return;

            Form leaving = Active;
            _stack.RemoveAt(_stack.Count - 1);
            _host.Controls.Remove(leaving);
            leaving.Dispose();

            Reveal(Active);
        }

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

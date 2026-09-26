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

    public class Shell : Form, IMessageFilter
    {
                private readonly Timer _idle = new Timer { Interval = 1000 };
        private DateTime _lastInput = DateTime.UtcNow;
        private readonly Label _idleNotice = new Label { Bounds = new Rectangle(0, 0, Hive.ScreenW, 44), BackColor = Hive.HoneyWash, TextAlign = ContentAlignment.MiddleCenter, Visible = false };
        public bool PreFilterMessage(ref Message m)
        {
            if ((m.Msg >= 0x100 && m.Msg <= 0x109) || (m.Msg >= 0x201 && m.Msg <= 0x20E) || m.Msg == 0x245 || m.Msg == 0x246)
            {
                _lastInput = DateTime.UtcNow;
                _idleNotice.Visible = false;
            }
            return false;
        }
        public void Complete(Form receipt)
        {
            Home();
            Go(receipt);
        }
        private void ShowStaffLogin()
        {
            if (!(Active is Form1)) return;
            string pin = Environment.GetEnvironmentVariable("HIVE_STAFF_PIN");
            if (string.IsNullOrWhiteSpace(pin)) { MessageBox.Show("Set HIVE_STAFF_PIN in Windows and restart the kiosk to enable staff access."); return; }
            var login = new Form { BackColor = Hive.Canvas };
            var label = new Label { Text = "Staff PIN", Bounds = new Rectangle(24, 100, 420, 40), Font = Hive.Title };
            var input = new TextBox { UseSystemPasswordChar = true, Bounds = new Rectangle(24, 160, 420, 40), Font = Hive.Title, MaxLength = 64 };
            var enter = new Button { Text = "Unlock", Bounds = new Rectangle(24, 220, 420, 54) };
            int attempts = 0;
            enter.Click += (s, e) => {
                if (input.Text == pin) { Back(); Go(new StaffPage()); }
                else { input.Clear(); label.Text = "Incorrect PIN"; if (++attempts >= 3) Back(); }
            };
            var back = new Button { Text = "Back", Bounds = new Rectangle(24, 290, 420, 54) };
            back.Click += (s, e) => Back();
            login.Controls.AddRange(new Control[] { label, input, enter, back });
            Go(login);
        }
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

                        Controls.Add(_idleNotice);
            Application.AddMessageFilter(this);
            _idle.Tick += (s, e) => {
                if (_stack.Count <= 1) return;
                double elapsed = (DateTime.UtcNow - _lastInput).TotalSeconds;
                if (elapsed >= 120) { Home(); _idleNotice.Visible = false; }
                else if (elapsed >= 90) { _idleNotice.Text = "Still there? Tap to continue. Reset in " + (120 - (int)elapsed) + "s"; _idleNotice.Visible = true; _idleNotice.BringToFront(); }
            };
            _idle.Start();
            Disposed += (s, e) => { _idle.Dispose(); Application.RemoveMessageFilter(this); Current = null; };
            Load += (s, e) => {
                try { LocalStore.LoadAvailability(); }
                catch (Exception ex) { MessageBox.Show("Cannot load availability. Please fix the saved file before taking orders.\n" + ex.Message); Close(); return; }
                Go(new Form1());
            };
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
            _lastInput = DateTime.UtcNow;

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
            GuestText.SetFilipino(false);
            GuestText.SetLargeText(false);
            Reveal(Active);
        }

        private void Reveal(Form page)
        {
            if (page == null) return;
            _lastInput = DateTime.UtcNow;

            page.Visible = true;
            page.BringToFront();

            IPage refreshable = page as IPage;
            if (refreshable != null) refreshable.OnRevealed();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.S)) { ShowStaffLogin(); return true; }
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

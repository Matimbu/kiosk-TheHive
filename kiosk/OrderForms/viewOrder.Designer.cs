namespace kiosk
{
    partial class viewOrder
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.header = new kiosk.UI.AppHeader();
            this.list = new kiosk.UI.ScrollHost();
            this.footer = new System.Windows.Forms.Panel();
            this.btn_menu = new kiosk.UI.HiveButton();
            this.btn_paymentMethod = new kiosk.UI.HiveButton();
            this.footer.SuspendLayout();
            this.SuspendLayout();
            //
            // list
            //
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Name = "list";
            //
            // btn_menu
            //
            this.btn_menu.Location = new System.Drawing.Point(16, 100);
            this.btn_menu.Name = "btn_menu";
            this.btn_menu.Size = new System.Drawing.Size(168, 54);
            this.btn_menu.Style = kiosk.UI.HiveStyle.Outline;
            this.btn_menu.Text = "Add more";
            //
            // btn_paymentMethod
            //
            this.btn_paymentMethod.Location = new System.Drawing.Point(196, 100);
            this.btn_paymentMethod.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right));
            this.btn_paymentMethod.Name = "btn_paymentMethod";
            this.btn_paymentMethod.Size = new System.Drawing.Size(268, 54);
            this.btn_paymentMethod.Style = kiosk.UI.HiveStyle.Accent;
            this.btn_paymentMethod.Text = "Checkout";
            //
            // footer
            //
            this.footer.BackColor = System.Drawing.Color.White;
            this.footer.Controls.Add(this.btn_menu);
            this.footer.Controls.Add(this.btn_paymentMethod);
            this.footer.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.footer.Name = "footer";
            this.footer.Size = new System.Drawing.Size(480, 172);
            //
            // header
            //
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Name = "header";
            this.header.Size = new System.Drawing.Size(480, 92);
            this.header.Title = "Your order";
            //
            // viewOrder
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(480, 720);
            this.Controls.Add(this.list);
            this.Controls.Add(this.footer);
            this.Controls.Add(this.header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "viewOrder";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Your order";
            this.footer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private kiosk.UI.AppHeader header;
        private kiosk.UI.ScrollHost list;
        private System.Windows.Forms.Panel footer;
        private kiosk.UI.HiveButton btn_menu;
        private kiosk.UI.HiveButton btn_paymentMethod;
    }
}

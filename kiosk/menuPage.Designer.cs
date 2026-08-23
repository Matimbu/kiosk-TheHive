namespace kiosk
{
    partial class menuPage
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
            this.rail = new kiosk.UI.ChipRail();
            this.grid = new kiosk.UI.CategoryView();
            this.cartBar = new kiosk.UI.CartBar();
            this.SuspendLayout();
            //
            // grid
            //
            this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid.Name = "grid";
            //
            // cartBar
            //
            this.cartBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.cartBar.Name = "cartBar";
            //
            // rail
            //
            this.rail.Dock = System.Windows.Forms.DockStyle.Top;
            this.rail.Name = "rail";
            //
            // header
            //
            this.header.Dock = System.Windows.Forms.DockStyle.Top;
            this.header.Name = "header";
            this.header.Title = "The Hive Cafe";
            //
            // menuPage
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(480, 720);
            this.Controls.Add(this.grid);
            this.Controls.Add(this.cartBar);
            this.Controls.Add(this.rail);
            this.Controls.Add(this.header);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "menuPage";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Menu";
            this.ResumeLayout(false);
        }

        #endregion

        private kiosk.UI.AppHeader header;
        private kiosk.UI.ChipRail rail;
        private kiosk.UI.CategoryView grid;
        private kiosk.UI.CartBar cartBar;
    }
}

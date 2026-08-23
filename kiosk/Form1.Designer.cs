namespace kiosk
{
    partial class Form1
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
            this.btn_Start = new kiosk.UI.HiveButton();
            this.SuspendLayout();
            //
            // btn_Start
            //
            this.btn_Start.Font = kiosk.UI.Hive.Subhead;
            this.btn_Start.Location = new System.Drawing.Point(90, 556);
            this.btn_Start.Name = "btn_Start";
            this.btn_Start.Size = new System.Drawing.Size(300, 64);
            this.btn_Start.Style = kiosk.UI.HiveStyle.Accent;
            this.btn_Start.TabIndex = 0;
            this.btn_Start.Text = "Start your order";
            this.btn_Start.Click += new System.EventHandler(this.btn_Start_Click);
            //
            // Form1
            //
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(480, 720);
            this.Controls.Add(this.btn_Start);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "The Hive Cafe";
            this.ResumeLayout(false);
        }

        #endregion

        private kiosk.UI.HiveButton btn_Start;
    }
}

using System.Windows.Forms;

namespace kiosk.Payments
{
    partial class ReceiptForm
    {
        private System.ComponentModel.IContainer components = null;
        private TextBox txtReceipt;
        private Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReceiptForm));
            this.txtReceipt = new System.Windows.Forms.TextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtReceipt
            // 
            this.txtReceipt.Font = new System.Drawing.Font("Consolas", 10F);
            this.txtReceipt.Location = new System.Drawing.Point(12, 12);
            this.txtReceipt.Multiline = true;
            this.txtReceipt.Name = "txtReceipt";
            this.txtReceipt.ReadOnly = true;
            this.txtReceipt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtReceipt.Size = new System.Drawing.Size(450, 465);
            this.txtReceipt.TabIndex = 0;
            this.txtReceipt.Text = resources.GetString("txtReceipt.Text");
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(187, 507);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // ReceiptForm
            // 
            this.ClientSize = new System.Drawing.Size(474, 561);
            this.Controls.Add(this.txtReceipt);
            this.Controls.Add(this.btnClose);
            this.Name = "ReceiptForm";
            this.Text = "Receipt";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
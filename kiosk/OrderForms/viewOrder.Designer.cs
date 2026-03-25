namespace kiosk
{
    partial class viewOrder
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxOrders = new System.Windows.Forms.ListView();
            this.button_menu = new System.Windows.Forms.Button();
            this.btn_paymentMethod = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.labelTotal = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // listBoxOrders
            // 
            this.listBoxOrders.HideSelection = false;
            this.listBoxOrders.Location = new System.Drawing.Point(56, 125);
            this.listBoxOrders.Name = "listBoxOrders";
            this.listBoxOrders.Size = new System.Drawing.Size(370, 348);
            this.listBoxOrders.TabIndex = 1;
            this.listBoxOrders.UseCompatibleStateImageBehavior = false;
            // 
            // button_menu
            // 
            this.button_menu.Location = new System.Drawing.Point(203, 547);
            this.button_menu.Name = "button_menu";
            this.button_menu.Size = new System.Drawing.Size(75, 23);
            this.button_menu.TabIndex = 2;
            this.button_menu.Text = "Order More";
            this.button_menu.UseVisualStyleBackColor = true;
            this.button_menu.Click += new System.EventHandler(this.button_menu_Click);
            // 
            // btn_paymentMethod
            // 
            this.btn_paymentMethod.Location = new System.Drawing.Point(183, 589);
            this.btn_paymentMethod.Name = "btn_paymentMethod";
            this.btn_paymentMethod.Size = new System.Drawing.Size(120, 23);
            this.btn_paymentMethod.TabIndex = 3;
            this.btn_paymentMethod.Text = "Proceed to Payment";
            this.btn_paymentMethod.UseVisualStyleBackColor = true;
            this.btn_paymentMethod.Click += new System.EventHandler(this.btn_paymentMethod_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(231, 573);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "-or-";
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.BackColor = System.Drawing.SystemColors.ControlLight;
            this.labelTotal.Font = new System.Drawing.Font("Sylfaen", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal.Location = new System.Drawing.Point(56, 476);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(39, 18);
            this.labelTotal.TabIndex = 8;
            this.labelTotal.Text = "Total:";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::kiosk.Properties.Resources.icon2;
            this.pictureBox1.Location = new System.Drawing.Point(200, 21);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(103, 98);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // viewOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 720);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_paymentMethod);
            this.Controls.Add(this.button_menu);
            this.Controls.Add(this.listBoxOrders);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "viewOrder";
            this.Text = "viewOrder";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView listBoxOrders;
        private System.Windows.Forms.Button button_menu;
        private System.Windows.Forms.Button btn_paymentMethod;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label labelTotal;
    }
}
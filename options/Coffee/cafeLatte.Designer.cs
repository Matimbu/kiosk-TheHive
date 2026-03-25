namespace kiosk
{
    partial class cafeLatte
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.radioCold = new System.Windows.Forms.RadioButton();
            this.radioHot = new System.Windows.Forms.RadioButton();
            this.buttonBack = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.drinkName1 = new System.Windows.Forms.Label();
            this.numericUpDownQuantity = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox_Size = new System.Windows.Forms.GroupBox();
            this.radioSize16L = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.radioSize22L = new System.Windows.Forms.RadioButton();
            this.groupBox_Temp = new System.Windows.Forms.GroupBox();
            this.btnPlaceOrder = new System.Windows.Forms.Button();
            this.labelTotalPrice = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuantity)).BeginInit();
            this.groupBox_Size.SuspendLayout();
            this.groupBox_Temp.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.pictureBox1.Image = global::kiosk.Properties.Resources.Cafe_Latte;
            this.pictureBox1.Location = new System.Drawing.Point(184, 127);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 100);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 26;
            this.pictureBox1.TabStop = false;
            // 
            // radioCold
            // 
            this.radioCold.AutoSize = true;
            this.radioCold.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioCold.Location = new System.Drawing.Point(157, 6);
            this.radioCold.Name = "radioCold";
            this.radioCold.Size = new System.Drawing.Size(46, 17);
            this.radioCold.TabIndex = 23;
            this.radioCold.TabStop = true;
            this.radioCold.Text = "Cold";
            this.radioCold.UseVisualStyleBackColor = true;
            // 
            // radioHot
            // 
            this.radioHot.AutoSize = true;
            this.radioHot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioHot.Location = new System.Drawing.Point(108, 6);
            this.radioHot.Name = "radioHot";
            this.radioHot.Size = new System.Drawing.Size(42, 17);
            this.radioHot.TabIndex = 24;
            this.radioHot.TabStop = true;
            this.radioHot.Text = "Hot";
            this.radioHot.UseVisualStyleBackColor = true;
            // 
            // buttonBack
            // 
            this.buttonBack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonBack.Location = new System.Drawing.Point(238, 460);
            this.buttonBack.Name = "buttonBack";
            this.buttonBack.Size = new System.Drawing.Size(123, 23);
            this.buttonBack.TabIndex = 21;
            this.buttonBack.Text = "Close";
            this.buttonBack.UseVisualStyleBackColor = true;
            this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 8);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 26);
            this.label5.TabIndex = 19;
            this.label5.Text = "diko alam \r\nanong label dapt";
            // 
            // drinkName1
            // 
            this.drinkName1.AutoSize = true;
            this.drinkName1.Location = new System.Drawing.Point(209, 241);
            this.drinkName1.Name = "drinkName1";
            this.drinkName1.Size = new System.Drawing.Size(56, 13);
            this.drinkName1.TabIndex = 20;
            this.drinkName1.Text = "Cafe Latte";
            // 
            // numericUpDownQuantity
            // 
            this.numericUpDownQuantity.Location = new System.Drawing.Point(212, 279);
            this.numericUpDownQuantity.Name = "numericUpDownQuantity";
            this.numericUpDownQuantity.ReadOnly = true;
            this.numericUpDownQuantity.Size = new System.Drawing.Size(32, 20);
            this.numericUpDownQuantity.TabIndex = 16;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(128, 286);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Quantity";
            // 
            // groupBox_Size
            // 
            this.groupBox_Size.Controls.Add(this.radioSize16L);
            this.groupBox_Size.Controls.Add(this.label3);
            this.groupBox_Size.Controls.Add(this.radioSize22L);
            this.groupBox_Size.Location = new System.Drawing.Point(103, 326);
            this.groupBox_Size.Name = "groupBox_Size";
            this.groupBox_Size.Size = new System.Drawing.Size(204, 30);
            this.groupBox_Size.TabIndex = 27;
            this.groupBox_Size.TabStop = false;
            // 
            // radioSize16L
            // 
            this.radioSize16L.AutoSize = true;
            this.radioSize16L.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioSize16L.Location = new System.Drawing.Point(108, 13);
            this.radioSize16L.Name = "radioSize16L";
            this.radioSize16L.Size = new System.Drawing.Size(43, 17);
            this.radioSize16L.TabIndex = 25;
            this.radioSize16L.TabStop = true;
            this.radioSize16L.Text = "16L";
            this.radioSize16L.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 18;
            this.label3.Text = "Size";
            // 
            // radioSize22L
            // 
            this.radioSize22L.AutoSize = true;
            this.radioSize22L.Cursor = System.Windows.Forms.Cursors.Hand;
            this.radioSize22L.Location = new System.Drawing.Point(157, 13);
            this.radioSize22L.Name = "radioSize22L";
            this.radioSize22L.Size = new System.Drawing.Size(43, 17);
            this.radioSize22L.TabIndex = 22;
            this.radioSize22L.TabStop = true;
            this.radioSize22L.Text = "22L";
            this.radioSize22L.UseVisualStyleBackColor = true;
            // 
            // groupBox_Temp
            // 
            this.groupBox_Temp.Controls.Add(this.label5);
            this.groupBox_Temp.Controls.Add(this.radioHot);
            this.groupBox_Temp.Controls.Add(this.radioCold);
            this.groupBox_Temp.Location = new System.Drawing.Point(103, 362);
            this.groupBox_Temp.Name = "groupBox_Temp";
            this.groupBox_Temp.Size = new System.Drawing.Size(204, 37);
            this.groupBox_Temp.TabIndex = 28;
            this.groupBox_Temp.TabStop = false;
            // 
            // btnPlaceOrder
            // 
            this.btnPlaceOrder.Location = new System.Drawing.Point(103, 460);
            this.btnPlaceOrder.Name = "btnPlaceOrder";
            this.btnPlaceOrder.Size = new System.Drawing.Size(123, 23);
            this.btnPlaceOrder.TabIndex = 29;
            this.btnPlaceOrder.Text = "Place Order";
            this.btnPlaceOrder.UseVisualStyleBackColor = true;
            this.btnPlaceOrder.Click += new System.EventHandler(this.btnPlaceOrder_Click);
            // 
            // labelTotalPrice
            // 
            this.labelTotalPrice.AutoSize = true;
            this.labelTotalPrice.Location = new System.Drawing.Point(100, 423);
            this.labelTotalPrice.Name = "labelTotalPrice";
            this.labelTotalPrice.Size = new System.Drawing.Size(65, 13);
            this.labelTotalPrice.TabIndex = 38;
            this.labelTotalPrice.Text = "Price: ₱0.00";
            // 
            // cafeLatte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(480, 720);
            this.Controls.Add(this.labelTotalPrice);
            this.Controls.Add(this.btnPlaceOrder);
            this.Controls.Add(this.groupBox_Temp);
            this.Controls.Add(this.groupBox_Size);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.buttonBack);
            this.Controls.Add(this.drinkName1);
            this.Controls.Add(this.numericUpDownQuantity);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "cafeLatte";
            this.Text = "countForm";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuantity)).EndInit();
            this.groupBox_Size.ResumeLayout(false);
            this.groupBox_Size.PerformLayout();
            this.groupBox_Temp.ResumeLayout(false);
            this.groupBox_Temp.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.RadioButton radioCold;
        private System.Windows.Forms.RadioButton radioHot;
        private System.Windows.Forms.Button buttonBack;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label drinkName1;
        
        private System.Windows.Forms.NumericUpDown numericUpDownQuantity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox_Size;
        private System.Windows.Forms.RadioButton radioSize16L;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioSize22L;
        private System.Windows.Forms.GroupBox groupBox_Temp;
        private System.Windows.Forms.Button btnPlaceOrder;
        private System.Windows.Forms.Label labelTotalPrice;
    }
}
using System;

namespace kiosk
{
    partial class EditOrderForm
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label txtProduct;
        private System.Windows.Forms.Label lblPrice;
        private System.Windows.Forms.Label lblQuantity;

        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtProduct = new System.Windows.Forms.Label();
            this.lblPrice = new System.Windows.Forms.Label();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.numericUpDown_Quantity = new System.Windows.Forms.NumericUpDown();
            this.radio_L16 = new System.Windows.Forms.RadioButton();
            this.radio_L22 = new System.Windows.Forms.RadioButton();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label_TotalPrice = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.radio_Hot = new System.Windows.Forms.RadioButton();
            this.radio_Iced = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Quantity)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtProduct
            // 
            this.txtProduct.AutoSize = true;
            this.txtProduct.Location = new System.Drawing.Point(135, 20);
            this.txtProduct.Name = "txtProduct";
            this.txtProduct.Size = new System.Drawing.Size(47, 13);
            this.txtProduct.TabIndex = 0;
            this.txtProduct.Text = "Product:";
            // 
            // lblPrice
            // 
            this.lblPrice.AutoSize = true;
            this.lblPrice.Location = new System.Drawing.Point(23, 84);
            this.lblPrice.Name = "lblPrice";
            this.lblPrice.Size = new System.Drawing.Size(27, 13);
            this.lblPrice.TabIndex = 2;
            this.lblPrice.Text = "Size";
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(23, 46);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(49, 13);
            this.lblQuantity.TabIndex = 4;
            this.lblQuantity.Text = "Quantity:";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(26, 181);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(80, 25);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(213, 181);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 25);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // numericUpDown_Quantity
            // 
            this.numericUpDown_Quantity.Location = new System.Drawing.Point(104, 44);
            this.numericUpDown_Quantity.Name = "numericUpDown_Quantity";
            this.numericUpDown_Quantity.ReadOnly = true;
            this.numericUpDown_Quantity.Size = new System.Drawing.Size(42, 20);
            this.numericUpDown_Quantity.TabIndex = 9;
            this.numericUpDown_Quantity.ValueChanged += new System.EventHandler(this.numericUpDown_Quantity_ValueChanged);
            // 
            // radio_L16
            // 
            this.radio_L16.AutoSize = true;
            this.radio_L16.Location = new System.Drawing.Point(104, 79);
            this.radio_L16.Name = "radio_L16";
            this.radio_L16.Size = new System.Drawing.Size(43, 17);
            this.radio_L16.TabIndex = 10;
            this.radio_L16.TabStop = true;
            this.radio_L16.Text = "L16";
            this.radio_L16.UseVisualStyleBackColor = true;
            this.radio_L16.CheckedChanged += new System.EventHandler(this.radio_L16_CheckedChanged);
            // 
            // radio_L22
            // 
            this.radio_L22.AutoSize = true;
            this.radio_L22.Location = new System.Drawing.Point(154, 79);
            this.radio_L22.Name = "radio_L22";
            this.radio_L22.Size = new System.Drawing.Size(43, 17);
            this.radio_L22.TabIndex = 11;
            this.radio_L22.TabStop = true;
            this.radio_L22.Text = "L22";
            this.radio_L22.UseVisualStyleBackColor = true;
            this.radio_L22.CheckedChanged += new System.EventHandler(this.radio_L22_CheckedChanged);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(122, 181);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 12;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label_TotalPrice
            // 
            this.label_TotalPrice.AutoSize = true;
            this.label_TotalPrice.Location = new System.Drawing.Point(23, 147);
            this.label_TotalPrice.Name = "label_TotalPrice";
            this.label_TotalPrice.Size = new System.Drawing.Size(37, 13);
            this.label_TotalPrice.TabIndex = 13;
            this.label_TotalPrice.Text = "Total: ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 84);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(0, 13);
            this.label1.TabIndex = 14;
            // 
            // radio_Hot
            // 
            this.radio_Hot.AutoSize = true;
            this.radio_Hot.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radio_Hot.Location = new System.Drawing.Point(8, 13);
            this.radio_Hot.Name = "radio_Hot";
            this.radio_Hot.Size = new System.Drawing.Size(45, 19);
            this.radio_Hot.TabIndex = 15;
            this.radio_Hot.TabStop = true;
            this.radio_Hot.Text = "Hot";
            this.radio_Hot.UseVisualStyleBackColor = true;
            this.radio_Hot.CheckedChanged += new System.EventHandler(this.radioh_CheckedChanged);
            // 
            // radio_Iced
            // 
            this.radio_Iced.AutoSize = true;
            this.radio_Iced.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radio_Iced.Location = new System.Drawing.Point(58, 13);
            this.radio_Iced.Name = "radio_Iced";
            this.radio_Iced.Size = new System.Drawing.Size(48, 19);
            this.radio_Iced.TabIndex = 15;
            this.radio_Iced.TabStop = true;
            this.radio_Iced.Text = "Iced";
            this.radio_Iced.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupBox1.Controls.Add(this.radio_Hot);
            this.groupBox1.Controls.Add(this.radio_Iced);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Location = new System.Drawing.Point(95, 102);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(113, 36);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            // 
            // EditOrderForm
            // 
            this.ClientSize = new System.Drawing.Size(320, 226);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label_TotalPrice);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.radio_L22);
            this.Controls.Add(this.radio_L16);
            this.Controls.Add(this.numericUpDown_Quantity);
            this.Controls.Add(this.txtProduct);
            this.Controls.Add(this.lblPrice);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox1);
            this.Name = "EditOrderForm";
            this.Text = "Edit Order";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown_Quantity)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private System.Windows.Forms.NumericUpDown numericUpDown_Quantity;
        private System.Windows.Forms.RadioButton radio_L16;
        private System.Windows.Forms.RadioButton radio_L22;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label_TotalPrice;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton radio_Hot;
        private System.Windows.Forms.RadioButton radio_Iced;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

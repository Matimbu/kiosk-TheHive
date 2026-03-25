namespace kiosk.Payments
{
    partial class PaymentSelectionForm
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
            this.btn_cash = new System.Windows.Forms.Button();
            this.btn_eWallet = new System.Windows.Forms.Button();
            this.btn_card = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btn_back = new System.Windows.Forms.Button();
            this.labelTotal = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_cash
            // 
            this.btn_cash.Location = new System.Drawing.Point(191, 281);
            this.btn_cash.Name = "btn_cash";
            this.btn_cash.Size = new System.Drawing.Size(75, 23);
            this.btn_cash.TabIndex = 0;
            this.btn_cash.Text = "Cash";
            this.btn_cash.UseVisualStyleBackColor = true;
            this.btn_cash.Click += new System.EventHandler(this.btn_cash_Click);
            // 
            // btn_eWallet
            // 
            this.btn_eWallet.Location = new System.Drawing.Point(110, 357);
            this.btn_eWallet.Name = "btn_eWallet";
            this.btn_eWallet.Size = new System.Drawing.Size(75, 23);
            this.btn_eWallet.TabIndex = 1;
            this.btn_eWallet.Text = "E-Wallet";
            this.btn_eWallet.UseVisualStyleBackColor = true;
            this.btn_eWallet.Click += new System.EventHandler(this.btn_eWallet_Click);
            // 
            // btn_card
            // 
            this.btn_card.Location = new System.Drawing.Point(266, 357);
            this.btn_card.Name = "btn_card";
            this.btn_card.Size = new System.Drawing.Size(75, 23);
            this.btn_card.TabIndex = 2;
            this.btn_card.Text = "Card";
            this.btn_card.UseVisualStyleBackColor = true;
            this.btn_card.Click += new System.EventHandler(this.btn_card_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(211, 362);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "-- or --";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(70, 318);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(331, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "______________________________________________________";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(88, 169);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(287, 25);
            this.label3.TabIndex = 5;
            this.label3.Text = "Choose your payment option";
            // 
            // btn_back
            // 
            this.btn_back.Location = new System.Drawing.Point(37, 636);
            this.btn_back.Name = "btn_back";
            this.btn_back.Size = new System.Drawing.Size(75, 23);
            this.btn_back.TabIndex = 6;
            this.btn_back.Text = "Back";
            this.btn_back.UseVisualStyleBackColor = true;
            this.btn_back.Click += new System.EventHandler(this.btn_back_Click);
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.BackColor = System.Drawing.SystemColors.ControlLight;
            this.labelTotal.Font = new System.Drawing.Font("Sylfaen", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelTotal.Location = new System.Drawing.Point(368, 638);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(80, 18);
            this.labelTotal.TabIndex = 9;
            this.labelTotal.Text = "Total: 555.00";
            // 
            // PaymentSelectionForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 681);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.btn_back);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_card);
            this.Controls.Add(this.btn_eWallet);
            this.Controls.Add(this.btn_cash);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "PaymentSelectionForm";
            this.Text = "selectingPaymentMethod";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_cash;
        private System.Windows.Forms.Button btn_eWallet;
        private System.Windows.Forms.Button btn_card;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btn_back;
        private System.Windows.Forms.Label labelTotal;
    }
}
using System.IO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kiosk.Payments
{
    public partial class ReceiptForm: Form
    {
        private readonly List<Order> cachedOrders;
        private readonly string _paymentMethod;
        private readonly string _fullCardNumber;

        public ReceiptForm(List<Order> ordersToUse, string paymentMethod, string fullCardNumber)
        {
            InitializeComponent();
            cachedOrders = ordersToUse.ToList();
            _paymentMethod = paymentMethod;
            _fullCardNumber = fullCardNumber;
            GenerateReceipt();

            TextBox hiddenDummy = new TextBox();
            hiddenDummy.Visible = false;
            hiddenDummy.TabStop = false;
            this.Controls.Add(hiddenDummy);

            this.Load += (s, e) => { this.ActiveControl = hiddenDummy; };
        }
        private void GenerateReceipt()
        {
            var orders = cachedOrders;
            StringBuilder receipt = new StringBuilder();

            const int totalWidth = 60; 
            const int itemWidth = 35;
            const int qtyWidth = 5;
            const int priceWidth = 9;
            const int subtotalWidth = 10;

            string CenterText(string text)
            {
                int padding = (totalWidth - text.Length) / 2;
                return new string(' ', Math.Max(0, padding)) + text;
            }

            receipt.AppendLine(CenterText("THE HIVE CAFE"));
            receipt.AppendLine(CenterText("--- Payment Receipt ---"));
            receipt.AppendLine(CenterText($"Date: {DateTime.Now}"));
            receipt.AppendLine(new string('-', totalWidth));

            // header nito--------------------------------------------------------------------------------------------------------------------
            receipt.AppendLine(string.Format("{0,-" + itemWidth + "}{1," + qtyWidth + "}{2," + priceWidth + "}{3," + subtotalWidth + "}",
                "Item", "Qty", "Price", "Subtotal"));
            receipt.AppendLine(new string('-', totalWidth));
            // Menu Bar-----------------------------------------------------------------------------------------------------------------------


            decimal total = 0;

            foreach (var order in orders)
            {
                decimal subtotal = order.Quantity * order.Price;

                string itemLabel = $"{order.Product} ({order.SizeTemp})";
                if (itemLabel.Length > itemWidth)
                    itemLabel = itemLabel.Substring(0, itemWidth - 1) + "…";

                receipt.AppendLine(string.Format("{0,-" + itemWidth + "}{1," + qtyWidth + "}{2," + priceWidth + ":₱0.00}{3," + subtotalWidth + ":₱0.00}",
                    itemLabel,
                    order.Quantity,
                    order.Price,
                    subtotal
                ));


                total += subtotal;
            }

            receipt.AppendLine(new string('-', totalWidth));

            // total -------------------------------------------------------------------------------------------------------------------------
            

            string paymentDisplay = _paymentMethod;
            receipt.AppendLine($"Mode of Payment: {paymentDisplay}");

            if (_paymentMethod == "Card" && !string.IsNullOrEmpty(_fullCardNumber))
            {
                string last4 = _fullCardNumber.Length >= 4 ? _fullCardNumber.Substring(_fullCardNumber.Length - 4) : "****";
                string cardType = GetCardType(_fullCardNumber);
                receipt.AppendLine($"Card Number: **** **** **** {last4}");
                receipt.AppendLine($"Card Type: {cardType}");
            }

            receipt.AppendLine(string.Format("{0,60}", $"Total: ₱{total:0.00}"));



            
            receipt.AppendLine(" \n ");
            receipt.AppendLine(CenterText("Thank you for your purchase!"));

            txtReceipt.Font = new Font("Consolas", 10);
            txtReceipt.Text = receipt.ToString();

            // save resibo
            try
            {
                string folderPath = Path.Combine(Application.StartupPath, "Receipts");
                Directory.CreateDirectory(folderPath); // C:\Users\source\repos\kiosk\kiosk\bin\Debug\Receipts - for example

                string fileName = $"receipt_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
                string filePath = Path.Combine(folderPath, fileName);

                File.WriteAllText(filePath, receipt.ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save receipt.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        private string GetCardType(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber) || cardNumber.Length < 4)
                return "Unknown";

            if (System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^4")) return "Visa";
            if (System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^5[1-5]")) return "MasterCard";
            if (System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^3[47]")) return "American Express";
            if (System.Text.RegularExpressions.Regex.IsMatch(cardNumber, @"^6(?:011|5)")) return "Discover";

            return "Other";
        }

        private void btnClose_Click(object sender, EventArgs e) => Application.Restart();
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kiosk
{
    public class Order
    {
        public string Product { get; set; }
        public string Size { get; set; }
        public string Temperature { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string ImagePath { get; set; }
        public string SizeTemp => $"{Size} - {Temperature}";
        public decimal Total => Price * Quantity;


        public override string ToString()
        {
            return $"{Product} ({SizeTemp}), x{Quantity}, ₱{Total:F2}";
        }
    }
}

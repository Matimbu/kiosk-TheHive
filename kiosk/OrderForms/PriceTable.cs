using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kiosk
{
    public static class PriceTable
    {
        public static decimal GetPrice(string product, string size)
        {
            switch (product)
            {
                case "Cafe Latte":
                    return size == "16L" ? 50m : 75m;
                case "Americano":
                    return size == "16L" ? 85m : 115m;
                // Add other drinks here
                case "Hazelnut Americano":
                    return size == "16L" ? 100m : 135m;


                default:
                    return 0m;
            }
        }
    }
}

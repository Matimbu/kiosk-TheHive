using kiosk.UI;

namespace kiosk
{
    public static class PriceTable
    {
        public static decimal GetPrice(string product, string size)
        {
            return MenuCatalog.PriceOf(product, size);
        }
    }
}

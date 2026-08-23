using kiosk.UI;

namespace kiosk
{
    /// <summary>
    /// Kept for compatibility with older call sites. Prices themselves live in
    /// <see cref="MenuCatalog"/> so the menu and the till can never disagree.
    /// </summary>
    public static class PriceTable
    {
        public static decimal GetPrice(string product, string size)
        {
            return MenuCatalog.PriceOf(product, size);
        }
    }
}

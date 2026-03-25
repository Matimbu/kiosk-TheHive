using System;
using System.Collections.Generic;

namespace kiosk
{
    /// <summary>
    /// Central price look-up table for all menu items.
    /// Update prices here — they flow through to every form automatically.
    /// </summary>
    public static class PriceTable
    {
        // ── Coffee (size: "16L" | "22L") ─────────────────────────────────
        private static readonly Dictionary<string, (decimal Small, decimal Large)> _coffeePrices
            = new Dictionary<string, (decimal, decimal)>(StringComparer.OrdinalIgnoreCase)
        {
            { "Cafe Latte",           (50m,   75m)  },
            { "Americano",            (85m,  115m)  },
            { "Hazelnut Americano",   (100m, 135m)  },
            { "Caramel Macchiato",    (95m,  130m)  },
            { "Cafe Tiramisu",        (110m, 145m)  },
            { "Cream Cafe Mocha",     (90m,  120m)  },
            { "Biscoff Latte",        (105m, 140m)  },
        };

        // ── Rice Meals (no size — fixed price) ───────────────────────────
        private static readonly Dictionary<string, decimal> _ricePrices
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "Beef Tapa",            250m },
            { "Caramelized Chicken",  200m },
            { "Ham & Egg",            180m },
            { "Hungarian",            175m },
            { "Baked Bangus",         220m },
            { "Giant Pork Tonkatsu",  280m },
            { "Garlic Longanisa",     185m },
        };

        // ── Snacks (no size — fixed price) ───────────────────────────────
        private static readonly Dictionary<string, decimal> _snackPrices
            = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase)
        {
            { "Waffle",               80m  },
        };

        // ── Public API ────────────────────────────────────────────────────

        /// <summary>
        /// Returns the unit price for a menu item.
        /// For coffee, pass size as "16L" or "22L".
        /// For rice/other items, size can be null or empty.
        /// </summary>
        public static decimal GetPrice(string product, string size = null)
        {
            if (string.IsNullOrWhiteSpace(product))
                return 0m;

            if (_coffeePrices.TryGetValue(product, out var prices))
                return size == "16L" ? prices.Small : prices.Large;

            if (_ricePrices.TryGetValue(product, out decimal ricePrice))
                return ricePrice;

            if (_snackPrices.TryGetValue(product, out decimal snackPrice))
                return snackPrice;

            return 0m;
        }

        /// <summary>Returns true if the product has a size option (coffee drinks).</summary>
        public static bool HasSize(string product)
            => _coffeePrices.ContainsKey(product);

        /// <summary>Returns all coffee drink names.</summary>
        public static IEnumerable<string> GetCoffeeDrinks()
            => _coffeePrices.Keys;

        /// <summary>Returns all rice meal names.</summary>
        public static IEnumerable<string> GetRiceMeals()
            => _ricePrices.Keys;
    }
}

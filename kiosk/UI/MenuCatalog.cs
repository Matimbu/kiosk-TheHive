using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace kiosk.UI
{
    /// <summary>One size a drink can be ordered in.</summary>
    public sealed class SizeOption
    {
        public SizeOption(string code, string label, decimal price)
        {
            Code = code;
            Label = label;
            Price = price;
        }

        /// <summary>Stored on the order (kept short for the receipt).</summary>
        public string Code { get; private set; }

        /// <summary>Shown to the guest.</summary>
        public string Label { get; private set; }

        public decimal Price { get; private set; }
    }

    /// <summary>A single sellable item on the menu.</summary>
    public sealed class MenuProduct
    {
        private Image _image;

        public string Name { get; set; }
        public string Category { get; set; }
        public string Description { get; set; }
        public string Badge { get; set; }
        public string ImageKey { get; set; }

        /// <summary>
        /// True for cut-out product shots on a white background, which have to
        /// be shown whole; false for full-bleed food photography, which looks
        /// better cropped to fill the tile.
        /// </summary>
        public bool ShowWhole { get; set; }

        public decimal BasePrice { get; set; }
        public SizeOption[] Sizes { get; set; }
        public bool HasTemperature { get; set; }

        public bool HasSizes { get { return Sizes != null && Sizes.Length > 0; } }

        /// <summary>Cheapest price, which is what the tile advertises.</summary>
        public decimal StartingPrice
        {
            get { return HasSizes ? Sizes.Min(s => s.Price) : BasePrice; }
        }

        public string PriceLabel
        {
            get { return HasSizes ? "from " + Hive.MoneyShort(StartingPrice) : Hive.MoneyShort(BasePrice); }
        }

        public decimal PriceFor(string sizeCode)
        {
            if (!HasSizes) return BasePrice;
            SizeOption match = Sizes.FirstOrDefault(s => s.Code == sizeCode);
            return match != null ? match.Price : Sizes[0].Price;
        }

        public Image Image
        {
            get
            {
                if (_image == null && !string.IsNullOrEmpty(ImageKey))
                    _image = MenuCatalog.LoadImage(ImageKey);
                return _image;
            }
        }
    }

    /// <summary>
    /// Who the receipt says the order was bought from. Fill these in with the
    /// branch's real details before the kiosk goes live.
    /// </summary>
    public static class CafeInfo
    {
        public const string Name     = "THE HIVE CAFE";
        public const string Branch   = "MCF Lifestyle Hub";
        public const string Address  = "Malolos, Bulacan";
        public const string Contact  = "fb.com/thehivecafe";
        public const string Terminal = "KIOSK-01";

        /// <summary>
        /// Registered taxpayer number. Left blank on purpose - put the real one
        /// here rather than anything invented.
        /// </summary>
        public const string TaxId = "";

        /// <summary>
        /// Only set this true once the kiosk is actually BIR-accredited to
        /// issue official receipts. Until then every receipt is stamped as not
        /// being one, which is what a non-accredited till must print.
        /// </summary>
        public const bool IssuesOfficialReceipts = false;
    }

    /// <summary>A tab in the category rail.</summary>
    public sealed class MenuCategory
    {
        public MenuCategory(string name, string tagline)
        {
            Name = name;
            Tagline = tagline;
        }

        public string Name { get; private set; }
        public string Tagline { get; private set; }
    }

    /// <summary>
    /// The single source of truth for what The Hive Cafe sells.
    /// Add a product here and it appears on the menu, prices correctly, and
    /// shows up on the receipt - no new forms required.
    /// </summary>
    public static class MenuCatalog
    {
        private static readonly Dictionary<string, Image> ImageCache = new Dictionary<string, Image>();

        // ---- Sizes ---------------------------------------------------------
        // Codes stay as "16L" / "22L" so previously placed orders and the
        // receipt printer keep working; only the labels are guest-facing.
        private static SizeOption[] Cups(decimal regular, decimal large)
        {
            return new[]
            {
                new SizeOption("16L", "16 oz", regular),
                new SizeOption("22L", "22 oz", large)
            };
        }

        public static readonly MenuCategory[] Categories =
        {
            new MenuCategory("Best Sellers", "What everyone orders"),
            new MenuCategory("Coffee",       "Pulled fresh all day"),
            new MenuCategory("Non-Coffee",   "Easy on the caffeine"),
            new MenuCategory("Classics",     "Thé Hive milk tea series"),
            new MenuCategory("Cheesecake",   "Thé Hive cheesecake series"),
            new MenuCategory("GentleTea",    "Thé Hive fruit tea series"),
            new MenuCategory("Rice Meals",   "Served hot, all day")
        };

        // ---- Products ------------------------------------------------------
        // Drink names and prices are transcribed from The Hive Cafe's printed
        // menu: the two columns there are 22oz and 16oz, so Cups(16oz, 22oz).
        // Edit a price here and the menu, the cart and the receipt all follow.
        private static readonly MenuProduct[] Products =
        {
            // ---- Coffee ----
            new MenuProduct
            {
                Name = "Americano", Category = "Coffee", ImageKey = "fpAmericano", ShowWhole = true,
                Description = "Espresso over filtered water.",
                Sizes = Cups(20m, 45m), HasTemperature = true, Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Hazelnut Americano", Category = "Coffee", ImageKey = "fpHazelnutAmericano", ShowWhole = true,
                Description = "Americano rounded out with toasted hazelnut.",
                Sizes = Cups(50m, 75m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Cafe Latte", Category = "Coffee", ImageKey = "fpCafeLatte", ShowWhole = true,
                Description = "Espresso with steamed milk and a thin veil of foam.",
                Sizes = Cups(50m, 75m), HasTemperature = true, Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Macadamia Nut Cafe Latte", Category = "Coffee", ImageKey = "omMacadamia",
                Description = "Cafe latte with buttery macadamia.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Irish Cream Cafe Latte", Category = "Coffee", ImageKey = "fpIrishCream", ShowWhole = true,
                Description = "Cafe latte with Irish cream syrup.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Spanish Latte", Category = "Coffee", ImageKey = "fpSpanishLatte", ShowWhole = true,
                Description = "Espresso and milk sweetened the Spanish way.",
                Sizes = Cups(50m, 75m), HasTemperature = true, Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Caramel Macchiato", Category = "Coffee", ImageKey = "fpCaramelMacchiato", ShowWhole = true,
                Description = "Vanilla, steamed milk, espresso and a caramel finish.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Cafe Tiramisu", Category = "Coffee",
                Description = "Mascarpone and cocoa over espresso.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Cafe Cream Mocha", Category = "Coffee", ImageKey = "fpCreamMocha", ShowWhole = true,
                Description = "Chocolate and espresso under a cap of cream.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Cafe Cream Berry", Category = "Coffee", ImageKey = "fpCreamBerry", ShowWhole = true,
                Description = "Espresso, cream and mixed berries.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Biscoff Cafe Latte", Category = "Coffee",
                Description = "Spiced biscuit syrup, milk and espresso.",
                Sizes = Cups(55m, 85m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Cinnamon Oatmilk Latte", Category = "Coffee", ImageKey = "fpCinnamonLatte", ShowWhole = true,
                Description = "Espresso and oat milk finished with cinnamon.",
                Sizes = Cups(60m, 95m), HasTemperature = true
            },

            // ---- Non-coffee ----
            new MenuProduct
            {
                Name = "Fraise Strawberry", Category = "Non-Coffee", ImageKey = "fpFraise", ShowWhole = true,
                Description = "Strawberry cream over ice.",
                Sizes = Cups(55m, 85m)
            },
            new MenuProduct
            {
                Name = "Traditional Matcha", Category = "Non-Coffee", ImageKey = "fpMatchaLatte", ShowWhole = true,
                Description = "Stone-ground matcha with milk.",
                Sizes = Cups(55m, 85m), Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Seasalt Matcha", Category = "Non-Coffee",
                Description = "Matcha under a salted cream cap.",
                Sizes = Cups(60m, 90m)
            },
            new MenuProduct
            {
                Name = "Banana Milk Matcha", Category = "Non-Coffee",
                Description = "Matcha layered over banana milk.",
                Sizes = Cups(60m, 90m)
            },
            new MenuProduct
            {
                Name = "Matcha Strawberry Latte", Category = "Non-Coffee",
                Description = "Matcha and strawberry with milk.",
                Sizes = Cups(60m, 90m)
            },
            new MenuProduct
            {
                Name = "Matcha Mango Latte", Category = "Non-Coffee",
                Description = "Matcha and ripe mango with milk.",
                Sizes = Cups(60m, 90m)
            },
            new MenuProduct
            {
                Name = "Belgian Cocoa", Category = "Non-Coffee", ImageKey = "fpBelgianCocoa", ShowWhole = true,
                Description = "Belgian chocolate, hot or iced.",
                Sizes = Cups(60m, 90m), HasTemperature = true
            },
            new MenuProduct
            {
                Name = "Passion Peach Fizz", Category = "Non-Coffee",
                Description = "Passion fruit and peach, sparkling.",
                Sizes = Cups(50m, 75m)
            },
            new MenuProduct
            {
                Name = "Green Apple Fizz", Category = "Non-Coffee",
                Description = "Green apple soda over ice.",
                Sizes = Cups(50m, 75m)
            },

            // ---- The Hive Classics: milk tea, one price ----
            new MenuProduct
            {
                Name = "Okinawa Brown Sugar Milk Tea", Category = "Classics", ImageKey = "fpOkinawa", ShowWhole = true,
                Description = "Black tea with Okinawa brown sugar.",
                BasePrice = 75m, Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Dark Choco Milk Tea", Category = "Classics", ImageKey = "fpDarkChoco", ShowWhole = true,
                Description = "Dark chocolate milk tea.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Pearl Milk Tea", Category = "Classics", ImageKey = "fpPearl", ShowWhole = true,
                Description = "The house milk tea with pearls.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Matcha Milk Tea", Category = "Classics", ImageKey = "omMatchaMT",
                Description = "Matcha blended into milk tea.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Winter Melon Milk Tea", Category = "Classics", ImageKey = "fpWinterMelon", ShowWhole = true,
                Description = "Winter melon with milk tea.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Thai Milk Tea", Category = "Classics",
                Description = "Spiced Thai tea with milk.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Hokkaido Milk Tea", Category = "Classics", ImageKey = "omHokkaido",
                Description = "Hokkaido-style creamy milk tea.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Taro Milk Tea", Category = "Classics", ImageKey = "fpTaro", ShowWhole = true,
                Description = "Taro root with milk tea.", BasePrice = 75m
            },

            // ---- The Hive Cheesecake series ----
            new MenuProduct
            {
                Name = "Choco Malt", Category = "Cheesecake", ImageKey = "fpChocoMalt", ShowWhole = true,
                Description = "Chocolate malt with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Berry Malt", Category = "Cheesecake", ImageKey = "fpBerryMalt", ShowWhole = true,
                Description = "Mixed berry malt with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Banana Malt", Category = "Cheesecake", ImageKey = "fpBananaMalt", ShowWhole = true,
                Description = "Banana malt with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Berry Banana", Category = "Cheesecake", ImageKey = "fpBerryBanana", ShowWhole = true,
                Description = "Berries and banana with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Choco Hazelnut", Category = "Cheesecake", ImageKey = "fpChocoHazelnut", ShowWhole = true,
                Description = "Chocolate and hazelnut with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Red Velvet", Category = "Cheesecake", ImageKey = "fpRedVelvet", ShowWhole = true,
                Description = "Red velvet with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Cookies & Cream", Category = "Cheesecake", ImageKey = "fpCookiesCream", ShowWhole = true,
                Description = "Crushed cookies with cheesecake cream.", BasePrice = 90m
            },
            new MenuProduct
            {
                Name = "Matcha Cheesecake", Category = "Cheesecake", ImageKey = "fpUjiMatcha", ShowWhole = true,
                Description = "Matcha with cheesecake cream.", BasePrice = 90m
            },

            // ---- The Hive GentleTea series ----
            new MenuProduct
            {
                Name = "Greentea Appleade", Category = "GentleTea", ImageKey = "fpAppleade", ShowWhole = true,
                Description = "Green tea with apple, sparkling.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Passion Fruit", Category = "GentleTea", ImageKey = "fpPassionFruit", ShowWhole = true,
                Description = "Passion fruit fruit tea.", BasePrice = 75m
            },
            new MenuProduct
            {
                Name = "Pomegranate Bliss", Category = "GentleTea", ImageKey = "omPomegranate",
                Description = "Pomegranate fruit tea.", BasePrice = 75m
            },

            // ---- Rice meals ----
            // NOTE: the printed menu we have covers drinks only, so these
            // prices are placeholders. Replace them with the real ones.
            new MenuProduct
            {
                Name = "Beef Tapa", Category = "Rice Meals", ImageKey = "beefTapa",
                Description = "Cured beef, garlic rice, egg and fresh tomato.",
                BasePrice = 145m, Badge = "Best seller"
            },
            new MenuProduct
            {
                Name = "Baked Bangus", Category = "Rice Meals", ImageKey = "bakedBangus",
                Description = "Boneless milkfish baked with herbs and butter.",
                BasePrice = 165m
            },
            new MenuProduct
            {
                Name = "Caramelized Chicken", Category = "Rice Meals", ImageKey = "caramelizedChicken",
                Description = "Glazed chicken thigh with steamed rice and egg.",
                BasePrice = 155m
            },
            new MenuProduct
            {
                Name = "Giant Pork Tonkatsu", Category = "Rice Meals", ImageKey = "giantPorkTonkatsu",
                Description = "Breaded pork cutlet under house katsu sauce.",
                BasePrice = 185m
            },
            new MenuProduct
            {
                Name = "Ham & Egg", Category = "Rice Meals", ImageKey = "hamEgg",
                Description = "Grilled ham, two eggs and toasted garlic rice.",
                BasePrice = 130m
            },
            new MenuProduct
            {
                Name = "Hungarian", Category = "Rice Meals", ImageKey = "hungarian",
                Description = "Hungarian sausage with rice, egg and pickles.",
                BasePrice = 160m
            }
        };

        // ---- Queries -------------------------------------------------------

        public static IEnumerable<MenuProduct> InCategory(string category)
        {
            if (string.Equals(category, "Best Sellers", StringComparison.OrdinalIgnoreCase))
                return Products.Where(p => p.Badge != null);

            return Products.Where(p => string.Equals(p.Category, category, StringComparison.OrdinalIgnoreCase));
        }

        public static MenuProduct Find(string name)
        {
            return Products.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        public static MenuCategory CategoryOf(string name)
        {
            return Categories.FirstOrDefault(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>Price lookup used by the order editor and the receipt.</summary>
        public static decimal PriceOf(string product, string sizeCode)
        {
            MenuProduct match = Find(product);
            return match == null ? 0m : match.PriceFor(sizeCode);
        }

        /// <summary>Turns a stored size code such as "16L" into "16 oz".</summary>
        public static string SizeLabel(string product, string sizeCode)
        {
            if (string.IsNullOrWhiteSpace(sizeCode)) return null;

            MenuProduct match = Find(product);
            if (match != null && match.HasSizes)
            {
                SizeOption option = match.Sizes.FirstOrDefault(s => s.Code == sizeCode);
                if (option != null) return option.Label;
            }
            return sizeCode;
        }

        public static Image ImageFor(string productName)
        {
            MenuProduct match = Find(productName);
            return match == null ? null : match.Image;
        }

        /// <summary>Pulls artwork out of Properties.Resources by name, once.</summary>
        public static Image LoadImage(string key)
        {
            if (string.IsNullOrEmpty(key)) return null;

            Image cached;
            if (ImageCache.TryGetValue(key, out cached)) return cached;

            Image loaded = null;
            try
            {
                loaded = Properties.Resources.ResourceManager.GetObject(key) as Image;
            }
            catch (Exception)
            {
                loaded = null;   // a missing photo falls back to the monogram tile
            }

            ImageCache[key] = loaded;
            return loaded;
        }

        /// <summary>The hive mark used in headers and on the welcome screen.</summary>
        public static Image Logo { get { return LoadImage("icon2"); } }
    }
}

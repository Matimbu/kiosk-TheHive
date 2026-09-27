using System;
using System.Collections.Generic;

namespace kiosk.UI
{
    public static class GuestText
    {
        public static bool Filipino { get; private set; }

        public static void SetFilipino(bool value) { Filipino = value; }

        private static readonly Dictionary<string, string> FilipinoText = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Good day.", "Magandang araw." },
            { "Freshly brewed coffee and hot meals,\nmade to order while you wait.", "Bagong timplang kape at mainit na pagkain,\ninihahanda para sa iyo." },
            { "Start your order", "Simulan ang order" },
            { "OPENING MENU...", "BINUBUKSAN..." },
            { "CASH AT COUNTER  ·  PAYMENT DEMOS", "CASH SA COUNTER  ·  DEMO NG BAYAD" },
            { "Best Sellers", "Sikat" }, { "Coffee", "Kape" }, { "Non-Coffee", "Iba pa" },
            { "Best seller", "Sikat" },
            { "SOLD OUT", "UBOS NA" },
            { "Still there? Tap to continue.", "Nandiyan ka pa ba? Pindutin para magpatuloy." },
            { "Reset in", "Mare-reset sa" },
            { "Just so you know", "Paalala lang" }, { "Something went wrong", "May problema" },
            { "Got it", "Sige" },
            { "Cannot add that item", "Hindi maidagdag" },
            { "Cannot save that change", "Hindi ma-save ang pagbabago" },
            { "Check your order", "Suriin ang order" },
            { "Something needs a change", "May kailangang baguhin" },
            { "Tap the item in your order to edit or remove it.", "Pindutin ang item sa order para baguhin o alisin." },
            { "Order not completed", "Hindi natuloy ang order" },
            { "Your order is still here, so you can try again.", "Nandiyan pa ang order mo, subukan ulit." },
            { "{0} added", "Naidagdag ang {0}" },
            { "Classics", "Klasiko" }, { "Cheesecake", "Keso" }, { "GentleTea", "Fruit tea" },
            { "Rice Meals", "Kanin" },
            { "What everyone orders", "Mga paborito" }, { "Pulled fresh all day", "Bagong timpla" },
            { "Easy on the caffeine", "Mas kaunting caffeine" },
            { "Thé Hive milk tea series", "Mga milk tea" },
            { "Malt drinks with cheesecake cream", "Mga malt na may cheesecake cream" },
            { "Thé Hive fruit tea series", "Mga fruit tea" },
            { "Served hot, all day", "Mainit na ihahain" },
            { "View order", "Tingnan ang order" }, { "Your order", "Ang order mo" },
            { "Your order is empty", "Wala pang order" },
            { "Clear", "Burahin" }, { "Add more", "Dagdagan" }, { "Checkout", "Magbayad" },
            { "Clear order", "Burahin ang order" }, { "Keep order", "Ituloy ang order" },
            { "Clear your order?", "Burahin ang order?" },
            { "This removes every item from your order.", "Mawawala ang lahat ng item sa order mo." },
            { "You can add items again from the menu.", "Puwede kang pumili muli sa menu." },
            { "CHOOSE AN OPTION BELOW", "PUMILI SA IBABA" },
            { "Total to pay", "Kabuuang bayad" }, { "Total", "Kabuuan" },
            { "Nothing here yet", "Wala pa rito" },
            { "Tap an item to edit", "Pindutin ang item para baguhin" },
            { "Add something from the menu and it will show up right here.", "Pumili ng pagkain o inumin sa menu." },
            { "No available items", "Walang available" },
            { "Try another category. Sold-out items are hidden.", "Pumili ng ibang kategorya. Nakatago ang ubos na produkto." },
            { "Photo coming soon", "Larawan ay susunod" },
            { "SELECT SIZE", "PILIIN ANG LAKI" }, { "SIZE", "LAKI" },
            { "TEMPERATURE", "TEMPERATURA" }, { "SERVED", "PAGHAHAIN" },
            { "SWEETNESS LEVEL", "TAMIS" }, { "SWEETNESS", "TAMIS" },
            { "QUANTITY", "DAMI" }, { "SUBTOTAL", "SUBTOTAL" },
            { "Hot", "Mainit" }, { "Iced", "Malamig" },
            { "CAFFEINE", "CAFFEINE" }, { "ALLERGENS", "ALLERGENS" },
            { "Contains caffeine (coffee)", "May caffeine (kape)" },
            { "Contains caffeine (tea)", "May caffeine (tsaa)" },
            { "Contains caffeine (matcha)", "May caffeine (matcha)" },
            { "Ask staff about caffeine", "Itanong sa staff ang caffeine" },
            { "Ingredients unverified — ask staff", "Di beripikado — itanong sa staff" },
            { "Add to order", "Idagdag sa order" },
            { "EDIT ITEM", "BAGUHIN ANG ITEM" },
            { "Line total", "Kabuuan ng item" },
            { "Remove item", "Alisin ang item" }, { "Tap again to remove", "Pindutin muli" },
            { "Cancel", "Kanselahin" }, { "Save changes", "I-save ang pagbabago" },
            { "AMOUNT DUE", "HALAGANG BABAYARAN" },
            { "HOW WOULD YOU LIKE TO PAY?", "PAANO KA MAGBABAYAD?" },
            { "Cash", "Cash" }, { "Card", "Card" }, { "E-wallet", "E-wallet" },
            { "Pay our cashier at the counter", "Magbayad sa cashier sa counter" },
            { "Demo only — no terminal connected", "Demo lang — walang card terminal" },
            { "Demo only — no payment taken", "Demo lang — walang bayad" },
            { "Not yet, go back", "Bumalik muna" }, { "Back", "Bumalik" },
            { "PAY WITH CASH", "MAGBAYAD NG CASH" },
            { "Please pay at the counter", "Magbayad sa counter" },
            { "Place your order", "Ipadala ang order" },
            { "Place order", "Ipadala ang order" },
            { "Place your order, then show the order number to our cashier and pay at the counter.",
              "Ipadala ang order, ipakita ang numero sa cashier, at magbayad sa counter." },
            { "YOUR ORDER IS HELD UNTIL PAYMENT IS CONFIRMED", "HIHINTAYIN ANG PAGKUMPIRMA NG BAYAD" },
            { "CARD PAYMENT DEMO", "DEMO NG CARD" },
            { "Try the card order flow", "Subukan ang card demo" },
            { "No payment terminal is connected.\nNo money will be charged.\n\nNo card details are needed.",
              "Walang nakakabit na card terminal.\nWalang sisingiling pera.\n\nHuwag maglagay ng card details." },
            { "Create demo order", "Gumawa ng demo order" },
            { "PAY WITH E-WALLET", "GAMIT ANG E-WALLET" },
            { "E-WALLET DEMO", "DEMO NG E-WALLET" },
            { "Demo — no payment QR", "Demo — walang QR sa bayad" },
            { "No wallet provider is connected. No money will be transferred.", "Walang konektadong e-wallet. Walang perang ililipat." },
            { "This creates a demonstration order only.", "Demo order lamang ito." },
            { "Order placed", "Naipadala ang order" },
            { "Demo order created", "Nagawa ang demo order" },
            { "Pending counter payment", "Magbayad sa counter" },
            { "Demo — no payment taken", "Demo — walang bayad" },
            { "Thank you, see you again!", "Salamat, balik po kayo!" },
            { "THIS IS NOT AN OFFICIAL RECEIPT", "HINDI ITO OPISYAL NA RESIBO" },
            { "ORDER NUMBER", "NUMERO NG ORDER" },
            { "Show this code at the counter", "Ipakita ang code sa counter" },
            { "Keep this code for reference", "Itabi ang code bilang sanggunian" },
            { "SWIPE UP FOR MORE", "I-SWIPE PATAAS PARA SA IBA PA" },
            { "Date", "Petsa" }, { "Terminal", "Terminal" }, { "Payment", "Bayad" },
            { "Status", "Status" }, { "ITEM", "ITEM" }, { "AMOUNT", "HALAGA" },
            { "Done", "Tapos" }, { "Regular", "Regular" }
        };

        private static readonly Dictionary<string, string> ProductDescriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Espresso over filtered water.", "Espresso na hinaluan ng tubig." },
            { "Americano rounded out with toasted hazelnut.", "Americano na may lasa ng hazelnut." },
            { "Espresso with steamed milk and a thin veil of foam.", "Espresso na may mainit na gatas at foam." },
            { "Cafe latte with buttery macadamia.", "Cafe latte na may macadamia." },
            { "Cafe latte with Irish cream syrup.", "Cafe latte na may Irish cream syrup." },
            { "Espresso and milk sweetened the Spanish way.", "Espresso at gatas na matamis sa estilong Espanyol." },
            { "Vanilla, steamed milk, espresso and a caramel finish.", "Vanilla, mainit na gatas, espresso at caramel." },
            { "Mascarpone and cocoa over espresso.", "Mascarpone at cocoa sa espresso." },
            { "Chocolate and espresso under a cap of cream.", "Tsokolate at espresso na may cream." },
            { "Espresso, cream and mixed berries.", "Espresso, cream at halo-halong berries." },
            { "Spiced biscuit syrup, milk and espresso.", "Biscuit syrup, gatas at espresso." },
            { "Espresso and oat milk finished with cinnamon.", "Espresso, oat milk at cinnamon." },
            { "Strawberry cream over ice.", "Strawberry cream na may yelo." },
            { "Stone-ground matcha with milk.", "Matcha na may gatas." },
            { "Matcha under a salted cream cap.", "Matcha na may maalat na cream." },
            { "Matcha layered over banana milk.", "Matcha sa ibabaw ng banana milk." },
            { "Matcha and strawberry with milk.", "Matcha, strawberry at gatas." },
            { "Matcha and ripe mango with milk.", "Matcha, hinog na mangga at gatas." },
            { "Belgian chocolate, hot or iced.", "Belgian chocolate, mainit o malamig." },
            { "Passion fruit and peach, sparkling.", "Passion fruit at peach na may soda." },
            { "Green apple soda over ice.", "Green apple soda na may yelo." },
            { "Black tea with Okinawa brown sugar.", "Black tea na may Okinawa brown sugar." },
            { "Dark chocolate milk tea.", "Milk tea na may dark chocolate." },
            { "The house milk tea with pearls.", "Milk tea ng bahay na may pearls." },
            { "Matcha blended into milk tea.", "Milk tea na may matcha." },
            { "Winter melon with milk tea.", "Milk tea na may winter melon." },
            { "Spiced Thai tea with milk.", "Thai tea na may gatas at pampalasa." },
            { "Hokkaido-style creamy milk tea.", "Creamy milk tea na estilong Hokkaido." },
            { "Taro root with milk tea.", "Milk tea na may taro." },
            { "Chocolate malt with cheesecake cream.", "Chocolate malt na may cheesecake cream." },
            { "Mixed berry malt with cheesecake cream.", "Berry malt na may cheesecake cream." },
            { "Banana malt with cheesecake cream.", "Banana malt na may cheesecake cream." },
            { "Berries and banana with cheesecake cream.", "Berries at banana na may cheesecake cream." },
            { "Chocolate and hazelnut with cheesecake cream.", "Tsokolate at hazelnut na may cheesecake cream." },
            { "Red velvet with cheesecake cream.", "Red velvet na may cheesecake cream." },
            { "Crushed cookies with cheesecake cream.", "Dinurog na cookies na may cheesecake cream." },
            { "Matcha with cheesecake cream.", "Matcha na may cheesecake cream." },
            { "Green tea with apple, sparkling.", "Green tea at mansanas na may soda." },
            { "Passion fruit fruit tea.", "Fruit tea na may passion fruit." },
            { "Pomegranate fruit tea.", "Fruit tea na may pomegranate." },
            { "Cured beef, garlic rice, egg and fresh tomato.", "Beef tapa, sinangag, itlog at kamatis." },
            { "Boneless milkfish baked with herbs and butter.", "Bangus na walang tinik, may herbs at butter." },
            { "Glazed chicken thigh with steamed rice and egg.", "Manok na may kanin at itlog." },
            { "Breaded pork cutlet under house katsu sauce.", "Breaded pork na may katsu sauce." },
            { "Grilled ham, two eggs and toasted garlic rice.", "Ham, dalawang itlog at sinangag." },
            { "Hungarian sausage with rice, egg and pickles.", "Hungarian sausage na may kanin, itlog at atsara." }
        };

        public static string T(string english)
        {
            if (!Filipino || english == null) return english;
            string translated;
            if (FilipinoText.TryGetValue(english, out translated)) return translated;
            if (ProductDescriptions.TryGetValue(english, out translated)) return translated;
            if (english.StartsWith("from ", StringComparison.OrdinalIgnoreCase)) return "mula " + english.Substring(5);
            if (english.StartsWith("Please pay ", StringComparison.OrdinalIgnoreCase) && english.EndsWith(" at the counter", StringComparison.OrdinalIgnoreCase))
                return "Magbayad ng " + english.Substring(11, english.Length - 26) + " sa counter";
            if (english.StartsWith("No payment taken — ", StringComparison.OrdinalIgnoreCase))
                return "Walang bayad — " + english.Substring(19);   // same length as before: the dash is one character
            if (english.EndsWith("% sugar", StringComparison.OrdinalIgnoreCase))
                return english.Substring(0, english.Length - 6) + " asukal";
            if (english.EndsWith(" is unavailable. Please remove it from your order.", StringComparison.OrdinalIgnoreCase))
                return "Ubos ang " + english.Substring(0, english.Length - 50) + ". Alisin ito sa order.";
            if (english == "Choose between 1 and 99 of each item.") return "Pumili ng 1 hanggang 99 sa bawat item.";
            if (english == "Choose a valid size.") return "Pumili ng tamang laki.";
            if (english == "Choose hot or iced.") return "Pumili ng mainit o malamig.";
            if (english == "The item price has changed. Please add it again.") return "Nagbago ang presyo. Idagdag muli ang item.";
            if (english == "Add an item before checking out.") return "Pumili muna ng item bago magbayad.";
            if (english == "You can order up to 99 of this combination.") return "Hanggang 99 lang sa kombinasyong ito.";
            if (english == "1 item") return "1 item";
            if (english.EndsWith(" items", StringComparison.OrdinalIgnoreCase)) return english.Substring(0, english.Length - 6) + " item";
            // Composed at the call site, so the whole line misses the lookup above.
            // The leading part is already Filipino; only the tail needs handling.
            if (english.EndsWith(" each", StringComparison.OrdinalIgnoreCase))
                return english.Substring(0, english.Length - 5) + " bawat isa";
            if (english.StartsWith("QTY ", StringComparison.OrdinalIgnoreCase))
                return "DAMI " + english.Substring(4);
            return english;
        }
    }
}

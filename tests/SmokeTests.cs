using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using kiosk;
using kiosk.UI;
using kiosk.Payments;

class SmokeTests
{
    static void Check(bool value, string message) { if (!value) throw new Exception(message); Console.WriteLine("PASS " + message); }
    static Order Coffee(int qty = 1) { return new Order { Product = "Americano", Size = "16L", Temperature = "Hot", Price = 20, Quantity = qty }; }
    static void Fails(Action action, string message) { try { action(); } catch (Exception) { Console.WriteLine("PASS " + message); return; } throw new Exception(message); }
    static void Shot(Form form, string name, string folder)
    {
        using (form) {
            form.ClientSize = new Size(480, 720);
            form.Show(); Application.DoEvents();
            using (var bitmap = new Bitmap(480, 720)) { form.DrawToBitmap(bitmap, new Rectangle(0, 0, 480, 720)); bitmap.Save(Path.Combine(folder, name + ".png")); }
        }
    }
    [STAThread] static int Main(string[] args)
    {
        try {
            Application.EnableVisualStyles();
            LocalStore.Root = Path.Combine(args[0], "data-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(args[0]);
            OrderStorage.AddOrder(Coffee(2)); OrderStorage.AddOrder(Coffee(3));
            Check(OrderStorage.Orders.Count == 1 && OrderStorage.GetTotal() == 100, "matching customization merges quantities and total");
            var lessSweet = Coffee(); lessSweet.Sweetness = "50%";
            OrderStorage.AddOrder(lessSweet);
            Check(OrderStorage.Orders.Count == 2 && OrderStorage.GetTotal() == 120, "different sweetness remains a separate line");
            Check(OrderText.Describe(lessSweet).Contains("50% sugar"), "cart and receipt description show sweetness");
            Fails(() => OrderStorage.AddOrder(Coffee(99)), "quantity overflow rejected");
            LocalStore.SetSoldOut("Americano", true);
            LocalStore.SoldOut.Clear(); LocalStore.LoadAvailability();
            Check(LocalStore.SoldOut.Contains("Americano"), "availability survives reload");
            Fails(() => LocalStore.Submit("Cash"), "sold-out cart cannot submit");

            // sold out is shown, not hidden: the tile stays on the menu, dimmed,
            // sorted to the end, and refuses to open the product page
            using (var grid = new CategoryView())
            {
                grid.Size = new Size(480, 520);
                grid.Load("Best Sellers");
                var tiles = grid.Content.Controls.OfType<ProductTile>().ToList();
                Check(tiles.Any(t => t.Item.Name == "Americano"),
                    "a sold-out item stays on the menu");
                Check(tiles.Single(t => t.Item.Name == "Americano").SoldOut,
                    "the sold-out tile is marked");
                Check(tiles.FindIndex(t => t.SoldOut) >= tiles.Count(t => !t.SoldOut),
                    "sold-out items settle to the end of the category");
                bool opened = false;
                var soldTile = tiles.Single(t => t.Item.Name == "Americano");
                soldTile.Chosen += (s, e) => opened = true;
                typeof(ProductTile).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance)
                    .Invoke(soldTile, new object[] { EventArgs.Empty });
                Check(!opened, "tapping a sold-out tile opens nothing");
            }

            LocalStore.SetSoldOut("Americano", false);
            var saved = LocalStore.Submit("Cash");
            Check(saved.DisplayNumber.Length == 7 && saved.Id.EndsWith(saved.DisplayNumber.Substring(1), StringComparison.OrdinalIgnoreCase),
                "guest order code is short and maps to the saved record");
            OrderStorage.Orders[0].Quantity = 1;
            Check(saved.Total == 120 && saved.Lines[1].Sweetness == "50%", "saved snapshot retains sweetness independently of cart");
            int unreadable; var history = LocalStore.History(out unreadable);
            Check(history.Count == 1 && history[0].Total == 120 && history[0].Status == "Pending counter payment" && history[0].Lines[1].Sweetness == "50%", "history reload preserves amounts, status and sweetness");
            File.WriteAllText(Path.Combine(LocalStore.Root, "Orders", "broken.xml"), "broken");
            Check(LocalStore.History(out unreadable).Count == 1 && unreadable == 1, "damaged record does not hide valid history");
            var demo = LocalStore.Submit("Card"); Check(demo.Status == "Demo - no payment taken", "card demo never marked paid");
            string root = LocalStore.Root;
            string blocked = Path.Combine(root, "blocked"); File.WriteAllText(blocked, "file"); LocalStore.Root = blocked;
            Fails(() => LocalStore.Submit("Cash"), "storage failure reported");
            Check(OrderStorage.Orders.Count == 2, "storage failure retains cart"); LocalStore.Root = root;
            using (var sheet = new ProductSheet(MenuCatalog.Find("Americano"))) {
                var temperature = (Segmented)typeof(ProductSheet).GetField("_temp", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(sheet);
                Check(temperature.SelectedIndex == 1, "drinks with a temperature choice default to iced");
            }
            Check(MenuCatalog.Find("Americano").DisplayCaffeineNote.Contains("caffeine") &&
                  MenuCatalog.Find("Americano").DisplayAllergenNote.Contains("unverified"),
                  "product details expose caffeine and unverified allergen guidance");
            Shot(new Form1(), "welcome", args[0]);
            Shot(new Form1(), "welcome", args[0]);
            Shot(new menuPage(), "menu", args[0]);
            Shot(new ProductSheet(MenuCatalog.Find("Americano")), "product", args[0]);
            Shot(new ProductSheet(MenuCatalog.Find("Traditional Matcha")), "product-matcha", args[0]);
            Shot(new EditOrderForm(lessSweet), "edit", args[0]);
            Shot(new viewOrder(), "cart", args[0]);
            Shot(new ClearOrderForm(), "clear-confirm", args[0]);
            Shot(new StaffPage(), "staff", args[0]);
            Shot(new PaymentSelectionForm(OrderStorage.GetOrders(), OrderStorage.GetTotal()), "payment", args[0]);
            Shot(new CardPaymentForm(), "card", args[0]);
            Shot(new EWalletPaymentForm(OrderStorage.GetOrders()), "wallet", args[0]);
            using (var preferences = new Form1()) {
                var language = (HiveButton)typeof(Form1).GetField("_languageButton", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(preferences);
                language.PerformClick();
                Check(GuestText.Filipino && GuestText.T("Your order") == "Ang order mo", "language choice changes the customer view");
                Check(OrderText.Describe(lessSweet).Contains("50% asukal"), "Filipino cart keeps customization details readable");
            }
            Shot(new Form1(), "welcome-fil", args[0]);
            Shot(new menuPage(), "menu-fil", args[0]);
            Shot(new ProductSheet(MenuCatalog.Find("Americano")), "product-fil", args[0]);
            Shot(new ProductSheet(MenuCatalog.Find("Traditional Matcha")), "product-matcha-fil", args[0]);
            Shot(new EditOrderForm(lessSweet), "edit-fil", args[0]);
            Shot(new viewOrder(), "cart-fil", args[0]);
            Shot(new ClearOrderForm(), "clear-confirm-fil", args[0]);
            Shot(new PaymentSelectionForm(OrderStorage.GetOrders(), OrderStorage.GetTotal()), "payment-fil", args[0]);
            Shot(new CashPaymentForm(), "cash-fil", args[0]);
            Shot(new CardPaymentForm(), "card-fil", args[0]);
            Shot(new EWalletPaymentForm(OrderStorage.GetOrders()), "wallet-fil", args[0]);
            var filipinoOrder = LocalStore.Submit("Cash");
            var receiptType = typeof(ReceiptForm);
            var receiptCtor = receiptType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(SavedOrder) }, null);
            using (var scrollableReceipt = (Form)receiptCtor.Invoke(new object[] { filipinoOrder })) {
                scrollableReceipt.Show(); Application.DoEvents();
                var scroll = (ScrollHost)receiptType.GetField("_scroll", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scrollableReceipt);
                var cue = (Label)receiptType.GetField("_scrollCue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(scrollableReceipt);
                Check(cue.Visible, "long receipt shows a scroll cue");
                typeof(ScrollHost).GetMethod("SetTop", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(scroll, new object[] { -10000 });
                Check(!cue.Visible, "scroll cue clears at the end of the receipt");
            }
            Shot((Form)receiptCtor.Invoke(new object[] { filipinoOrder }), "receipt-fil", args[0]);
            Check(Directory.GetFiles(LocalStore.ReceiptFolder, "*.txt").Any(file => File.ReadAllText(file).Contains("ORDER BLG. " + filipinoOrder.DisplayNumber) && File.ReadAllText(file).Contains("Ipakita ang code sa counter")), "Filipino receipt copy shows the short code and counter instruction");

            // the printed slip is 40 columns; Filipino customizations are longer
            // than the English ones and used to run past the edge
            var filipinoSlip = Directory.GetFiles(LocalStore.ReceiptFolder, "*.txt")
                .OrderByDescending(File.GetLastWriteTimeUtc).First();
            var slipLines = File.ReadAllLines(filipinoSlip);
            Check(slipLines.All(l => l.Length <= 40),
                "every line of the Filipino receipt fits the 40-column slip");
            Check(slipLines.Any(l => l.Contains("HALAGA")),
                "the Filipino receipt translates the amount column");
            Check(!slipLines.Any(l => l.TrimEnd().EndsWith("50%")),
                "a long customization is not split mid-phrase");

            GuestText.SetFilipino(false);
            using (var shell = new Shell()) {
                shell.Show(); Application.DoEvents();
                var stack = (System.Collections.IList)typeof(Shell).GetField("_stack", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shell);
                var welcome = (Form1)stack[0];
                var start = (HiveButton)typeof(Form1).GetField("btn_Start", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(welcome);
                start.PerformClick();
                Check(stack.Count == 1 && start.Text == "OPENING MENU...", "start tap shows feedback before navigation");
                using (var image = new Bitmap(480,720)) { shell.DrawToBitmap(image,new Rectangle(0,0,480,720)); image.Save(Path.Combine(args[0],"welcome-pressed.png")); }
                start.PerformClick();
                var startTimer = (Timer)typeof(Form1).GetField("_startDelay", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(welcome);
                typeof(Timer).GetMethod("OnTick", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(startTimer, new object[] { EventArgs.Empty });
                Check(stack.Count == 2 && stack[1] is menuPage, "repeated start taps open one menu");

                // the add confirmation: recorded on add, spoken by the menu, then cleared
                var menu = (menuPage)stack[1];
                OrderStorage.AddOrder(Coffee());
                Check(OrderStorage.LastAdded != null && OrderStorage.LastAdded.Contains("Americano"),
                    "adding an item records it for the confirmation");
                menu.OnRevealed(); Application.DoEvents();
                var toast = (Toast)typeof(menuPage).GetField("_added", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(menu);
                Check(toast.Visible && OrderStorage.LastAdded == null,
                    "the menu shows the add confirmation once and clears it");
                OrderStorage.ClearOrders();
                Check(OrderStorage.LastAdded == null, "clearing the cart drops a pending confirmation");

                OrderStorage.AddOrder(Coffee());
                shell.Go(new viewOrder()); Application.DoEvents();
                var cart = (viewOrder)stack[2];
                var clear = (HiveButton)typeof(viewOrder).GetField("_clear", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(cart);
                clear.PerformClick();
                Check(stack.Count == 4 && stack[3] is ClearOrderForm && OrderStorage.Orders.Count > 0,
                    "clear opens a kiosk confirmation without changing the cart");
                var confirmation = (ClearOrderForm)stack[3];
                ((HiveButton)typeof(ClearOrderForm).GetField("_keep", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(confirmation)).PerformClick();
                Check(stack.Count == 3 && OrderStorage.Orders.Count > 0, "keeping the order preserves every item");
                clear.PerformClick();
                confirmation = (ClearOrderForm)stack[3];
                ((HiveButton)typeof(ClearOrderForm).GetField("_confirm", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(confirmation)).PerformClick();
                Check(stack.Count == 3 && OrderStorage.Orders.Count == 0, "confirmed clear empties the cart");

                // a blocked checkout must open a kiosk notice, not a Windows dialog
                shell.Go(new viewOrder()); Application.DoEvents();
                var blockedCart = (viewOrder)stack[stack.Count - 1];
                OrderStorage.AddOrder(Coffee());
                LocalStore.SetSoldOut("Americano", true);
                var payBtn = (HiveButton)typeof(viewOrder)
                    .GetField("btn_paymentMethod", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(blockedCart);
                int before = stack.Count;
                payBtn.PerformClick(); Application.DoEvents();
                Check(stack.Count == before + 1 && stack[stack.Count - 1] is NoticeForm,
                    "a blocked checkout opens a kiosk notice");
                Check(OrderStorage.Orders.Count > 0, "a blocked checkout leaves the order alone");
                shell.Back(); shell.Back();
                LocalStore.SetSoldOut("Americano", false);
                OrderStorage.ClearOrders();

                shell.Back();
                OrderStorage.AddOrder(Coffee());
                ReceiptForm.Submit("Cash"); Application.DoEvents();
                Check(OrderStorage.Orders.Count == 0, "successful submission clears cart");
                Check(stack.Count == 2 && stack[1] is ReceiptForm, "completion removes previous checkout pages");
                using (var image = new Bitmap(480,720)) { shell.DrawToBitmap(image,new Rectangle(0,0,480,720)); image.Save(Path.Combine(args[0],"receipt.png")); }
                shell.Back(); Check(stack.Count == 1 && stack[0] is Form1, "back after checkout returns to welcome");
                Check(start.Text == "Start your order", "welcome resets after checkout");
                shell.Go(new menuPage()); OrderStorage.AddOrder(Coffee());
                GuestText.SetFilipino(true);
                typeof(Shell).GetField("_lastInput",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(shell,DateTime.UtcNow.AddSeconds(-125));
                var timer = (Timer)typeof(Shell).GetField("_idle",BindingFlags.NonPublic|BindingFlags.Instance).GetValue(shell);
                typeof(Timer).GetMethod("OnTick",BindingFlags.NonPublic|BindingFlags.Instance).Invoke(timer,new object[]{EventArgs.Empty});
                Check(stack.Count == 1 && OrderStorage.Orders.Count == 0 && !GuestText.Filipino,
                    "idle timeout clears cart and restores default language");

                // the idle warning is a Label, so it never passes through Hive.Text
                shell.Go(new menuPage()); Application.DoEvents();
                GuestText.SetFilipino(true);
                var notice = (Label)typeof(Shell).GetField("_idleNotice", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shell);
                typeof(Shell).GetField("_lastInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(shell, DateTime.UtcNow.AddSeconds(-95));
                typeof(Timer).GetMethod("OnTick", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(timer, new object[] { EventArgs.Empty });
                Check(notice.Text.StartsWith("Nandiyan ka pa ba"),
                    "the idle warning speaks the guest language");
                GuestText.SetFilipino(false);

            }
            Console.WriteLine("All smoke tests passed."); return 0;
        } catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}

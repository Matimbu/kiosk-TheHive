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
            AuditTaps(form, name);
            using (var bitmap = new Bitmap(480, 720)) { form.DrawToBitmap(bitmap, new Rectangle(0, 0, 480, 720)); bitmap.Save(Path.Combine(folder, name + ".png")); }
        }
    }
    static readonly System.Collections.Generic.List<string> TapProblems = new System.Collections.Generic.List<string>();

    // Every guest tap zone at least 48x48 and no button label cut off with an
    // ellipsis. Runs on every screen Shot renders, so a new screen is audited
    // without anyone remembering to add it. Selectors and steppers are one
    // control each but several tap zones, so they are measured per zone.
    static void AuditTaps(Control root, string screen)
    {
        if (screen.StartsWith("staff")) return;   // staff screen is out of scope
        using (var scratch = new Bitmap(1, 1))
        using (var g = Graphics.FromImage(scratch))
            AuditTaps(root, screen, g);
    }

    static void AuditTaps(Control parent, string screen, Graphics g)
    {
        foreach (Control c in parent.Controls)
        {
            if (!c.Visible) continue;
            Size zone = Size.Empty;
            var button = c as HiveButton;
            var picker = c as Segmented;
            if (button != null)
            {
                zone = button.Size;
                if (!button.LabelFits(g))
                    TapProblems.Add(screen + ": '" + button.Text + "' is cut off at " + button.Width + "px");
            }
            else if (picker != null && picker.Options.Length > 0) zone = new Size(picker.Width / picker.Options.Length, picker.Height);
            else if (c is Stepper) zone = new Size(c.Height, c.Height);
            else if (c is HiveChip || c is ProductTile || c is OrderRow || c is MethodTile) zone = c.Size;

            var quick = c as ProductTile;
            if (quick != null && quick.CanQuickAdd && (quick.QuickAddZone.Width < 48 || quick.QuickAddZone.Height < 48))
                TapProblems.Add(screen + ": quick-add + on '" + quick.Item.Name + "' is " + quick.QuickAddZone.Width + "x" + quick.QuickAddZone.Height);

            if (!zone.IsEmpty && (zone.Width < 48 || zone.Height < 48))
                TapProblems.Add(screen + ": " + c.GetType().Name + " '" + c.Text + "' is " + zone.Width + "x" + zone.Height);
            AuditTaps(c, screen, g);
        }
    }

    static System.Collections.Generic.IEnumerable<T> FindAll<T>(Control root) where T : Control
    {
        foreach (Control c in root.Controls)
        {
            if (c is T) yield return (T)c;
            foreach (T inner in FindAll<T>(c)) yield return inner;
        }
    }

    // WCAG 2 contrast ratio between two opaque colours.
    static double Contrast(Color a, Color b)
    {
        Func<double, double> lin = v => { v /= 255; return v <= 0.03928 ? v / 12.92 : Math.Pow((v + 0.055) / 1.055, 2.4); };
        Func<Color, double> lum = c => 0.2126 * lin(c.R) + 0.7152 * lin(c.G) + 0.0722 * lin(c.B);
        double x = lum(a), y = lum(b);
        return (Math.Max(x, y) + 0.05) / (Math.Min(x, y) + 0.05);
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
            Check(OrderText.Describe(new Order { Product = "Beef Tapa", Size = "", Temperature = "", Price = 145, Quantity = 1 }) == "",
                "a rice meal has no option line, not a placeholder word");
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
            Shot(new NoticeForm("Check your order", "Something needs a change", "Cafe Latte is unavailable. Please remove it from your order.", "Tap the item in your order to edit or remove it.", false), "notice", args[0]);
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
            Shot(new NoticeForm("Check your order", "Something needs a change", "Cafe Latte is unavailable. Please remove it from your order.", "Tap the item in your order to edit or remove it.", false), "notice-fil", args[0]);
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
            string filMonth = System.Globalization.CultureInfo.GetCultureInfo("fil-PH").DateTimeFormat.AbbreviatedMonthNames[DateTime.Now.Month - 1];
            Check(slipLines.Any(l => l.StartsWith("Petsa") && l.Contains(" " + filMonth + " ")),
                "the Filipino receipt is dated in Filipino months");

            // dates follow the guest's language, never the PC's regional setting
            var machineCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.GetCultureInfo("fil-PH");
            bool guestWasFilipino = GuestText.Filipino;
            GuestText.SetFilipino(false);
            string englishDate = new DateTime(2026, 9, 27).ToString("dd MMM yyyy", GuestText.DateCulture);
            GuestText.SetFilipino(true);
            string filipinoDate = new DateTime(2026, 9, 27).ToString("dd MMM yyyy", GuestText.DateCulture);
            GuestText.SetFilipino(guestWasFilipino);
            System.Threading.Thread.CurrentThread.CurrentCulture = machineCulture;
            Check(englishDate == "27 Sep 2026", "English dates stay English on a Filipino-locale PC");
            Check(filipinoDate == "27 Set 2026", "Filipino dates use Filipino months");

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

                // the + adds a rice meal straight to the cart; drinks still open their page
                var menuGrid = FindAll<CategoryView>(menu).First();
                var tileDown = typeof(ProductTile).GetMethod("OnMouseDown", BindingFlags.NonPublic | BindingFlags.Instance);
                var tileClick = typeof(ProductTile).GetMethod("OnClick", BindingFlags.NonPublic | BindingFlags.Instance);
                Action<ProductTile, Point> tapTile = (tile, at) =>
                {
                    tileDown.Invoke(tile, new object[] { new MouseEventArgs(MouseButtons.Left, 1, at.X, at.Y, 0) });
                    tileClick.Invoke(tile, new object[] { EventArgs.Empty });
                    Application.DoEvents();
                };
                Func<Rectangle, Point> centre = r => new Point(r.X + r.Width / 2, r.Y + r.Height / 2);

                OrderStorage.ClearOrders();
                menuGrid.Load("Rice Meals"); Application.DoEvents();
                var tapa = FindAll<ProductTile>(menuGrid).Single(t => t.Item.Name == "Beef Tapa");
                int onMenu = stack.Count;
                toast.Visible = false;
                tapTile(tapa, centre(tapa.QuickAddZone));
                Check(OrderStorage.Orders.Count == 1 && OrderStorage.Orders[0].Product == "Beef Tapa" && stack.Count == onMenu,
                    "the + on a rice meal adds it without leaving the menu");
                Check(toast.Visible && OrderStorage.LastAdded == null, "a quick add shows the confirmation");
                Check(tapa.InOrder == 1, "a quick add shows the count on the tile at once");
                tapTile(tapa, centre(tapa.QuickAddZone));
                Check(tapa.InOrder == 2 && OrderStorage.Orders.Count == 1, "a second quick add raises the tile count, on one cart line");
                Check(FindAll<ProductTile>(menuGrid).Where(t => t != tapa).All(t => t.InOrder == 0),
                    "only the item that was added carries a count");
                bool wasFilipino = GuestText.Filipino;
                foreach (bool fil in new[] { false, true })
                {
                    GuestText.SetFilipino(fil);
                    int best = TextRenderer.MeasureText(GuestText.T("Best seller").ToUpperInvariant(), Hive.Overline).Width + 18;
                    int most = TextRenderer.MeasureText(string.Format(GuestText.T("{0} in order"), 99).ToUpperInvariant(), Hive.Overline).Width + 18;
                    Check(8 + best + 8 + most + 8 <= tapa.Width - 7,
                        "the Best Seller tag and a 99 count fit side by side on a tile" + (fil ? " (Filipino)" : ""));
                }
                GuestText.SetFilipino(wasFilipino);
                tapTile(tapa, new Point(tapa.Width / 2, 40));   // on the photo, not the +
                Check(stack.Count == onMenu + 1 && stack[stack.Count - 1] is ProductSheet,
                    "tapping a rice meal anywhere else still opens its page");
                shell.Back(); Application.DoEvents();

                menuGrid.Load("Coffee"); Application.DoEvents();
                var americanoTile = FindAll<ProductTile>(menuGrid).First(t => t.Item.Name == "Americano");
                int inCart = OrderStorage.Orders.Count;
                tapTile(americanoTile, centre(americanoTile.QuickAddZone));
                Check(stack[stack.Count - 1] is ProductSheet && OrderStorage.Orders.Count == inCart,
                    "the + on a drink opens its page instead of guessing size and temperature");
                shell.Back(); Application.DoEvents();
                OrderStorage.AddOrder(Coffee(2));
                OrderStorage.AddOrder(new Order { Product = "Americano", Size = "16L", Temperature = "Iced", Price = 20, Quantity = 1 });
                menu.OnRevealed(); Application.DoEvents();
                Check(americanoTile.InOrder == 3, "the tile count adds up every cart line for that item (hot and iced)");
                OrderStorage.ClearOrders(); Application.DoEvents();
                Check(americanoTile.InOrder == 0, "clearing the cart clears the tile counts");

                var malt = MenuCatalog.Find("Choco Malt");
                Check(malt.IsDrink && malt.HasChoices,
                    "the cheesecake malts count as drinks, so they get a sweetness choice");
                OrderStorage.ClearOrders();
                menuGrid.Load("Best Sellers"); Application.DoEvents();

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

                // changing how many happens on the cart row, not behind an edit screen
                OrderStorage.AddOrder(Coffee());
                shell.Go(new viewOrder()); Application.DoEvents();
                var cartRow = FindAll<OrderRow>((Control)stack[stack.Count - 1]).First();
                var rowStepper = cartRow.Controls.OfType<Stepper>().Single();
                var press = typeof(Stepper).GetMethod("OnMouseDown", BindingFlags.NonPublic | BindingFlags.Instance);
                Action<int> tapAt = x => press.Invoke(rowStepper, new object[] { new MouseEventArgs(MouseButtons.Left, 1, x, rowStepper.Height / 2, 0) });
                int depth = stack.Count;
                tapAt(rowStepper.Width - rowStepper.Height / 2);   // the + knob
                Check(OrderStorage.Orders[0].Quantity == 2 && OrderStorage.GetTotal() == 40,
                    "the + on a cart row adds one in place");
                Check(stack.Count == depth && !cartRow.IsDisposed,
                    "changing quantity stays on the cart");
                tapAt(rowStepper.Height / 2); tapAt(rowStepper.Height / 2);   // the - knob, twice
                Check(OrderStorage.Orders[0].Quantity == 1,
                    "the - on a cart row stops at one");
                shell.Back();
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

                // the idle warning speaks the guest's language and fills the screen
                shell.Go(new menuPage()); Application.DoEvents();
                OrderStorage.AddOrder(Coffee());
                GuestText.SetFilipino(true);
                var notice = (IdleWarning)typeof(Shell).GetField("_idleNotice", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(shell);
                typeof(Shell).GetField("_lastInput", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(shell, DateTime.UtcNow.AddSeconds(-95));
                typeof(Timer).GetMethod("OnTick", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(timer, new object[] { EventArgs.Empty });
                Check(notice.Visible && notice.Text.StartsWith("Nandiyan ka pa ba") && notice.Text.Contains("Mabubura ang order mo"),
                    "the idle warning speaks the guest language and warns about the order");
                Check(notice.Width >= Hive.ScreenW && notice.Height >= Hive.ScreenH,
                    "the idle warning covers the whole screen");
                GuestText.SetFilipino(false);

                // the tap that dismisses it must not also press what is underneath
                var dismissTap = Message.Create(shell.Handle, 0x201, IntPtr.Zero, IntPtr.Zero);   // WM_LBUTTONDOWN
                bool swallowed = shell.PreFilterMessage(ref dismissTap);
                Check(swallowed && !notice.Visible,
                    "the tap that dismisses the idle warning goes no further");
                var nextPress = Message.Create(shell.Handle, 0x201, IntPtr.Zero, IntPtr.Zero);
                Check(!shell.PreFilterMessage(ref nextPress),
                    "with the warning gone, taps reach the page again");
                OrderStorage.ClearOrders();

            }
            // every text colour must read at 4.5:1 on every light surface. Disabled
            // is left out on purpose: inactive controls are meant to look faint.
            var textColours = new[] { Hive.Ink, Hive.InkSoft, Hive.Muted, Hive.Teal, Hive.TealDeep, Hive.TealDarker, Hive.Danger };
            var surfaces = new[] { Hive.Canvas, Hive.Surface, Hive.SurfaceAlt, Hive.TealWash, Hive.HoneyWash };
            double worst = textColours.SelectMany(fg => surfaces.Select(bg => Contrast(fg, bg))).Min();
            Check(worst >= 4.5, "every text colour reads at 4.5:1 on every surface (worst " + worst.ToString("0.00") + ")");
            // No stock Windows font anywhere in the type system: every Font token
            // resolves to Sitka or Bahnschrift on a machine that has them.
            var stock = new[] { "Segoe", "Consolas", "Tahoma", "Microsoft Sans Serif", "Arial", "Calibri" };
            var plainFonts = typeof(Hive).GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(f => f.FieldType == typeof(Font))
                .Select(f => f.Name + " = " + ((Font)f.GetValue(null)).FontFamily.Name)
                .Where(s => stock.Any(name => s.Split('=')[1].Trim().StartsWith(name)))
                .ToArray();
            Check(plainFonts.Length == 0, "no text uses a stock Windows font"
                + (plainFonts.Length == 0 ? "" : ": " + string.Join(", ", plainFonts)));

            Check(Contrast(Hive.InkSoft, Hive.Canvas) - Contrast(Hive.Muted, Hive.Canvas) >= 1.5,
                "muted text stays clearly quieter than body text");

            Check(TapProblems.Count == 0, "every guest tap target is at least 48px and no label is cut off"
                + (TapProblems.Count == 0 ? "" : ": " + string.Join("; ", TapProblems.ToArray())));
            Console.WriteLine("All smoke tests passed."); return 0;
        } catch(Exception ex) { Console.Error.WriteLine(ex); return 1; }
    }
}

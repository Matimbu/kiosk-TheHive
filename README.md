# The Hive Cafe — Self-Order Kiosk

**Version 2.2** · [Patch notes](docs/patch-notes/v2.2.md) · [Release history](docs/patch-notes/README.md)

A C# WinForms touchscreen kiosk with a hand-painted GDI+ interface, menu browsing, customization, cart editing, and saved orders.

<p align="center">
  <img src="docs/screenshots/welcome.png" width="220" alt="Welcome screen">
  <img src="docs/screenshots/menu.png" width="220" alt="Menu and category navigation">
  <img src="docs/screenshots/product.png" width="220" alt="Product customization">
</p>

## Run

1. Open `kiosk.sln` in Visual Studio 2022 or later with .NET Framework 4.7.2 targeting support.
2. Set `kiosk` as the startup project and press F5.
3. The kiosk opens at 480 × 720.

After building, you can also open `kiosk/bin/Release/kiosk.exe` directly without Visual Studio.

## Customer guidance and accessibility

- The bottom of the menu, order review, and payment screens shows **Menu → Order → Pay** with the current step highlighted.
- On the welcome screen, tap **English / Filipino** to switch languages before starting. The choice applies through the customer flow, including product descriptions and the receipt copy.
- After an order is finished or the kiosk times out, the language resets for the next customer. Product names remain as printed on the cafe menu.

## Ordering

- Browse the menu using seven icon-labeled category tabs; swipe the tabs sideways for more categories.
- Sold-out items are hidden. Availability and prices are checked again at checkout.
- Choose size, hot/iced preparation, sweetness where supported, and quantity. Drinks with a temperature choice start iced; guests can switch to hot. Product details show caffeine guidance and flag unverified allergen information before adding an item. Identical customizations merge into one cart line, with a maximum of 99 per combination.
- Tap a cart item to edit its options or remove it. Clear empties the cart after confirmation.
- After 90 seconds without keyboard, click, wheel, or touch input, a 30-second warning appears. Interaction dismisses it; at two minutes the session returns to welcome and clears the cart.

## Payment behavior

**There is no connected payment processor.**

- **Cash:** saves an order as **Pending counter payment**. The receipt shows a short guest code such as `#637E93` with **Show this code at the counter**; the guest reads that code to the cashier. Staff can record cash received in the staff screen.
- **Card and e-wallet:** create explicitly labeled demonstration orders. They do not charge money, collect card details, or display a live payment QR.
- Submitted orders are saved before the cart clears. A storage failure keeps the cart available to retry. Back/Escape after submission returns to welcome, so the old checkout cannot be resubmitted.
- The kiosk saves a text receipt copy; it does not automatically send it to a physical printer. Receipts are not official tax receipts.

## Staff access

Set a private `HIVE_STAFF_PIN` Windows environment variable for the account running the kiosk, then restart the app (and Visual Studio if launching from it). There is no default PIN. From the welcome screen, press **Ctrl+Shift+S**, enter the PIN, and unlock.

- **Order history:** inspect saved line items, totals, payment methods, and statuses. Select a pending cash order and use **Confirm selected cash payment** only after receiving the money.
- **Availability:** checked products are sold out. Changes save immediately and persist after restarting.
- Staff sessions also expire after two minutes of inactivity. This is a local staff gate, not a multi-user authentication system; protect the Windows account and its data directory.

## Data and recovery

Data is stored per Windows user in `%LOCALAPPDATA%\TheHiveCafe`:

- `Orders\*.xml`: individual submitted order snapshots with unique IDs and payment status.
- `Receipts\*.txt`: readable receipt copies.
- `availability.xml`: sold-out products.

Back up this folder to preserve history and availability. Submitted orders survive restarts; unfinished carts intentionally do not, so a new guest never inherits the previous guest's order. Writes use a temporary file followed by replacement. Unreadable order files are counted in history while valid records remain accessible. If availability cannot be read, startup stops rather than silently selling unavailable stock. Restore that file from a backup, or rename it to reset all products to available.

Old receipt files beside the executable remain in place and are not imported into structured history. There is no cloud synchronization or automatic retention cleanup.

## Verification

`tests/SmokeTests.cs` checks cart merging and limits, availability reload, sold-out checkout rejection, independent saved snapshots, history reload and damaged records, payment status, storage failure preservation, checkout navigation, and idle reset. It also renders the main screens to PNGs for layout review.

Run `tests/Run-SmokeTests.ps1` from PowerShell. Build output and test data/screenshots go beneath the ignored `kiosk/bin/Release` directory. Each test run uses its own data directory and does not touch real kiosk history.

## Project structure

- `kiosk/UI`: design system, catalog, navigation, menu, and staff screen.
- `kiosk/OrderForms`: cart, customization, validation, and local persistence.
- `kiosk/Payments`: payment choices, demo flows, and receipts.
- `kiosk/Resources`: product photography.

Some products still have photo placeholders. Rice-meal prices in `MenuCatalog.cs` are provisional and should be confirmed with the cafe before operational use.

MIT — see [LICENSE](LICENSE).

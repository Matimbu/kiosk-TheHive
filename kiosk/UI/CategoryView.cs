using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kiosk.UI
{
    /// <summary>
    /// The scrolling two-column grid of menu tiles. One instance is reused for
    /// every category, so switching tabs only swaps the contents.
    /// </summary>
    public class CategoryView : ScrollHost
    {
        private const int TileW = 226;
        private const int TileH = 214;
        private const int ColGap = 4;

        private string _category;

        public event EventHandler<ProductEventArgs> ProductChosen;

        public CategoryView()
        {
            BackColor = Hive.Canvas;
        }

        public string Category { get { return _category; } }

        public void Load(string category)
        {
            _category = category;

            foreach (Control c in Content.Controls.Cast<Control>().ToList())
            {
                Content.Controls.Remove(c);
                c.Dispose();
            }

            List<MenuProduct> items = MenuCatalog.InCategory(category).ToList();

            if (items.Count == 0)
            {
                EmptyState empty = new EmptyState(
                    category + " is on the way",
                    "This part of the menu is not ready yet. Ask our barista, or pick another tab above.");
                empty.Size = new Size(Width, Math.Max(300, Height - 20));
                empty.Location = Point.Empty;
                Content.Controls.Add(empty);
                Measure(0);
                return;
            }

            int top = 8;

            for (int i = 0; i < items.Count; i++)
            {
                int col = i % 2;
                int row = i / 2;

                ProductTile tile = new ProductTile(items[i]);
                tile.Size = new Size(TileW, TileH);
                tile.Location = new Point(12 + col * (TileW + ColGap), top + row * (TileH + ColGap));
                tile.Chosen += (s, e) =>
                {
                    EventHandler<ProductEventArgs> handler = ProductChosen;
                    if (handler != null) handler(this, e);
                };
                Hook(tile);
                Content.Controls.Add(tile);
            }

            Measure(Hive.Gap);
        }
    }
}

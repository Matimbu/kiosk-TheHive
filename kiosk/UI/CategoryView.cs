using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace kiosk.UI
{
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

            // designer never runs Load(), so sketch the grid instead of showing nothing
            Content.Paint += (s, e) =>
            {
                if (DesignMode && Content.Controls.Count == 0) PaintDesignPreview(e.Graphics);
            };
        }

        private void PaintDesignPreview(Graphics g)
        {
            Hive.Smooth(g);

            int rows = Math.Max(1, (Height - 8) / (TileH + ColGap));
            using (Pen p = new Pen(Hive.Line, 1.2f))
            {
                p.DashPattern = new float[] { 4f, 4f };
                for (int i = 0; i < rows * 2; i++)
                {
                    int col = i % 2, row = i / 2;
                    g.DrawRectangle(p,
                        14 + col * (TileW + ColGap),
                        10 + row * (TileH + ColGap),
                        TileW - 5, TileH - 8);
                }
            }

            Hive.Text(g, "Menu tiles are built at run time from MenuCatalog", Hive.Caption,
                      new Rectangle(0, 10 + rows * (TileH + ColGap) + 8, Width, 40), Hive.Muted,
                      TextFormatFlags.HorizontalCenter | TextFormatFlags.WordBreak);
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

            // Sold-out items stay on the menu, dimmed, and settle to the end of the
            // category. InCategory already sorted photographed items first and
            // OrderBy is stable, so that order survives inside each group.
            List<MenuProduct> items = MenuCatalog.InCategory(category)
                .OrderBy(p => LocalStore.SoldOut.Contains(p.Name) ? 1 : 0)
                .ToList();

            if (items.Count == 0)
            {
                EmptyState empty = new EmptyState(
                    "Nothing here yet",
                    "Try another category.");
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
                tile.SoldOut = LocalStore.SoldOut.Contains(items[i].Name);
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

using System;
using System.Drawing;
using System.Windows.Forms;

namespace kiosk.UI
{
    /// <summary>
    /// A standalone window for one menu category. The main kiosk flow shows
    /// categories inside <c>menuPage</c>; this exists so each category can
    /// still be opened - and previewed in the designer - on its own.
    /// </summary>
    public class CategoryForm : Form
    {
        private readonly AppHeader _header = new AppHeader();
        private readonly CategoryView _grid = new CategoryView();
        private readonly string _category;

        public CategoryForm() : this("Best Sellers") { }

        public CategoryForm(string category)
        {
            _category = category;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(Hive.ScreenW, Hive.ScreenH);
            BackColor = Hive.Canvas;
            Font = Hive.Body;

            MenuCategory meta = MenuCatalog.CategoryOf(category);
            _header.Title = category;
            _header.Subtitle = meta != null ? meta.Tagline : null;
            _header.ShowBack((s, e) => { if (Shell.Current != null) Nav.Back(); else Close(); });

            _grid.Dock = DockStyle.Fill;
            _grid.ProductChosen += (s, e) =>
            {
                Nav.Go(new ProductSheet(e.Item));
            };

            Controls.Add(_grid);
            Controls.Add(_header);

            Load += (s, e) => _grid.Load(_category);
        }

        protected CategoryView Grid { get { return _grid; } }
        protected AppHeader Header { get { return _header; } }
    }
}

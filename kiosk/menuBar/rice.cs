using kiosk.UI;

namespace kiosk
{
    /// <summary>Standalone window for the Rice Meals tab. Content comes from MenuCatalog.</summary>
    public partial class rice : CategoryForm
    {
        public rice() : base("Rice Meals")
        {
            InitializeComponent();
        }
    }
}

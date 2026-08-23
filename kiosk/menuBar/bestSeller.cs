using kiosk.UI;

namespace kiosk
{
    /// <summary>Standalone window for the Best Sellers tab. Content comes from MenuCatalog.</summary>
    public partial class bestSeller : CategoryForm
    {
        public bestSeller() : base("Best Sellers")
        {
            InitializeComponent();
        }
    }
}

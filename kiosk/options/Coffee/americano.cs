using kiosk.UI;

namespace kiosk
{
    /// <summary>Order sheet for Americano. Pricing and copy come from MenuCatalog.</summary>
    public partial class americano : ProductSheet
    {
        public americano() : base("Americano")
        {
            InitializeComponent();
        }
    }
}

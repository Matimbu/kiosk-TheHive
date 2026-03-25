using kiosk.options.Rice;
using System;
using System.Windows.Forms;

namespace kiosk
{
    public partial class rice : Form
    {
        public rice()
        {
            InitializeComponent();
        }

        private void btn_caramelizedChicken_Click(object sender, EventArgs e)
        {
            new caramelizedChicken().Show();
        }

        private void btn_beefTapa_Click(object sender, EventArgs e)
        {
            new beefTapa().Show();
        }

        private void btn_hungarian_Click(object sender, EventArgs e)
        {
            new hungarian().Show();
        }

        private void btn_bakedBangus_Click(object sender, EventArgs e)
        {
            new bakedBangus().Show();
        }

        // button4 = Ham & Egg
        private void button4_Click(object sender, EventArgs e)
        {
            new hamEgg().Show();
        }

        // button2 = Ham & Egg (duplicate panel — wire same)
        private void button2_Click(object sender, EventArgs e)
        {
            new hamEgg().Show();
        }

        // button1 = Garlic Longanisa (panel9)
        private void button1_Click(object sender, EventArgs e)
        {
            new garlicLonganisa().Show();
        }

        // button3 = Giant Pork Tonkatsu
        private void button3_Click(object sender, EventArgs e)
        {
            new giantPorkTonkatsu().Show();
        }

        // Waffle button (panel8 - mislabelled in designer but it's Garlic Longanisa there;
        // we'll open waffle since that's what it was intended for)
        private void Waffle_Click(object sender, EventArgs e)
        {
            new waffle().Show();
        }

        private void vScrollBar1_Scroll(object sender, System.Windows.Forms.ScrollEventArgs e) { }
    }
}

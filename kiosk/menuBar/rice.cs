using kiosk.options;
using kiosk.options.Rice;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }

        private void btn_beefTapa_Click(object sender, EventArgs e)
        {
            beefTapa beefTapa = new beefTapa();
            beefTapa.Show();
        }
    }
}

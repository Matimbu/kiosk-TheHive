using kiosk.options;
using kiosk.options.Rice;
using System;
using System.Windows.Forms;

namespace kiosk
{
    public partial class option3 : Form
    {
        public option3()
        {
            InitializeComponent();
        }

        private void btn_cafeLatte_Click(object sender, EventArgs e)
        {
            new cafeLatte().Show();
        }

        private void btn_americano_Click(object sender, EventArgs e)
        {
            new americano().Show();
        }

        private void btn_hazelnutAmericano_Click(object sender, EventArgs e)
        {
            new hazelnutAmericano().Show();
        }

        // button1 = Caramel Macchiato
        private void button1_Click(object sender, EventArgs e)
        {
            new caramelMacchiato().Show();
        }

        // button3 = Cafe Tiramisu
        private void button3_Click(object sender, EventArgs e)
        {
            new cafeTiramisu().Show();
        }

        // button2 = Cream Cafe Mocha
        private void button2_Click(object sender, EventArgs e)
        {
            new creamCafeMocha().Show();
        }
    }
}

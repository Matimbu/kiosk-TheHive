using kiosk.options;
using kiosk.options.Rice;
using System;
using System.Windows.Forms;

namespace kiosk
{
    public partial class option4 : Form
    {
        public option4()
        {
            InitializeComponent();
        }

        // btn_cafeLatte = Cafe Latte
        private void btn_cafeLatte_Click(object sender, EventArgs e)
        {
            new cafeLatte().Show();
        }

        // btn_Americano = Americano
        private void btn_Americano_Click(object sender, EventArgs e)
        {
            new americano().Show();
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

        // button4 = Biscoff Latte
        private void button4_Click(object sender, EventArgs e)
        {
            new biscoffLatte().Show();
        }

        // Waffle
        private void Waffle_Click(object sender, EventArgs e)
        {
            new waffle().Show();
        }
    }
}

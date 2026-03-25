using kiosk.options;
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
    public partial class option3 : Form
    {
        //private hazelnutAmericano hazelnutAmericano;

        public option3()
        {
            InitializeComponent();

            //hazelnutAmericano = new hazelnutAmericano();
        }

        private void btn_cafeLatte_Click(object sender, EventArgs e)
        {
            cafeLatte cafeLatteOption = new cafeLatte();

            cafeLatteOption.Show(); ;
        }

        private void btn_americano_Click(object sender, EventArgs e)
        {
            americano americanoOption = new americano();

            americanoOption.Show();
        }

        private void btn_hazelnutAmericano_Click(object sender, EventArgs e)
        {
            hazelnutAmericano hazelnutAmericano = new hazelnutAmericano();

            hazelnutAmericano.Show();
        }
    }
}

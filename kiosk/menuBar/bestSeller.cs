using kiosk.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace kiosk
{
    public partial class bestSeller : Form
    {
        

        public bestSeller()
        {
            InitializeComponent();

        }


        private void btn_cafeLatte_Click(object sender, EventArgs e)
        {
            cafeLatte cafeLatteOption = new cafeLatte();

            cafeLatteOption.Show();
        }





        //var optionsForm = new FormProductOptions();
        //if (optionsForm.ShowDialog() == DialogResult.OK)
        //{
        //    string name = optionsForm.ProductName;
        //    string size = optionsForm.selectedSize;
        //    int quantity = optionsForm.quantity;
        //    string ProductName = "Caffe Latte";


        //    Image productImage = pictureBoxCafeLatte.Image;
        //    string productName = "Coffee";





        //private void AddToOrderList(string item, string size, int quantity)
        //{
        //    ListViewItem item = new ListViewItem(productName);
        //    item.SubItems.Add(size);
        //    item.SubItems.Add(quantity.ToString());

        //    listViewOrders.Items.Add(item);
        //}




        private void btn_Americano_Click(object sender, EventArgs e)
        {
            americano AmericanoOption = new americano();

            AmericanoOption.Show();
        }

        
    }
}

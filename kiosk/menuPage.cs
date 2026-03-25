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
    public partial class menuPage : Form
    {
        public string selectedSize { get; private set; }
        public string product { get; private set; }
        public string temperature { get; private set; }
        public int quantity { get; private set; }

        
        public menuPage()
        {
            InitializeComponent();

            OrderStorage.ClearOrders(); // clear orders if using a shared storage

            // Add a hidden dummy control to prevent unwanted auto-focus
            TextBox hiddenDummy = new TextBox();
            hiddenDummy.Visible = false;
            hiddenDummy.TabStop = false;
            this.Controls.Add(hiddenDummy);

            this.Load += (s, e) =>
            {
                this.ActiveControl = hiddenDummy;
            };
        }

        public void loadform(object Form)
        {
            if (this.show_panel.Controls.Count > 0)
                this.show_panel.Controls.RemoveAt(0);

            Form f = Form as Form;
            f.TopLevel = false;
            f.Dock = DockStyle.Fill;
            this.show_panel.Controls.Add(f);
            this.show_panel.Tag = f;
            f.Show();
        }
        private void buttonViewOrder_Click_Click(object sender, EventArgs e)
        {
            viewOrder orderListForm = new viewOrder();
            orderListForm.Show();
        }
        private void viewOrder_Load(object sender, EventArgs e)
        {


        }

        private void btn_bestSeller_Click(object sender, EventArgs e)
        {
            loadform(new bestSeller());
        }
        private void btn_coffee_Click(object sender, EventArgs e)
        {
            loadform(new Coffee());
        }
        private void btn_rice_Click(object sender, EventArgs e)
        {
            loadform(new rice());
        }

        private void btn_pasta_Click(object sender, EventArgs e)
        {

        }

        private void btn_barchow_Click(object sender, EventArgs e)
        {
            loadform(new option2());
        }
    }
}

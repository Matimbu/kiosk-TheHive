using kiosk.Payments;
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
    public partial class viewOrder : Form
    {


        public viewOrder()
        {
            InitializeComponent();

            if (listBoxOrders.Columns.Count == 0)
            {
                listBoxOrders.View = View.Details;
                listBoxOrders.Columns.Add("Item", 140, HorizontalAlignment.Left);
                listBoxOrders.Columns.Add("Size & Temp", 100, HorizontalAlignment.Left);
                listBoxOrders.Columns.Add("Qty", 40, HorizontalAlignment.Center);
                listBoxOrders.Columns.Add("Total", 80, HorizontalAlignment.Right);
            }

            listBoxOrders.LabelEdit = false;
            
            listBoxOrders.ColumnWidthChanging += listBoxOrders_ColumnWidthChanging;
            listBoxOrders.HeaderStyle = ColumnHeaderStyle.Nonclickable;

            listBoxOrders.FullRowSelect = true;
            listBoxOrders.MultiSelect = false;
            listBoxOrders.Click += listBoxOrders_Click;

            OrderStorage.OrdersUpdated += UpdateOrderList;
            UpdateOrderList();
        }

        private void UpdateOrderList()
        {
            listBoxOrders.Items.Clear();

            foreach (var order in OrderStorage.Orders)
            {
                decimal total = order.Price * order.Quantity;

                var item = new ListViewItem(order.Product);
                item.SubItems.Add(order.SizeTemp);
                item.SubItems.Add(order.Quantity.ToString());
                item.SubItems.Add($"₱{total:F2}");

                listBoxOrders.Items.Add(item);
            }

            labelTotal.Text = $"Total: ₱{OrderStorage.GetTotal():F2}";
        }
        protected override void OnFormClosed(FormClosedEventArgs e)
        {          
            OrderStorage.OrdersUpdated -= UpdateOrderList;
            base.OnFormClosed(e);
        }

        private void button_menu_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void listBoxOrders_Click(object sender, EventArgs e)
        {
            if (listBoxOrders.SelectedItems.Count == 0)
                return;

            int index = listBoxOrders.SelectedItems[0].Index;

            if (index < 0 || index >= OrderStorage.Orders.Count)
                return;

            Order selectedOrder = OrderStorage.Orders[index];

            using (EditOrderForm editForm = new EditOrderForm(selectedOrder))
            {
                if (editForm.ShowDialog() == DialogResult.OK)
                {
                    if (editForm.IsRemoved)
                    {
                        OrderStorage.Orders.RemoveAt(index);
                        listBoxOrders.Items.RemoveAt(index);
                    }
                    else if (editForm.UpdatedOrder != null)
                    {
                        OrderStorage.Orders[index] = editForm.UpdatedOrder;
                    }

                    UpdateOrderList();
                }
            }
        }
        private void listBoxOrders_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            e.Cancel = true; 
            e.NewWidth = listBoxOrders.Columns[e.ColumnIndex].Width; 
        }

        private void btn_paymentMethod_Click(object sender, EventArgs e)
        {
            List<Order> currentOrders = OrderStorage.GetOrders(); 
            decimal totalAmount = OrderStorage.GetTotalPrice();

            PaymentSelectionForm paymentForm = new PaymentSelectionForm(currentOrders, totalAmount);
            paymentForm.Show();
        }     
    }    
}




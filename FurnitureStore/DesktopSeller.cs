using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class DesktopSeller : Form
    {
        public DesktopSeller()
        {
            InitializeComponent();
            AutoLockManager.StartMonitoring();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Product Product = new Product(3);
            this.Visible = false;
            Product.ShowDialog();
            this.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Orders Orders = new Orders(3);
            this.Visible = false;
            Orders.ShowDialog();
            this.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clents Clents = new Clents();
            this.Visible = false;
            Clents.ShowDialog();
            this.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}

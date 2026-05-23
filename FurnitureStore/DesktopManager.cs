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
    public partial class DesktopManager : Form
    {
        public DesktopManager()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Product Product = new Product(2);
            this.Visible = false;
            Product.ShowDialog();
            this.Visible = true;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы действительно хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void buttonSupplier_Click(object sender, EventArgs e)
        {
            Supplier Supplier = new Supplier();
            this.Visible = false;
            Supplier.ShowDialog();
            this.Visible = true;
        }

        private void buttonCategory_Click(object sender, EventArgs e)
        {
            Category Category = new Category();
            this.Visible = false;
            Category.ShowDialog();
            this.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Orders Order = new Orders(2);
            this.Visible = false;
            Order.ShowDialog();
            this.Visible = true;
        }
    }
}

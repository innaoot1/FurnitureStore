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
    public partial class DesktopAdministrator : Form
    {
        public DesktopAdministrator()
        {
            InitializeComponent();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Worker Worker = new Worker();
            this.Visible = false;
            Worker.ShowDialog();
            this.Visible = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Role Role = new Role();
            this.Visible = false;
            Role.ShowDialog();
            this.Visible = true;
        }
    }
}

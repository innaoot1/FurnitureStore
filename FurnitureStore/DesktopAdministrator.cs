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
            AutoLockManager.StartMonitoring();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void button8_Click(object sender, EventArgs e)
        {
            Worker Worker = new Worker();
            this.Visible = false;
            Worker.ShowDialog();
            this.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ManagementDB ManagementDB = new ManagementDB();
            this.Visible = false;
            ManagementDB.ShowDialog();
            this.Visible = true;
        }
    }
}

using MySql.Data.MySqlClient;
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
    public partial class Role : Form
    {
        private DataTable roleTable;
        public Role()
        {
            InitializeComponent();
        }

        private void Role_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            RoleName AS 'Роль'
                        FROM Role;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    roleTable = new DataTable();
                    da.Fill(roleTable);
                    dataGridView1.DataSource = roleTable;

                    label3.Text = $"Всего: {roleTable.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            if (roleTable == null) return;

            string searchText = textBoxSearch.Text.Trim().Replace("'", "''");

            DataView view = new DataView(roleTable);
            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
                filter = $"[Роль] LIKE '%{searchText}%'";

            view.RowFilter = filter;
            dataGridView1.DataSource = view;

            label3.Text = $"Всего: {view.Count}";
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }
    }
}

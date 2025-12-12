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
    public partial class Category : Form
    {
        private DataTable categoryTable;
        public Category()
        {
            InitializeComponent();
        }

        private void Category_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            CategoryName AS 'Категории товаров'
                        FROM Category;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    categoryTable = new DataTable();
                    da.Fill(categoryTable);
                    dataGridView1.DataSource = categoryTable;

                    label3.Text = $"Всего: {categoryTable.Rows.Count}";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            if (categoryTable == null) return;

            string searchText = textBoxSearch.Text.Trim().Replace("'", "''");

            DataView view = new DataView(categoryTable);
            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
                filter = $"[Категории товаров] LIKE '%{searchText}%'"; 

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
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            Insert form = new Insert("Category", "add");
            form.ShowDialog();
            Category_Load(null, null);
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

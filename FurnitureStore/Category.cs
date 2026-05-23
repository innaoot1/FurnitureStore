using MySql.Data.MySqlClient;
using System;
using System.Data;
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
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            CategoryID,
                            CategoryName AS 'Категории товаров'
                        FROM Category;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    categoryTable = new DataTable();
                    da.Fill(categoryTable);
                    dataGridView1.DataSource = categoryTable;

                    label3.Text = $"Всего: {categoryTable.Rows.Count}";

                    if (dataGridView1.Columns.Contains("CategoryID"))
                        dataGridView1.Columns["CategoryID"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            CategoryInsert form = new CategoryInsert("add", 0, "", this);
            form.ShowDialog();
            LoadCategories();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите категорию для редактирования!", "Внимание",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["CategoryID"].Value);
            string name = dataGridView1.CurrentRow.Cells["Категории товаров"].Value.ToString();

            CategoryInsert form = new CategoryInsert("edit", id, name, this);
            form.ShowDialog();
            LoadCategories();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                buttonUpdate.Enabled = true;
            }
        }
    }
}
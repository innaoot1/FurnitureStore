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
    public partial class Supplier : Form
    {
        private DataTable supplierTable;
        public Supplier()
        {
            InitializeComponent();
        }

        private void Supplier_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            SupplierID,
                            SupplierName AS 'Поставщик',
                            IsActive AS 'Активен'
                        FROM Supplier
                        WHERE IsActive = 1;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    supplierTable = new DataTable();
                    da.Fill(supplierTable);
                    dataGridView1.DataSource = supplierTable;

                    label3.Text = $"Всего: {supplierTable.Rows.Count}";

                    if (dataGridView1.Columns.Contains("SupplierID"))
                        dataGridView1.Columns["SupplierID"].Visible = false;
                    if (dataGridView1.Columns.Contains("Активен"))
                        dataGridView1.Columns["Активен"].Visible = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyFilters()
        {
            if (supplierTable == null) return;

            string searchText = textBoxSearch.Text.Trim().Replace("'", "''");

            DataView view = new DataView(supplierTable);
            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
                filter = $"[Поставщик] LIKE '%{searchText}%'";

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

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int supplierId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["SupplierID"].Value);
            string name = dataGridView1.CurrentRow.Cells["Поставщик"].Value.ToString();

            bool hasProducts = CheckIfSupplierHasProducts(supplierId);

            string message = hasProducts
                ? $"Поставщик \"{name}\" имеет товары в базе. При удалении:\n- Существующие товары сохранят название поставщика\n- Новые товары нельзя будет создать с этим поставщиком\n\nПродолжить удаление?"
                : $"Удалить поставщика \"{name}\"?";

            DialogResult result = MessageBox.Show(message, "Удаление поставщика",
                MessageBoxButtons.YesNo, hasProducts ? MessageBoxIcon.Warning : MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                    {
                        con.Open();

                        MySqlCommand cmd = new MySqlCommand(
                            "UPDATE Supplier SET IsActive = 0 WHERE SupplierID = @id", con);
                        cmd.Parameters.AddWithValue("@id", supplierId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Поставщик \"{name}\" удален!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Supplier_Load(null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool CheckIfSupplierHasProducts(int supplierId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM Product WHERE ProductSupplier = @supplierId", con);
                    cmd.Parameters.AddWithValue("@supplierId", supplierId);

                    int productCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return productCount > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["SupplierID"].Value);
            string name = dataGridView1.CurrentRow.Cells["Поставщик"].Value.ToString();

            Insert form = new Insert("Supplier", "edit", id, name);
            form.ShowDialog();
            Supplier_Load(null, null);
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            Insert form = new Insert("Supplier", "add");
            form.ShowDialog();
            Supplier_Load(null, null);
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
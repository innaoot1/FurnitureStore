using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class Product : Form
    {
        private int roleId;
        private DataTable productTable;
        private Dictionary<string, Image> imageCache = new Dictionary<string, Image>();
        private Image plugImage;

        public Product(int role)
        {
            InitializeComponent();
            roleId = role;
            ConfigureButtons();
            LoadPlugImage();

        }

        private void LoadPlugImage()
        {
            string plugPath = ProductImageManager.Instance.GetPlugImagePath();
            if (File.Exists(plugPath))
            {
                try
                {
                    plugImage = ProductImageManager.Instance.LoadImageFromFile(plugPath);
                }
                catch (Exception)
                {
                    plugImage = null;
                }
            }
        }

        private void ConfigureButtons()
        {
            if (roleId == 2)
            {
                buttonCreate.Visible = true;
                buttonUpdate.Visible = true;
                buttonDelete.Visible = true;
            }
        }

        private void Product_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadProducts();
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmdCategories = new MySqlCommand("SELECT CategoryName FROM Category", con);
                    MySqlDataReader readerCategories = cmdCategories.ExecuteReader();

                    comboBoxFilter.Items.Clear();
                    comboBoxFilter.Items.Add("");
                    while (readerCategories.Read())
                    {
                        comboBoxFilter.Items.Add(readerCategories.GetString("CategoryName"));
                    }
                    readerCategories.Close();

                    comboBoxSort.Items.Clear();
                    comboBoxSort.Items.Add("");
                    comboBoxSort.Items.Add("По возрастанию");
                    comboBoxSort.Items.Add("По убыванию");

                    comboBoxFilter.SelectedIndex = 0;
                    comboBoxSort.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных комбобоксов: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT 
                            p.ProductID,
                            p.ProductName AS 'Товар',
                            c.CategoryName AS 'Категория',
                            p.ProductPrice AS 'Цена',
                            p.ProductManufacturer AS 'Производитель',
                            p.OriginalSupplierName AS 'Поставщик', 
                            p.ProductQuantityInStock AS 'Количество',
                            p.ProductDescription AS 'Описание',
                            p.ProductPhoto
                        FROM Product p
                        JOIN Category c ON p.ProductCategory = c.CategoryId
                        WHERE p.IsActive = 1;";

                    MySqlDataAdapter da = new MySqlDataAdapter(query, con);
                    productTable = new DataTable();
                    da.Fill(productTable);

                    DataView dataView = new DataView(productTable);
                    dataGridView1.DataSource = dataView;

                    label5.Text = $"Всего: {dataView.Count}";

                    if (dataGridView1.Columns.Contains("ProductID"))
                        dataGridView1.Columns["ProductID"].Visible = false;

                    if (dataGridView1.Columns.Contains("ProductPhoto"))
                        dataGridView1.Columns["ProductPhoto"].Visible = false;

                    LoadImagesToGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadImagesToGrid()
        {
            BeginInvoke(new Action(async () =>
            {
                await LoadImagesGraduallyAsync();
            }));
        }

        private void SetPlugImagesToAllRows()
        {
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].IsNewRow) continue;

                if (dataGridView1.Rows[i].Cells["ColumnImage"].Value == null)
                {
                    dataGridView1.Rows[i].Cells["ColumnImage"].Value = plugImage;
                }
            }
        }

        private async Task LoadImagesGraduallyAsync()
        {
            SetPlugImagesToAllRows();

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (dataGridView1.Rows[i].IsNewRow) continue;

                string photoHash = dataGridView1.Rows[i].Cells["ProductPhoto"].Value?.ToString();

                if (!string.IsNullOrEmpty(photoHash))
                {
                    Image image = await ProductImageManager.Instance.LoadImageByHashAsync(photoHash);
                    if (image != null)
                    {
                        if (dataGridView1.InvokeRequired)
                        {
                            dataGridView1.Invoke(new Action<int, Image>((rowIndex, img) =>
                            {
                                SafeSetCellImage(rowIndex, img);
                            }), i, image);
                        }
                        else
                        {
                            SafeSetCellImage(i, image);
                        }
                    }
                }

                await Task.Delay(30);
            }
        }

        private void SafeSetCellImage(int rowIndex, Image image)
        {
            try
            {
                if (rowIndex < dataGridView1.Rows.Count &&
                    !dataGridView1.Rows[rowIndex].IsNewRow &&
                    dataGridView1.Rows[rowIndex].Cells["ColumnImage"] != null)
                {
                    dataGridView1.Rows[rowIndex].Cells["ColumnImage"].Value = image;
                }
            }
            catch (Exception)
            {
            }
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (productTable == null) return;

            string searchText = textBoxSearch.Text.Trim().Replace("'", "''");
            string selectedCategory = comboBoxFilter.SelectedItem?.ToString() ?? "";
            string sortOption = comboBoxSort.SelectedItem?.ToString() ?? "";

            DataView view = dataGridView1.DataSource as DataView;
            if (view == null) return;

            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
            {
                filter = $"[Товар] LIKE '%{searchText}%' OR [Описание] LIKE '%{searchText}%'";
            }

            if (!string.IsNullOrEmpty(selectedCategory))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += $"[Категория] = '{selectedCategory}'";
            }

            view.RowFilter = filter;

            if (sortOption == "По возрастанию")
                view.Sort = "[Цена] ASC";
            else if (sortOption == "По убыванию")
                view.Sort = "[Цена] DESC";
            else
                view.Sort = "";

            label5.Text = $"Всего: {view.Count}";

            LoadImagesToGrid();
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            try
            {
                textBoxSearch.Text = "";
                comboBoxFilter.SelectedIndex = 0;
                comboBoxSort.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при очистке фильтров: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            ProductInsert form = new ProductInsert("add");
            form.ShowDialog();
            LoadProducts();
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            DataGridViewRow row = dataGridView1.CurrentRow;

            string photoHash = GetProductPhotoHashFromDatabase(Convert.ToInt32(row.Cells["ProductID"].Value));

            ProductInsert form = new ProductInsert("edit");
            form.SetEditData(
                productId: Convert.ToInt32(row.Cells["ProductID"].Value),
                name: row.Cells["Товар"].Value?.ToString() ?? "",
                description: row.Cells["Описание"].Value?.ToString() ?? "",
                price: Convert.ToDecimal(row.Cells["Цена"].Value),
                manufacturer: row.Cells["Производитель"].Value?.ToString() ?? "",
                category: row.Cells["Категория"].Value?.ToString() ?? "",
                supplier: row.Cells["Поставщик"].Value?.ToString() ?? "",
                quantity: Convert.ToInt32(row.Cells["Количество"].Value),
                photoHash: photoHash
            );

            form.ShowDialog();
            LoadProducts();
        }

        private string GetProductPhotoHashFromDatabase(int productId)
        {
            string photoHash = "";

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT ProductPhoto FROM Product WHERE ProductID = @id", con);
                    cmd.Parameters.AddWithValue("@id", productId);

                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        photoHash = result.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при получении фото товара: " + ex.Message);
            }

            return photoHash;
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ProductID"].Value);
            string name = dataGridView1.CurrentRow.Cells["Товар"].Value.ToString();

            bool hasOrders = CheckIfProductHasOrders(id);

            string message = hasOrders
                ? $"Товар \"{name}\" используется в заказах. При удалении:\n- Существующие заказы сохранят данные товара\n- Новые заказы нельзя будет создать с этим товаром\n\nПродолжить удаление?"
                : $"Удалить товар \"{name}\"?";

            DialogResult res = MessageBox.Show(message, "Удаление товара",
                MessageBoxButtons.YesNo, hasOrders ? MessageBoxIcon.Warning : MessageBoxIcon.Question);

            if (res != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(
                        "UPDATE Product SET IsActive = 0 WHERE ProductID = @id", con);
                    cmd.Parameters.AddWithValue("@id", id);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Товар \"{name}\" удален!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadProducts();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка удаления",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckIfProductHasOrders(int productId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM OrderProduct WHERE ProductID = @productId", con);
                    cmd.Parameters.AddWithValue("@productId", productId);

                    int orderCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return orderCount > 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s-,]$"))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == dataGridView1.Columns["ColumnImage"].Index)
            {
                e.ThrowException = false;
            }
        }

        private void dataGridView1_Sorted(object sender, EventArgs e)
        {
            LoadImagesToGrid();
        }
    }
}
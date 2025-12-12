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
    public partial class ProductInsert : Form
    {
        private string mode;
        private string selectedImageName = null;
        private string selectedImageHash = null;
        private string oldImageHash = null;

        public int ProductID { get; set; }
        private int selectedCategoryId;
        private int selectedSupplierId;

        public new string ProductName
        {
            get => textBoxName.Text.Trim();
            set => textBoxName.Text = value;
        }
        public string ProductDescription
        {
            get => textBoxDescription.Text.Trim();
            set => textBoxDescription.Text = value;
        }
        public decimal ProductPrice
        {
            get => decimal.TryParse(textBoxPrice.Text, out decimal p) ? p : 0;
            set => textBoxPrice.Text = value.ToString("0.##");
        }
        public string ProductManufacturer
        {
            get => textBoxManufacturer.Text.Trim();
            set => textBoxManufacturer.Text = value;
        }

        public int ProductQuantity
        {
            get => int.TryParse(textBoxQuantityInStock.Text, out int q) ? q : 0;
            set => textBoxQuantityInStock.Text = value.ToString();
        }

        public string ProductPhotoHash
        {
            get => selectedImageHash;
            set => selectedImageHash = value;
        }

        public ProductInsert(string mode)
        {
            InitializeComponent();
            this.mode = mode;
            LoadComboBoxes();

            AutoLockManager.StartMonitoring();

            if (mode == "add")
            {
                LoadDefaultImage();
            }
        }

        private void LoadComboBoxes()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    comboBoxCategory.Items.Clear();
                    MySqlCommand cmdCat = new MySqlCommand("SELECT CategoryID, CategoryName FROM Category;", con);
                    MySqlDataReader readerCat = cmdCat.ExecuteReader();
                    while (readerCat.Read())
                    {
                        comboBoxCategory.Items.Add(new
                        {
                            Id = readerCat.GetInt32("CategoryID"),
                            Name = readerCat.GetString("CategoryName")
                        });
                    }
                    readerCat.Close();
                    comboBoxCategory.DisplayMember = "Name";
                    comboBoxCategory.ValueMember = "Id";

                    comboBoxSupplier.Items.Clear();
                    MySqlCommand cmdSup = new MySqlCommand("SELECT SupplierID, SupplierName FROM Supplier WHERE IsActive = 1;", con);
                    MySqlDataReader readerSup = cmdSup.ExecuteReader();
                    while (readerSup.Read())
                    {
                        comboBoxSupplier.Items.Add(new
                        {
                            Id = readerSup.GetInt32("SupplierID"),
                            Name = readerSup.GetString("SupplierName")
                        });
                    }
                    readerSup.Close();
                    comboBoxSupplier.DisplayMember = "Name";
                    comboBoxSupplier.ValueMember = "Id";

                    if (comboBoxCategory.Items.Count > 0) comboBoxCategory.SelectedIndex = 0;
                    if (comboBoxSupplier.Items.Count > 0) comboBoxSupplier.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetCategoryByName(string categoryName)
        {
            foreach (var item in comboBoxCategory.Items)
            {
                dynamic category = item;
                if (category.Name == categoryName)
                {
                    comboBoxCategory.SelectedItem = item;
                    selectedCategoryId = category.Id;
                    return;
                }
            }
            if (comboBoxCategory.Items.Count > 0)
                comboBoxCategory.SelectedIndex = 0;
        }

        private void SetSupplierByName(string supplierName)
        {
            foreach (var item in comboBoxSupplier.Items)
            {
                dynamic supplier = item;
                if (supplier.Name == supplierName)
                {
                    comboBoxSupplier.SelectedItem = item;
                    selectedSupplierId = supplier.Id;
                    return;
                }
            }
            if (comboBoxSupplier.Items.Count > 0)
                comboBoxSupplier.SelectedIndex = 0;
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxCategory.SelectedItem != null)
            {
                dynamic category = comboBoxCategory.SelectedItem;
                selectedCategoryId = category.Id;
            }
        }

        private void comboBoxSupplier_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxSupplier.SelectedItem != null)
            {
                dynamic supplier = comboBoxSupplier.SelectedItem;
                selectedSupplierId = supplier.Id;
            }
        }

        public void SetEditData(int productId, string name, string description, decimal price,
                      string manufacturer, string category, string supplier, int quantity, string photoHash)
        {
            ProductID = productId;
            ProductName = name;
            ProductDescription = description;
            ProductPrice = price;
            ProductManufacturer = manufacturer;
            ProductQuantity = quantity;
            selectedImageHash = photoHash;
            oldImageHash = photoHash;

            this.Load += (s, e) =>
            {
                SetCategoryByName(category);
                SetSupplierByName(supplier);
                LoadProductPhotoByHash(photoHash);
            };
        }

        private void LoadProductPhotoByHash(string photoHash)
        {
            if (string.IsNullOrWhiteSpace(photoHash))
            {
                LoadDefaultImage();
                return;
            }

            try
            {
                string imagePath = ProductImageManager.Instance.FindImageByHash(photoHash);

                if (imagePath != null && File.Exists(imagePath))
                {
                    byte[] imageData = File.ReadAllBytes(imagePath);
                    UpdatePictureBox(imageData);
                    selectedImageName = Path.GetFileName(imagePath);
                    selectedImageHash = photoHash;
                }
                else
                {
                    LoadDefaultImage();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки изображения товара: " + ex.Message);
                LoadDefaultImage();
            }
        }

        private void LoadDefaultImage()
        {
            try
            {
                string plugPath = ProductImageManager.Instance.GetPlugImagePath();
                if (plugPath != null && File.Exists(plugPath))
                {
                    byte[] imageData = File.ReadAllBytes(plugPath);
                    UpdatePictureBox(imageData);
                    selectedImageName = "plug.png";
                    selectedImageHash = ProductImageManager.Instance.CalculateImageHash(imageData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки изображения-заглушки: " + ex.Message);
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    string checkQuery = mode == "add"
                        ? "SELECT COUNT(*) FROM Product WHERE ProductName = @name"
                        : "SELECT COUNT(*) FROM Product WHERE ProductName = @name AND ProductID <> @id";

                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, con);
                    checkCmd.Parameters.AddWithValue("@name", ProductName);
                    if (mode == "edit") checkCmd.Parameters.AddWithValue("@id", ProductID);
                    int exists = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        MessageBox.Show("Товар с таким названием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    if (!string.IsNullOrEmpty(selectedImageHash) && selectedImageHash != oldImageHash)
                    {
                        MySqlCommand checkHashCmd = new MySqlCommand(
                            "SELECT COUNT(*) FROM Product WHERE ProductPhoto = @hash AND ProductID <> @id", con);
                        checkHashCmd.Parameters.AddWithValue("@hash", selectedImageHash);
                        checkHashCmd.Parameters.AddWithValue("@id", mode == "edit" ? ProductID : 0);

                        int hashCount = Convert.ToInt32(checkHashCmd.ExecuteScalar());
                        if (hashCount > 0)
                        {
                            MessageBox.Show("Данное изображение уже используется для другого товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    string getSupplierNameQuery = "SELECT SupplierName FROM Supplier WHERE SupplierID = @supplierId";
                    MySqlCommand getNameCmd = new MySqlCommand(getSupplierNameQuery, con);
                    getNameCmd.Parameters.AddWithValue("@supplierId", selectedSupplierId);
                    string currentSupplierName = getNameCmd.ExecuteScalar()?.ToString() ?? comboBoxSupplier.Text;

                    MySqlCommand cmd;
                    if (mode == "add")
                    {
                        cmd = new MySqlCommand(@"
                            INSERT INTO Product 
                            (ProductName, ProductDescription, ProductPrice, ProductManufacturer, 
                             ProductCategory, ProductSupplier, OriginalSupplierName, ProductQuantityInStock, ProductPhoto)
                            VALUES (@name, @desc, @price, @man, @category, @supplier, @originalSupplier, @qty, @photo)", con);
                    }
                    else
                    {
                        cmd = new MySqlCommand(@"
                            UPDATE Product
                            SET 
                                ProductName = @name,
                                ProductDescription = @desc,
                                ProductPrice = @price,
                                ProductManufacturer = @man,
                                ProductCategory = @category,
                                ProductSupplier = @supplier,
                                ProductQuantityInStock = @qty,
                                ProductPhoto = @photo
                            WHERE ProductID = @id", con);
                        cmd.Parameters.AddWithValue("@id", ProductID);
                    }

                    cmd.Parameters.AddWithValue("@name", ProductName);
                    cmd.Parameters.AddWithValue("@desc", ProductDescription);
                    cmd.Parameters.AddWithValue("@price", ProductPrice);
                    cmd.Parameters.AddWithValue("@man", ProductManufacturer);
                    cmd.Parameters.AddWithValue("@category", selectedCategoryId);
                    cmd.Parameters.AddWithValue("@supplier", selectedSupplierId);
                    cmd.Parameters.AddWithValue("@photo", selectedImageHash ?? "");
                    cmd.Parameters.AddWithValue("@qty", ProductQuantity);

                    if (mode == "add")
                    {
                        cmd.Parameters.AddWithValue("@originalSupplier", currentSupplierName);
                    }

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        string message = mode == "add" ? "добавлен" : "обновлён";
                        MessageBox.Show($"Товар \"{ProductName}\" успешно {message}!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.DialogResult = DialogResult.OK;
                    }
                    else
                    {
                        MessageBox.Show("Не удалось сохранить товар!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (MySqlException mysqlEx)
            {
                if (mysqlEx.Number == 1452)
                {
                    MessageBox.Show("Ошибка связи с выбранной категорией или поставщиком!", "Ошибка целостности данных", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Ошибка базы данных: {mysqlEx.Message}", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка сохранения", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(ProductName))
            {
                MessageBox.Show("Введите название товара!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxName.Focus(); return false;
            }

            if (string.IsNullOrWhiteSpace(ProductDescription))
            {
                MessageBox.Show("Введите описание!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxDescription.Focus(); return false;
            }

            if (ProductPrice <= 0)
            {
                MessageBox.Show("Введите корректную цену!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPrice.Focus(); return false;
            }

            if (comboBoxCategory.SelectedItem == null)
            {
                MessageBox.Show("Выберите категорию!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxCategory.Focus(); return false;
            }

            if (comboBoxSupplier.SelectedItem == null)
            {
                MessageBox.Show("Выберите поставщика!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxSupplier.Focus(); return false;
            }

            if (string.IsNullOrWhiteSpace(ProductManufacturer))
            {
                MessageBox.Show("Введите производителя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxManufacturer.Focus(); return false;
            }

            if (string.IsNullOrWhiteSpace(textBoxQuantityInStock.Text) || ProductQuantity < 0)
            {
                MessageBox.Show("Введите корректное количество на складе!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxQuantityInStock.Focus(); return false;
            }

            return true;
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s-,]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s-.,]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[0-9,]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxQuantityInStock_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                e.Handled = true;
        }

        private void textBoxManufacturer_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s-]$"))
            {
                e.Handled = true;
            }
        }

        private void buttonImage_Click(object sender, EventArgs e)
        {
            AutoLockManager.SuspendMonitoring();

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Изображения (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png";
                ofd.Title = "Выберите фото для товара";


                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (!ProductImageManager.Instance.ValidateImageFile(ofd.FileName))
                    {
                        MessageBox.Show("Недопустимый тип файла или размер превышает 3 МБ! Разрешены только JPG и PNG изображения.",
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    AutoLockManager.ResumeMonitoring();

                    try
                    {
                        byte[] imageData = File.ReadAllBytes(ofd.FileName);
                        string imageHash = ProductImageManager.Instance.CalculateImageHash(imageData);

                        using (var con = new MySqlConnection(connStr.ConnectionString))
                        {
                            con.Open();
                            using (var cmd = new MySqlCommand(
                                "SELECT ProductID FROM Product WHERE ProductPhoto = @hash AND ProductID != @id;", con))
                            {
                                cmd.Parameters.AddWithValue("@hash", imageHash);
                                cmd.Parameters.AddWithValue("@id", mode == "edit" ? ProductID : 0);
                                object exists = cmd.ExecuteScalar();
                                if (exists != null)
                                {
                                    MessageBox.Show("Данное изображение уже используется для другого товара!\nВыберите другое изображение.",
                                        "Изображение занято", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                    return;
                                }
                            }
                        }

                        string existingFileName = ProductImageManager.Instance.FindExistingImageByHash(imageHash);
                        string finalFileName;
                        string originalFileName = Path.GetFileNameWithoutExtension(ofd.FileName);
                        string extension = Path.GetExtension(ofd.FileName);

                        if (existingFileName != null)
                        {
                            finalFileName = existingFileName;
                        }
                        else
                        {
                            finalFileName = ProductImageManager.Instance.GenerateUniqueFileName(originalFileName, extension);
                            ProductImageManager.Instance.SaveImageToProductDirectory(imageData, finalFileName);
                        }

                        UpdatePictureBox(imageData);

                        selectedImageName = finalFileName;
                        selectedImageHash = imageHash;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка при выборе изображения: " + ex.Message,
                            "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdatePictureBox(byte[] imageData)
        {
            if (pictureBoxImage.Image != null)
            {
                pictureBoxImage.Image.Dispose();
                pictureBoxImage.Image = null;
            }

            using (var ms = new MemoryStream(imageData))
            {
                pictureBoxImage.Image = Image.FromStream(ms);
            }
        }

        private void ProductInsert_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (pictureBoxImage.Image != null)
            {
                pictureBoxImage.Image.Dispose();
                pictureBoxImage.Image = null;
            }
        }
    }
}
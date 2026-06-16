using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class Supplier : Form
    {
        private DataTable supplierTable;
        public Supplier()
        {
            InitializeComponent();
            AutoLockManager.StartMonitoring();

            KeyboardLayoutManager.AttachRussianLayout(textBoxSearch);
        }

        private void Supplier_Load(object sender, EventArgs e)
        {
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            SupplierID,
                            SupplierName AS 'Поставщик',
                            SupplierContactPerson AS 'Контактное лицо',
                            SupplierPhone AS 'Телефон',
                            SupplierAddress AS 'Адрес',
                            SupplierINN AS 'ИНН',
                            SupplierDescription AS 'Описание',
                            IsActive AS 'Активен'
                        FROM Supplier
                        WHERE IsActive = 1
                        ORDER BY SupplierName ASC;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    supplierTable = new DataTable();
                    da.Fill(supplierTable);
                    dataGridView1.DataSource = supplierTable;

                    label3.Text = $"Всего: {supplierTable.Rows.Count}";

                    if (dataGridView1.Columns.Contains("SupplierID"))
                        dataGridView1.Columns["SupplierID"].Visible = false;
                    if (dataGridView1.Columns.Contains("Активен"))
                        dataGridView1.Columns["Активен"].Visible = false;
                    if (dataGridView1.Columns.Contains("Описание"))
                        dataGridView1.Columns["Описание"].Visible = false;

                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

                    if (dataGridView1.Columns.Contains("Поставщик"))
                        dataGridView1.Columns["Поставщик"].FillWeight = 30;
                    if (dataGridView1.Columns.Contains("Контактное лицо"))
                        dataGridView1.Columns["Контактное лицо"].FillWeight = 25;
                    if (dataGridView1.Columns.Contains("Телефон"))
                        dataGridView1.Columns["Телефон"].FillWeight = 20;
                    if (dataGridView1.Columns.Contains("Адрес"))
                        dataGridView1.Columns["Адрес"].FillWeight = 25;
                    if (dataGridView1.Columns.Contains("ИНН"))
                        dataGridView1.Columns["ИНН"].FillWeight = 15;
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
                filter = $"[Поставщик] LIKE '%{searchText}%' OR [Контактное лицо] LIKE '%{searchText}%'";

            view.RowFilter = filter;
            dataGridView1.DataSource = view;

            label3.Text = $"Всего: {view.Count}";
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
            LoadSuppliers();
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
                    using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
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
                            LoadSuppliers();
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
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
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
            var supplierData = GetSupplierData(id);

            SupplierInsert form = new SupplierInsert("edit", this)
            {
                SupplierID = id,
                SupplierName = supplierData.Name,
                ContactPerson = supplierData.ContactPerson,
                SupplierPhone = supplierData.Phone,
                SupplierAddress = supplierData.Address,
                SupplierINN = supplierData.INN,
                SupplierDescription = supplierData.Description
            };
            form.ShowDialog();
            LoadSuppliers();
        }

        private (string Name, string ContactPerson, string Phone, string Address, string INN, string Description) GetSupplierData(int supplierId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            SupplierName,
                            SupplierContactPerson,
                            SupplierPhone,
                            SupplierAddress,
                            SupplierINN,
                            SupplierDescription
                        FROM Supplier 
                        WHERE SupplierID = @id", con);
                    cmd.Parameters.AddWithValue("@id", supplierId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                reader.GetString("SupplierName"),
                                reader.IsDBNull(reader.GetOrdinal("SupplierContactPerson")) ? "" : reader.GetString("SupplierContactPerson"),
                                reader.IsDBNull(reader.GetOrdinal("SupplierPhone")) ? "" : reader.GetString("SupplierPhone"),
                                reader.IsDBNull(reader.GetOrdinal("SupplierAddress")) ? "" : reader.GetString("SupplierAddress"),
                                reader.IsDBNull(reader.GetOrdinal("SupplierINN")) ? "" : reader.GetString("SupplierINN"),
                                reader.IsDBNull(reader.GetOrdinal("SupplierDescription")) ? "" : reader.GetString("SupplierDescription")
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных поставщика: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return ("", "", "", "", "", "");
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            SupplierInsert form = new SupplierInsert("add", this);
            form.ShowDialog();
            LoadSuppliers();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            string text = e.Value.ToString();

            if (columnName == "Контактное лицо")
            {
                e.Value = FormatFullName(text);
            }
            else if (columnName == "Телефон")
            {
                string phone = new string(text.Where(char.IsDigit).ToArray());
                if (phone.Length == 11 && phone.StartsWith("7"))
                {
                    e.Value = FormatPhoneNumber(text);
                }
            }
            else if (columnName == "ИНН")
            {
                e.Value = FormatINN(text);
            }
            else if (columnName == "Адрес")
            {
                e.Value = FormatAddress(text);
            }
        }

        private string FormatFullName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length >= 3)
            {
                return $"{nameParts[0]} {nameParts[1][0]}.{nameParts[2][0]}.";
            }
            else if (nameParts.Length == 2)
            {
                return $"{nameParts[0]} {nameParts[1][0]}.";
            }
            else
            {
                return fullName;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            string numbersOnly = new string(phoneNumber.Where(char.IsDigit).ToArray());

            if (numbersOnly.Length == 11 && numbersOnly.StartsWith("7"))
            {
                string prefix = "+7";
                string hiddenFirst = "***";
                string hiddenSecond = "***";
                string lastFour = numbersOnly.Substring(numbersOnly.Length - 4);
                string formattedLast = $"{lastFour.Substring(0, 2)}-{lastFour.Substring(2)}";

                return $"{prefix}({hiddenFirst}) {hiddenSecond}-{formattedLast}";
            }
            else
            {
                return phoneNumber;
            }
        }

        private string FormatINN(string inn)
        {
            if (string.IsNullOrEmpty(inn))
                return string.Empty;

            string digitsOnly = new string(inn.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length >= 10)
            {
                string firstFour = digitsOnly.Substring(0, 4);
                string lastSix = digitsOnly.Substring(digitsOnly.Length - 6);
                return $"{firstFour} {lastSix.Substring(0, 3)} ***";
            }
            else if (digitsOnly.Length >= 6)
            {
                string firstFour = digitsOnly.Substring(0, Math.Min(4, digitsOnly.Length));
                return $"{firstFour} *** ***";
            }
            else
            {
                return "**** ******";
            }
        }

        private string FormatAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
                return string.Empty;

            int commaIndex = address.IndexOf(',');
            if (commaIndex > 0)
            {
                string city = address.Substring(0, commaIndex).Trim();
                string hiddenPart = new string('*', 10);
                return $"{city}{hiddenPart}";
            }
            else
            {
                string visiblePart = address.Length > 5
                    ? address.Substring(0, 5)
                    : address;
                string hiddenPart = new string('*', 10);
                return $"{visiblePart}{hiddenPart}";
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                SupplierInsert form = new SupplierInsert("view", this)
                {
                    SupplierName = row.Cells["Поставщик"].Value?.ToString() ?? "",
                    ContactPerson = row.Cells["Контактное лицо"].Value?.ToString() ?? "",
                    SupplierPhone = row.Cells["Телефон"].Value?.ToString() ?? "",
                    SupplierAddress = row.Cells["Адрес"].Value?.ToString() ?? "",
                    SupplierINN = row.Cells["ИНН"].Value?.ToString() ?? "",
                    SupplierDescription = row.Cells["Описание"].Value?.ToString() ?? ""
                };
                form.ShowDialog();
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                buttonUpdate.Enabled = true;
                buttonDelete.Enabled = true;
            }
        }
    }
}
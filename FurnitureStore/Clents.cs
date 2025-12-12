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
    public partial class Clents : Form
    {
        private DataTable clientTable;
        public Clents()
        {
            InitializeComponent();
            AutoLockManager.StartMonitoring();
        }

        private void Clents_Load(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                        SELECT 
                            CustomersID AS 'ID',
                            CustomersFIO AS 'ФИО клиента',
                            CustomersBirthday AS 'День рождения',
                            CustomersEmail AS 'Почта',
                            CustomersPhone AS 'Телефон',
                            CustomersAddress AS 'Адрес',
                            IsActive AS 'Активен'
                        FROM Customers
                            WHERE IsActive = 1;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    clientTable = new DataTable();
                    da.Fill(clientTable);
                    dataGridView1.DataSource = clientTable;

                    label3.Text = $"Всего: {clientTable.Rows.Count}";

                    if (dataGridView1.Columns.Contains("ID"))
                        dataGridView1.Columns["ID"].Visible = false;
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
            if (clientTable == null) return;

            string searchText = textBoxSearch.Text.Trim().Replace("'", "''");

            DataView view = new DataView(clientTable);
            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
                filter = $"[ФИО клиента] LIKE '%{searchText}%'";

            view.RowFilter = filter;
            dataGridView1.DataSource = view;

            label3.Text = $"Всего: {view.Count}";
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null) return;

            int clientId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            string name = dataGridView1.CurrentRow.Cells["ФИО клиента"].Value.ToString();

            bool hasOrders = CheckIfClientHasOrders(clientId);

            string message = hasOrders
                ? $"Клиент \"{name}\" имеет заказы в базе. При удалении:\n- Существующие заказы сохранят данные клиента\n- Новые заказы нельзя будет создать с этим клиентом\n\nПродолжить удаление?"
                : $"Удалить клиента \"{name}\"?";

            DialogResult result = MessageBox.Show(message, "Удаление клиента",
                MessageBoxButtons.YesNo, hasOrders ? MessageBoxIcon.Warning : MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                    {
                        con.Open();

                        MySqlCommand cmd = new MySqlCommand(
                            "UPDATE Customers SET IsActive = 0 WHERE CustomersID = @id", con);
                        cmd.Parameters.AddWithValue("@id", clientId);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show($"Клиент \"{name}\" удален!", "Успех",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            Clents_Load(null, null);
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

        private bool CheckIfClientHasOrders(int clientId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM `Order` WHERE OrderCustomers = @clientId", con);
                    cmd.Parameters.AddWithValue("@clientId", clientId);

                    int orderCount = Convert.ToInt32(cmd.ExecuteScalar());
                    return orderCount > 0;
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

            int id = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            string name = dataGridView1.CurrentRow.Cells["ФИО клиента"].Value.ToString();

            var clientData = GetClientData(id);

            ClientsInsert form = new ClientsInsert("edit")
            {
                ClientID = id,
                ClientFIO = clientData.FIO,
                ClientBirthday = clientData.Birthday,
                ClientEmail = clientData.Email,
                ClientPhone = clientData.Phone,
                ClientAddress = clientData.Address
            };

            form.ShowDialog();
            Clents_Load(null, null);
        }
        private void buttonCreate_Click(object sender, EventArgs e)
        {
            ClientsInsert form = new ClientsInsert("add");
            form.ShowDialog();
            Clents_Load(null, null);
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private (string FIO, DateTime Birthday, string Email, string Phone, string Address) GetClientData(int clientId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT CustomersFIO, CustomersBirthday, CustomersEmail, CustomersPhone, CustomersAddress FROM Customers WHERE CustomersID = @id", con);
                    cmd.Parameters.AddWithValue("@id", clientId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return (
                                reader.GetString("CustomersFIO"),
                                reader.GetDateTime("CustomersBirthday"),
                                reader.GetString("CustomersEmail"),
                                reader.GetString("CustomersPhone"),
                                reader.GetString("CustomersAddress")
                            );
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных клиента: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ("", DateTime.Now, "", "", "");
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                ClientsInsert form = new ClientsInsert("view")
                {
                    ClientFIO = row.Cells["ФИО клиента"].Value.ToString(),
                    ClientBirthday = Convert.ToDateTime(row.Cells["День рождения"].Value),
                    ClientEmail = row.Cells["Почта"].Value.ToString(),
                    ClientPhone = row.Cells["Телефон"].Value.ToString(),
                    ClientAddress = row.Cells["Адрес"].Value.ToString()
                };

                form.ShowDialog();
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            int cursorPos = textBoxSearch.SelectionStart;
            string input = textBoxSearch.Text;

            int spaceCount = input.Count(c => c == ' ');
            if (spaceCount > 2)
            {
                int lastSpace = input.LastIndexOf(' ');
                if (lastSpace >= 0)
                    input = input.Remove(lastSpace, 1);
            }

            int dashCount = input.Count(c => c == '-');
            if (dashCount > 1)
            {
                int lastDash = input.LastIndexOf('-');
                if (lastDash >= 0)
                    input = input.Remove(lastDash, 1);
            }

            char[] chars = input.ToLower().ToCharArray();
            bool makeUpper = true;

            for (int i = 0; i < chars.Length; i++)
            {
                if (makeUpper && char.IsLetter(chars[i]))
                {
                    chars[i] = char.ToUpper(chars[i]);
                    makeUpper = false;
                }
                else if (chars[i] == ' ' || chars[i] == '-')
                {
                    makeUpper = true;
                }
            }

            string formatted = new string(chars);

            textBoxSearch.TextChanged -= textBoxSearch_TextChanged;
            textBoxSearch.Text = formatted;
            textBoxSearch.SelectionStart = Math.Min(cursorPos, textBoxSearch.Text.Length);
            textBoxSearch.TextChanged += textBoxSearch_TextChanged;
            ApplyFilters();
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            string text = e.Value.ToString();

            if (columnName == "ФИО клиента")
            {
                e.Value = FormatClientName(text);
            }
            else if (columnName == "Телефон")
            {
                e.Value = FormatClientPhone(text);
            }
            else if (columnName == "Почта")
            {
                e.Value = FormatClientEmail(text);
            }
            else if (columnName == "День рождения")
            {
                if (DateTime.TryParse(text, out DateTime date))
                {
                    e.Value = FormatClientBirthday(date);
                }
            }
            else if (columnName == "Адрес")
            {
                e.Value = FormatClientAddress(text);
            }
        }

        private string FormatClientName(string fullName)
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

        private string FormatClientPhone(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return string.Empty;

            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11 && digitsOnly.StartsWith("7"))
            {
                string prefix = "+7";
                string hiddenCode = "***";
                string hiddenNumber = "***";
                string lastFourDigits = digitsOnly.Substring(digitsOnly.Length - 4);
                string formattedLast = $"{lastFourDigits.Substring(0, 2)}-{lastFourDigits.Substring(2)}";

                return $"{prefix}({hiddenCode}) {hiddenNumber}-{formattedLast}";
            }
            else if (digitsOnly.Length == 11 && digitsOnly.StartsWith("8"))
            {
                string prefix = "8";
                string hiddenCode = "***";
                string hiddenNumber = "***";
                string lastFourDigits = digitsOnly.Substring(digitsOnly.Length - 4);
                string formattedLast = $"{lastFourDigits.Substring(0, 2)}-{lastFourDigits.Substring(2)}";

                return $"{prefix}({hiddenCode}) {hiddenNumber}-{formattedLast}";
            }
            else if (digitsOnly.Length >= 6)
            {
                int visibleStartCount = Math.Min(2, digitsOnly.Length - 4);
                string visibleStart = digitsOnly.Substring(0, visibleStartCount);
                string lastFourDigits = digitsOnly.Length >= 4
                    ? digitsOnly.Substring(digitsOnly.Length - 4)
                    : digitsOnly;

                string formattedLast = lastFourDigits.Length == 4
                    ? $"{lastFourDigits.Substring(0, 2)}-{lastFourDigits.Substring(2)}"
                    : lastFourDigits;

                int hiddenCount = digitsOnly.Length - visibleStartCount - 4;
                if (hiddenCount > 0)
                {
                    string hiddenPart = new string('*', hiddenCount);
                    return $"{visibleStart}{hiddenPart}-{formattedLast}";
                }
                else
                {
                    return $"{visibleStart}-{formattedLast}";
                }
            }
            else
            {
                return phone;
            }
        }

        private string FormatClientEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return string.Empty;

            int atIndex = email.IndexOf('@');
            if (atIndex > 0)
            {
                string hiddenLocal = new string('*', 5);

                string hiddenDomain = new string('*', 5);

                return $"{hiddenLocal}@{hiddenDomain}";
            }
            else
            {
                return new string('*', 5);
            }
        }

        private string FormatClientBirthday(DateTime date)
        {
            string day = date.Day.ToString("00");
            string month = date.Month.ToString("00");

            return $"{day}.{month}.****";
        }

        private string FormatClientAddress(string address)
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

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
        }
    }
}
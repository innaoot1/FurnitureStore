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

            if (columnName == "Телефон")
            {
                string phone = new string(text.Where(char.IsDigit).ToArray());

                if (phone.Length >= 10)
                {
                    string visiblePart1 = phone.Length >= 4 ? phone.Substring(0, 4) : phone;
                    string hiddenPart = new string('*', 5);
                    string visiblePart2 = phone.Length >= 6 ? phone.Substring(phone.Length - 2, 2) : "";

                    if (phone.Length == 11 && phone.StartsWith("7"))
                    {
                        e.Value = $"+{visiblePart1[0]}({visiblePart1.Substring(1, 3)}){hiddenPart}-{visiblePart2}";
                    }
                    else
                    {
                        e.Value = $"{visiblePart1}{hiddenPart}{visiblePart2}";
                    }
                }
            }
            else if (columnName == "День рождения")
            {
                if (DateTime.TryParse(text, out DateTime date))
                {
                    e.Value = date.ToString("dd.MM.****");
                }
            }
            else if (columnName == "ФИО клиента" || columnName == "Почта" || columnName == "Адрес")
            {
                if (text.Length > 4)
                {   
                    string visiblePart = text.Substring(0, 4);
                    string hiddenPart = new string('*', 80);
                    e.Value = visiblePart + hiddenPart;
                }
            }
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";
        }
    }
}
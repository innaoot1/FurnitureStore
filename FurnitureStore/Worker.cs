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
    public partial class Worker : Form
    {
        private DataTable workerTable;
        public int CurrentUserID { get; set; }

        public Worker()
        {
            InitializeComponent();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void Worker_Load(object sender, EventArgs e)
        {
            LoadWorkers();
        }

        private void LoadWorkers()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
    SELECT 
        w.WorkerID AS 'ID',
        w.WorkerFIO AS 'Сотрудник',
        w.WorkerLogin AS 'Логин',
        w.WorkerBirthday AS 'Дата рождения',
        w.WorkerEmployment AS 'Дата найма',
        w.WorkerEmail AS 'Почта',
        w.WorkerPhone AS 'Телефон',
        r.RoleName AS 'Роль'
    FROM Worker w
    JOIN Role r ON w.WorkerRole = r.RoleID
    WHERE w.IsActive = 1;", con);  

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    workerTable = new DataTable();
                    da.Fill(workerTable);
                    dataGridView1.DataSource = workerTable;

                    label3.Text = $"Всего: {workerTable.Rows.Count}";

                    if (dataGridView1.Columns.Contains("ID"))
                        dataGridView1.Columns["ID"].Visible = false;

                    MySqlCommand cmdRoles = new MySqlCommand("SELECT RoleName FROM Role;", con);
                    MySqlDataReader reader = cmdRoles.ExecuteReader();

                    comboBoxCategory.Items.Clear();
                    comboBoxCategory.Items.Add("");
                    while (reader.Read())
                    {
                        comboBoxCategory.Items.Add(reader.GetString(0));
                    }
                    reader.Close();

                    comboBoxCategory.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите сотрудника для удаления!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedWorkerId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);

            if (selectedWorkerId == CurrentUserID)
            {
                MessageBox.Show("Вы не можете удалить самого себя!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string workerFIO = dataGridView1.CurrentRow.Cells["Сотрудник"].Value.ToString();

            bool hasOrders = CheckIfWorkerHasOrders(selectedWorkerId);

            string message = hasOrders
                ? $"Сотрудник \"{workerFIO}\" имеет заказы в базе. При удалении:\n- Существующие заказы сохранят данные сотрудника\n- Новые заказы нельзя будет создать с этим сотрудником\n\nПродолжить удаление?"
                : $"Удалить сотрудника \"{workerFIO}\"?";

            DialogResult result = MessageBox.Show(message, "Удаление сотрудника",
                MessageBoxButtons.YesNo, hasOrders ? MessageBoxIcon.Warning : MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "UPDATE Worker SET IsActive = 0 WHERE WorkerID = @id", con);
                    cmd.Parameters.AddWithValue("@id", selectedWorkerId);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show($"Сотрудник \"{workerFIO}\" удален!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadWorkers();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CheckIfWorkerHasOrders(int workerId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT COUNT(*) FROM `Order` WHERE OrderWorker = @workerId", con);
                    cmd.Parameters.AddWithValue("@workerId", workerId);

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
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите сотрудника для редактирования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;

            WorkerInsert WorkerInsert = new WorkerInsert("edit")
            {
                WorkerFIO = row.Cells["Сотрудник"].Value.ToString(),
                WorkerLogin = row.Cells["Логин"].Value.ToString(),
                WorkerPhone = row.Cells["Телефон"].Value.ToString(),
                WorkerEmail = row.Cells["Почта"].Value.ToString(),
                WorkerBirthday = Convert.ToDateTime(row.Cells["Дата рождения"].Value),
                WorkerDateEmployment = Convert.ToDateTime(row.Cells["Дата найма"].Value),
                WorkerRole = row.Cells["Роль"].Value.ToString(),
                WorkerID = Convert.ToInt32(row.Cells["ID"].Value)
            };

            WorkerInsert.ShowDialog();
            LoadWorkers();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            WorkerInsert WorkerInsert = new WorkerInsert("add");
            WorkerInsert.ShowDialog();
            LoadWorkers();
        }

        private void comboBoxCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void textBoxWorker_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (workerTable == null) return;

            string searchText = textBoxWorker.Text.Trim();
            string selectedRole = comboBoxCategory.SelectedItem?.ToString() ?? "";

            DataView view = new DataView(workerTable);
            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
                filter = $"Сотрудник LIKE '%{searchText}%'";

            if (!string.IsNullOrEmpty(selectedRole))
            {
                if (!string.IsNullOrEmpty(filter))
                    filter += " AND ";
                filter += $"Роль = '{selectedRole}'";
            }

            view.RowFilter = filter;
            dataGridView1.DataSource = view;

            label3.Text = $"Всего: {view.Count}";
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxWorker.Text = "";
            comboBoxCategory.SelectedIndex = 0;

            if (workerTable != null)
            {
                DataView view = new DataView(workerTable);
                view.RowFilter = "";
                dataGridView1.DataSource = view;
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                WorkerInsert form = new WorkerInsert("view")
                {
                    WorkerFIO = row.Cells["Сотрудник"].Value.ToString(),
                    WorkerLogin = row.Cells["Логин"].Value.ToString(),
                    WorkerPhone = row.Cells["Телефон"].Value.ToString(),
                    WorkerEmail = row.Cells["Почта"].Value.ToString(),
                    WorkerBirthday = Convert.ToDateTime(row.Cells["Дата рождения"].Value),
                    WorkerDateEmployment = Convert.ToDateTime(row.Cells["Дата найма"].Value),
                    WorkerRole = row.Cells["Роль"].Value.ToString()
                };

                form.ShowDialog();
            }
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            string text = e.Value.ToString();

            if (columnName == "Телефон")
            {
                string phone = new string(text.Where(char.IsDigit).ToArray());

                if (phone.Length == 11 && phone.StartsWith("7"))
                {
                    string visiblePart1 = phone.Substring(0, 4); 
                    string hiddenPart = new string('*', 5); 
                    string visiblePart2 = phone.Substring(9, 2); 
                    e.Value = $"+{visiblePart1[0]}({visiblePart1.Substring(1, 3)}){hiddenPart}-{visiblePart2}";
                }
            }
            else if (columnName == "Почта")
            {
                if (text.Contains("@"))
                {
                    string[] parts = text.Split('@');
                    if (parts[0].Length > 4)
                    {
                        string visiblePart = parts[0].Substring(0, 4);
                        string hiddenPart = new string('*', 80); 
                        e.Value = visiblePart + hiddenPart;
                    }
                }
                else
                {
                    if (text.Length > 4)
                    {
                        string visiblePart = text.Substring(0, 4);
                        string hiddenPart = new string('*', 80);
                        e.Value = visiblePart + hiddenPart;
                    }
                }
            }
            else if (columnName == "Дата рождения" || columnName == "Дата найма")
            {
                if (DateTime.TryParse(text, out DateTime date))
                {
                    e.Value = date.ToString("dd.MM.****");
                }
            }
            else if (columnName == "Сотрудник" || columnName == "Логин")
            {
                if (text.Length > 4)
                {
                    string visiblePart = text.Substring(0, 4);
                    string hiddenPart = new string('*', 80);
                    e.Value = visiblePart + hiddenPart;
                }
            }
        }

        private void textBoxWorker_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                buttonUpdate.Enabled = true;

                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                string role = row.Cells["Роль"].Value?.ToString() ?? "";

                buttonDelete.Enabled = (role != "Администратор");
            }
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null)
            {
                buttonUpdate.Enabled = true;

                DataGridViewRow row = dataGridView1.CurrentRow;
                string role = row.Cells["Роль"].Value?.ToString() ?? "";

                buttonDelete.Enabled = (role != "Администратор");
            }
        }
    }
}
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class WorkerInsert : Form
    {
        private string mode;
        public int WorkerID { get; set; }

        public WorkerInsert(string mode)
        {
            InitializeComponent();
            this.mode = mode;

            dateTimePickerBirthday.MaxDate = DateTime.Today.AddYears(-16);
            dateTimePickerBirthday.MinDate = DateTime.Today.AddYears(-70);
            dateTimePickerEmployment.MaxDate = DateTime.Today;

            LoadRoles();
            ApplyMode();
        }

        private void LoadRoles()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    DataTable rolesTable = new DataTable();
                    MySqlDataAdapter daRoles = new MySqlDataAdapter("SELECT RoleName FROM Role;", con);
                    daRoles.Fill(rolesTable);

                    comboBoxRole.Items.Clear();
                    foreach (DataRow row in rolesTable.Rows)
                    {
                        comboBoxRole.Items.Add(row["RoleName"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyMode()
        {
            switch (mode)
            {
                case "view":
                    label9.Visible = false;
                    label4.Visible = false;
                    textBoxPasswd.Visible = false;
                    textBoxConfPasswd.Visible = false;
                    buttonWrite.Visible = false;

                    textBoxFIO.ReadOnly = true;
                    textBoxLogin.ReadOnly = true;
                    textBoxPasswd.ReadOnly = true;
                    textBoxConfPasswd.ReadOnly = true;
                    maskedTextBoxPhone.ReadOnly = true;
                    textBoxEmail.ReadOnly = true;

                    comboBoxRole.Enabled = false;
                    dateTimePickerBirthday.Enabled = false;
                    dateTimePickerEmployment.Enabled = false;

                    break;

                case "add":
                    textBoxFIO.Text = "";
                    textBoxLogin.Text = "";
                    textBoxPasswd.Text = "";
                    textBoxConfPasswd.Text = "";
                    maskedTextBoxPhone.Text = "";
                    textBoxEmail.Text = "";
                    comboBoxRole.SelectedIndex = 0;
                    dateTimePickerEmployment.Value = DateTime.Today;

                    buttonWrite.Visible = true;
                    break;

                case "edit":
                    textBoxFIO.ReadOnly = false;
                    textBoxLogin.ReadOnly = false;
                    maskedTextBoxPhone.ReadOnly = false;
                    textBoxEmail.ReadOnly = false;

                    comboBoxRole.Enabled = true;
                    dateTimePickerBirthday.Enabled = false;

                    buttonWrite.Visible = true;
                    break;
            }
        }

        public string WorkerFIO
        {
            get => textBoxFIO.Text;
            set => textBoxFIO.Text = value;
        }

        public string WorkerLogin
        {
            get => textBoxLogin.Text;
            set => textBoxLogin.Text = value;
        }

        public string WorkerPhone
        {
            get => new string(maskedTextBoxPhone.Text.Where(char.IsDigit).ToArray());
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    maskedTextBoxPhone.Text = "";
                    return;
                }

                string digits = new string(value.Where(char.IsDigit).ToArray());

                if (digits.StartsWith("7") && maskedTextBoxPhone.Mask.StartsWith("+7"))
                {
                    digits = digits.Substring(1);
                }

                maskedTextBoxPhone.Text = digits;
            }
        }

        public string WorkerEmail
        {
            get => textBoxEmail.Text;
            set => textBoxEmail.Text = value;
        }

        public DateTime WorkerBirthday
        {
            get => dateTimePickerBirthday.Value;
            set => dateTimePickerBirthday.Value = value;
        }

        public DateTime WorkerDateEmployment
        {
            get => dateTimePickerEmployment.Value;
            set => dateTimePickerEmployment.Value = value;
        }

        public string WorkerRole
        {
            get => comboBoxRole.Text;
            set => comboBoxRole.Text = value;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы действительно хотите сохранить запись?",
                "Подтверждение записи",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            if (string.IsNullOrWhiteSpace(textBoxFIO.Text))
            {
                MessageBox.Show("Введите ФИО!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxFIO.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxLogin.Text))
            {
                MessageBox.Show("Введите логин!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxLogin.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxPasswd.Text) && mode == "add")
            {
                MessageBox.Show("Введите пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxPasswd.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxConfPasswd.Text) && mode == "add")
            {
                MessageBox.Show("Введите подтверждение пароля!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxConfPasswd.Focus();
                return;
            }
            string userDigits = new string(maskedTextBoxPhone.Text.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(maskedTextBoxPhone.Text) || userDigits.Length < 11)
            {
                MessageBox.Show("Введите полный номер телефона!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBoxPhone.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxEmail.Text))
            {
                MessageBox.Show("Введите почту!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return;
            }

            if (comboBoxRole.SelectedIndex == -1)
            {
                MessageBox.Show("Выберите роль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                comboBoxRole.Focus();
                return;
            }

            if (textBoxPasswd.Visible && textBoxConfPasswd.Visible)
            {
                string pass = textBoxPasswd.Text.Trim();
                string confPass = textBoxConfPasswd.Text.Trim();

                if (!string.IsNullOrEmpty(pass) || !string.IsNullOrEmpty(confPass))
                {
                    if (pass != confPass)
                    {
                        MessageBox.Show("Пароль и подтверждение пароля не совпадают!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
            }

            var fioParts = WorkerFIO.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (fioParts.Length < 2)
            {
                MessageBox.Show("Введите полное ФИО (минимум фамилия и имя).", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxFIO.Focus();
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    string hashedPassword = "";
                    if (!string.IsNullOrEmpty(textBoxPasswd.Text))
                    {
                        using (SHA256 sha256 = SHA256.Create())
                        {
                            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(textBoxPasswd.Text));
                            hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                        }
                    }

                    string duplicateQuery = @"SELECT COUNT(*) FROM Worker 
                                      WHERE (WorkerLogin = @Login OR WorkerPhone = @Phone)
                                      {0}";
                    string excludeId = mode == "edit" ? "AND WorkerID <> @Id" : "";
                    duplicateQuery = string.Format(duplicateQuery, excludeId);

                    using (MySqlCommand checkCmd = new MySqlCommand(duplicateQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@Login", textBoxLogin.Text);
                        checkCmd.Parameters.AddWithValue("@Phone", userDigits); 
                        if (mode == "edit")
                            checkCmd.Parameters.AddWithValue("@Id", WorkerID);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show("Пользователь с таким логином или номером телефона уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            return;
                        }
                    }

                    if (mode == "add")
                    {
                        string query = @"INSERT INTO Worker 
                    (WorkerFIO, OriginalWorkerFIO, WorkerLogin, WorkerPassword, WorkerPhone, WorkerEmail, 
                     WorkerBirthday, WorkerEmployment, WorkerRole)
                     VALUES (@FIO, @OriginalFIO, @Login, @Password, @Phone, @Email, 
                             @Birthday, @Employment, 
                             (SELECT RoleID FROM Role WHERE RoleName = @Role))";

                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@FIO", textBoxFIO.Text);
                        cmd.Parameters.AddWithValue("@OriginalFIO", textBoxFIO.Text);
                        cmd.Parameters.AddWithValue("@Login", textBoxLogin.Text);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@Phone", userDigits); 
                        cmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                        cmd.Parameters.AddWithValue("@Birthday", dateTimePickerBirthday.Value);
                        cmd.Parameters.AddWithValue("@Employment", dateTimePickerEmployment.Value);
                        cmd.Parameters.AddWithValue("@Role", comboBoxRole.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Сотрудник \"{textBoxFIO.Text}\" успешно добавлен!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (mode == "edit")
                    {
                        string getOriginalFIOQuery = "SELECT OriginalWorkerFIO FROM Worker WHERE WorkerID = @Id";
                        MySqlCommand getOriginalCmd = new MySqlCommand(getOriginalFIOQuery, con);
                        getOriginalCmd.Parameters.AddWithValue("@Id", WorkerID);
                        string originalFIO = getOriginalCmd.ExecuteScalar()?.ToString() ?? textBoxFIO.Text;

                        string query = @"UPDATE Worker 
                         SET WorkerFIO = @FIO,
                             OriginalWorkerFIO = @OriginalFIO,
                             WorkerLogin = @Login,
                             WorkerPhone = @Phone,
                             WorkerEmail = @Email,
                             WorkerBirthday = @Birthday,
                             WorkerEmployment = @Employment,
                             WorkerRole = (SELECT RoleID FROM Role WHERE RoleName = @Role)
                             {0}
                         WHERE WorkerID = @Id";

                        string passwordPart = !string.IsNullOrEmpty(hashedPassword) ? ", WorkerPassword = @Password" : "";
                        query = string.Format(query, passwordPart);

                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@FIO", textBoxFIO.Text);
                        cmd.Parameters.AddWithValue("@OriginalFIO", originalFIO);
                        cmd.Parameters.AddWithValue("@Login", textBoxLogin.Text);
                        cmd.Parameters.AddWithValue("@Phone", userDigits); 
                        cmd.Parameters.AddWithValue("@Email", textBoxEmail.Text);
                        cmd.Parameters.AddWithValue("@Birthday", dateTimePickerBirthday.Value);
                        cmd.Parameters.AddWithValue("@Employment", dateTimePickerEmployment.Value);
                        cmd.Parameters.AddWithValue("@Role", comboBoxRole.Text);
                        cmd.Parameters.AddWithValue("@Id", WorkerID);
                        if (!string.IsNullOrEmpty(hashedPassword))
                            cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        cmd.ExecuteNonQuery();

                        MessageBox.Show(
                            $"Данные сотрудника успешно обновлены!\n" +
                            $"ФИО: \"{textBoxFIO.Text}\"\n\n" +
                            "Примечание: в существующих заказах останется старое ФИО сотрудника, " +
                            "в новых заказах будет использоваться новое ФИО.",
                            "Успех",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxFIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9@._-]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxPasswd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{}|;:,.<>?]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxConfPasswd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{}|;:,.<>?]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9@._-]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxFIO_TextChanged(object sender, EventArgs e)
        {
            int cursorPos = textBoxFIO.SelectionStart;

            string input = textBoxFIO.Text;
            bool showSpaceWarning = false;
            bool showDashWarning = false;

            int spaceCount = input.Count(c => c == ' ');
            if (spaceCount > 2)
            {
                int lastSpace = input.LastIndexOf(' ');
                input = input.Remove(lastSpace, 1);
                showSpaceWarning = true;
            }

            int dashCount = input.Count(c => c == '-');
            if (dashCount > 1)
            {
                int lastDash = input.LastIndexOf('-');
                input = input.Remove(lastDash, 1);
                showDashWarning = true;
            }

            string[] parts = input
                .Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower())
                .ToArray();

            string formatted = input;
            int index = 0;
            foreach (string part in parts)
            {
                int pos = formatted.IndexOf(part, index, StringComparison.OrdinalIgnoreCase);
                if (pos >= 0)
                {
                    formatted = formatted.Remove(pos, part.Length).Insert(pos, part);
                    index = pos + part.Length;
                }
            }

            textBoxFIO.TextChanged -= textBoxFIO_TextChanged;
            textBoxFIO.Text = formatted;
            textBoxFIO.SelectionStart = Math.Min(cursorPos, textBoxFIO.Text.Length);
            textBoxFIO.TextChanged += textBoxFIO_TextChanged;

            if (showSpaceWarning)
                MessageBox.Show("Можно использовать не более двух пробелов.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            if (showDashWarning)
                MessageBox.Show("Можно использовать только одно тире.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
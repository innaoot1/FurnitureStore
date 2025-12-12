using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class Autorizathion : Form
    {
        public Autorizathion()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы действительно хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(textBoxLogin.Text) || string.IsNullOrEmpty(textBoxPasswd.Text))
                {
                    MessageBox.Show("Введите логин и пароль для входа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                string login = textBoxLogin.Text;
                string passwd = textBoxPasswd.Text;

                string hash_pass;
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
                    hash_pass = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT WorkerPassword, WorkerRole FROM Worker WHERE WorkerLogin = @login;", con);
                    cmd.Parameters.AddWithValue("@login", login);

                    DataTable dt = new DataTable();
                    new MySqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        MessageBox.Show("Пользователя с таким логином не существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxLogin.Clear();
                        textBoxPasswd.Clear();
                        return;
                    }

                    string passwordHashInDB = dt.Rows[0]["WorkerPassword"].ToString();
                    int userRole = Convert.ToInt32(dt.Rows[0]["WorkerRole"]);

                    if (hash_pass != passwordHashInDB)
                    {
                        MessageBox.Show("Введен неверный пароль!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        textBoxPasswd.Clear();
                        return;
                    }

                    int workerId = GetWorkerIdByLogin(login, con);
                    string workerFIO = GetWorkerFIOByLogin(login, con);

                    CurrentUser.UserId = workerId;
                    CurrentUser.UserLogin = login;
                    CurrentUser.UserRole = userRole;
                    CurrentUser.UserFIO = workerFIO;

                    Form nextForm = null;
                    switch (userRole)
                    {
                        case 1: nextForm = new DesktopAdministrator(); break;
                        case 2: nextForm = new DesktopManager(); break;
                        case 3: nextForm = new DesktopSeller(); break;
                    }

                    if (nextForm != null)
                    {
                        this.Visible = false;
                        nextForm.ShowDialog();
                        textBoxLogin.Clear();
                        textBoxPasswd.Clear();
                        this.Visible = true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetWorkerIdByLogin(string login, MySqlConnection con)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT WorkerID FROM Worker WHERE WorkerLogin = @login", con);
            cmd.Parameters.AddWithValue("@login", login);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private string GetWorkerFIOByLogin(string login, MySqlConnection con)
        {
            MySqlCommand cmd = new MySqlCommand("SELECT WorkerFIO FROM Worker WHERE WorkerLogin = @login", con);
            cmd.Parameters.AddWithValue("@login", login);
            return cmd.ExecuteScalar()?.ToString() ?? "";
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            textBoxPasswd.UseSystemPasswordChar = !checkBox1.Checked;
        }
        private void textBoxPasswd_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{}|;:,.<>?]$"))
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
    }
}
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class Autorizathion : Form
    {
        private int failedAttempts = 0;
        private string currentCaptcha = "";
        private Random random = new Random();
        private Timer blockTimer;
        private bool captchaShown = false;
        public Autorizathion()
        {
            InitializeComponent();

            blockTimer = new Timer();
            blockTimer.Interval = 10000;
            blockTimer.Tick += BlockTimer_Tick;

            KeyboardLayoutManager.AttachEnglishLayout(textBoxLogin, textBoxPasswd, textBoxCaptcha);
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

                if (failedAttempts >= 2)
                {
                    if (string.IsNullOrWhiteSpace(textBoxCaptcha.Text))
                    {
                        MessageBox.Show(
                            "Введите captcha!",
                            "Ошибка",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        return;
                    }

                    if (textBoxCaptcha.Text.Trim().ToUpper() != currentCaptcha)
                    {
                        MessageBox.Show(
                            "Неверно введена капча. Вход заблокирован на 10 секунд.",
                            "Ошибка авторизации",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);

                        BlockAuthorization();

                        LoadRandomCaptcha();
                        textBoxCaptcha.Clear();

                        return;
                    }
                }

                if (textBoxLogin.Text == "admin" && textBoxPasswd.Text == "admin")
                {
                    this.Visible = false;

                    ManagementDB form = new ManagementDB();
                    form.ShowDialog();

                    textBoxLogin.Clear();
                    textBoxPasswd.Clear();

                    this.Visible = true;
                    return;
                }

                if (!DatabaseChecker.QuickCheck())
                {
                    DialogResult res = MessageBox.Show(
                        "Отсутствует подключение к базе данных.\nПерейти к настройкам?",
                        "Ошибка подключения",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Error);

                    if (res == DialogResult.Yes)
                    {
                        SettingsForm settingsForm = new SettingsForm();
                        settingsForm.ShowDialog();

                        if (!DatabaseChecker.QuickCheck())
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                string login = textBoxLogin.Text;
                string passwd = textBoxPasswd.Text;

                string hash_pass;
                using (var sha256 = SHA256.Create())
                {
                    var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(passwd));
                    hash_pass = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }

                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(
                        "SELECT WorkerPassword, WorkerRole FROM Worker WHERE WorkerLogin = @login;", con);
                    cmd.Parameters.AddWithValue("@login", login);

                    DataTable dt = new DataTable();
                    new MySqlDataAdapter(cmd).Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        failedAttempts++;

                        if (failedAttempts == 1)
                        {
                            MessageBox.Show(
                                "Пользователь с указанным логином не найден.",
                                "Ошибка авторизации",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                        }
                        else if (failedAttempts == 2)
                        {
                                MessageBox.Show(
                                    "Пользователь не найден. Теперь требуется ввод captcha.",
                                    "Ошибка авторизации",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                            ShowCaptcha();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Неудачная авторизация. Вход заблокирован на 10 секунд.",
                                "Ошибка авторизации",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            BlockAuthorization();
                        }

                        return;
                    }

                    string passwordHashInDB = dt.Rows[0]["WorkerPassword"].ToString();
                    int userRole = Convert.ToInt32(dt.Rows[0]["WorkerRole"]);

                    if (hash_pass != passwordHashInDB)
                    {
                        failedAttempts++;

                        if (failedAttempts == 1)
                        {   
                                MessageBox.Show(
                                    "Неверный пароль.",
                                    "Ошибка авторизации",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);
                        }
                        else if (failedAttempts == 2)
                        {
                            MessageBox.Show(
                                "Неверный пароль. Теперь требуется ввод captcha.",
                                "Ошибка авторизации",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            ShowCaptcha();
                        }
                        else
                        {
                            MessageBox.Show(
                                "Неудачная авторизация. Вход заблокирован на 10 секунд.",
                                "Ошибка авторизации",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);

                            BlockAuthorization();
                        }

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

                        failedAttempts = 0;
                        HideCaptcha();

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

        private void Autorizathion_Load(object sender, EventArgs e)
        {
            CheckConnectionBeforeShow();
        }

        private void CheckConnectionBeforeShow()
        {
            if (!DatabaseChecker.QuickCheck())
            {
                DialogResult res = MessageBox.Show(
                    "Отсутствует подключение к базе данных.\nПерейти к настройкам?",
                    "Ошибка подключения",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Error);

                if (res == DialogResult.Yes)
                {
                    SettingsForm settingsForm = new SettingsForm();
                    settingsForm.ShowDialog();

                    if (!DatabaseChecker.QuickCheck())
                    {
                        DialogResult exitRes = MessageBox.Show(
                            "Подключение не установлено. Завершить работу приложения?",
                            "Ошибка подключения",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Error);

                        if (exitRes == DialogResult.Yes)
                        {
                            Application.Exit();
                        }
                    }
                }
                else
                {
                    Application.Exit();
                }
            }

            this.BeginInvoke(new Action(() =>
            {
                DatabaseChecker.CheckConnectionWithMessage();
            }));
        }

        private void buttonSettings_Click(object sender, EventArgs e)
        {
            SettingsForm settingsForm = new SettingsForm();

            settingsForm.ShowDialog();

            DatabaseChecker.CheckConnectionWithMessage();
        }

        private void ShowCaptcha()
        {
            if (captchaShown)
                return;

            captchaShown = true;

            labelCaptcha.Visible = true;
            textBoxCaptcha.Visible = true;
            pictureBoxCaptcha.Visible = true;

            buttonEnter.Location = new Point(
                buttonEnter.Location.X,
                buttonEnter.Location.Y + 100);

            this.Height += 100;

            LoadRandomCaptcha();

            this.CenterToScreen();
        }

        private void HideCaptcha()
        {
            if (!captchaShown)
                return;

            captchaShown = false;

            labelCaptcha.Visible = false;
            textBoxCaptcha.Visible = false;
            pictureBoxCaptcha.Visible = false;

            textBoxCaptcha.Clear();

            buttonEnter.Location = new Point(
                buttonEnter.Location.X,
                buttonEnter.Location.Y - 100);

            this.Height -= 100;

            this.CenterToScreen();
        }

        private void LoadRandomCaptcha()
        {
            string captchaFolder =
                Path.Combine(
                    Application.StartupPath,
                    "Resources",
                    "captcha");

            string[] files =
                Directory.GetFiles(captchaFolder, "*.png");

            if (files.Length == 0)
                return;

            string selectedFile =
                files[random.Next(files.Length)];

            using (var fs = new FileStream(
                selectedFile,
                FileMode.Open,
                FileAccess.Read))
            {
                pictureBoxCaptcha.Image =
                    Image.FromStream(fs);
            }

            currentCaptcha =
                Path.GetFileNameWithoutExtension(selectedFile)
                .ToUpper();
        }

        private void BlockAuthorization()
        {
            textBoxLogin.Enabled = false;
            textBoxPasswd.Enabled = false;
            textBoxCaptcha.Enabled = false;

            buttonEnter.Enabled = false;

            textBoxLogin.Clear();
            textBoxPasswd.Clear();
            textBoxCaptcha.Clear();

            blockTimer.Start();
        }

        private void BlockTimer_Tick(object sender, EventArgs e)
        {
            blockTimer.Stop();

            textBoxLogin.Enabled = true;
            textBoxPasswd.Enabled = true;
            textBoxCaptcha.Enabled = true;

            buttonEnter.Enabled = true;

            if (captchaShown)
            {
                textBoxCaptcha.Clear();
                LoadRandomCaptcha();
            }

            MessageBox.Show(
                "Вход снова доступен.",
                "Разблокировка",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }

        private void textBoxCaptcha_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9]$"))
            {
                e.Handled = true;
            }
        }
    }
}
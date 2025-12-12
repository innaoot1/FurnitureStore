using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class ClientsInsert : Form
    {
        private string mode;
        public int ClientID { get; set; }

        public ClientsInsert(string mode)
        {
            InitializeComponent();
            this.mode = mode;
            ApplyMode();

            dateTimePickerBirthday.MaxDate = DateTime.Today.AddYears(-14);
            dateTimePickerBirthday.MinDate = DateTime.Today.AddYears(-90);
        }

        private void ApplyMode()
        {
            switch (mode)
            {
                case "view":
                    textBoxFIO.ReadOnly = true;
                    dateTimePickerBirthday.Enabled = false;
                    textBoxEmail.ReadOnly = true;
                    maskedTextBoxPhone.ReadOnly = true;
                    textBoxAddress.ReadOnly = true;
                    buttonWrite.Visible = false;
                    break;
                case "edit":
                    dateTimePickerBirthday.Enabled = false;
                    break;
            }
        }

        public string ClientFIO
        {
            get => textBoxFIO.Text.Trim();
            set => textBoxFIO.Text = value;
        }

        public DateTime ClientBirthday
        {
            get => dateTimePickerBirthday.Value;
            set => dateTimePickerBirthday.Value = value;
        }

        public string ClientEmail
        {
            get => textBoxEmail.Text.Trim();
            set => textBoxEmail.Text = value;
        }

        public string ClientPhone
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

        public string ClientAddress
        {
            get => textBoxAddress.Text.Trim();
            set => textBoxAddress.Text = value;
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
                    MySqlCommand cmd;

                    string duplicateQuery;
                    if (mode == "edit")
                    {
                        duplicateQuery = "SELECT COUNT(*) FROM Customers WHERE CustomersFIO = @fio AND CustomersID <> @id AND IsActive = 1";
                    }
                    else
                    {
                        duplicateQuery = "SELECT COUNT(*) FROM Customers WHERE CustomersFIO = @fio AND IsActive = 1";
                    }

                    using (MySqlCommand checkCmd = new MySqlCommand(duplicateQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@fio", ClientFIO);
                        if (mode == "edit") checkCmd.Parameters.AddWithValue("@id", ClientID);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show(
                                $"Клиент с таким ФИО уже существует!",
                                "Ошибка дублирования",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (mode == "add")
                    {
                        cmd = new MySqlCommand(@"
                    INSERT INTO Customers 
                    (CustomersFIO, OriginalClientFIO, CustomersBirthday, CustomersEmail, CustomersPhone, CustomersAddress, IsActive)
                    VALUES (@fio, @originalFio, @birthday, @email, @phone, @address, 1)", con);
                        cmd.Parameters.AddWithValue("@originalFio", ClientFIO); 
                    }
                    else
                    {
                        string getOriginalFIOQuery = "SELECT OriginalClientFIO FROM Customers WHERE CustomersID = @id";
                        MySqlCommand getOriginalCmd = new MySqlCommand(getOriginalFIOQuery, con);
                        getOriginalCmd.Parameters.AddWithValue("@id", ClientID);
                        string originalFIO = getOriginalCmd.ExecuteScalar()?.ToString() ?? ClientFIO;

                        cmd = new MySqlCommand(@"
                    UPDATE Customers 
                    SET CustomersFIO = @fio,
                        OriginalClientFIO = @originalFio,
                        CustomersBirthday = @birthday,
                        CustomersEmail = @email,
                        CustomersPhone = @phone,
                        CustomersAddress = @address
                    WHERE CustomersID = @id", con);
                        cmd.Parameters.AddWithValue("@id", ClientID);
                        cmd.Parameters.AddWithValue("@originalFio", originalFIO);  
                    }

                    cmd.Parameters.AddWithValue("@fio", ClientFIO);
                    cmd.Parameters.AddWithValue("@birthday", ClientBirthday);
                    cmd.Parameters.AddWithValue("@email", ClientEmail);
                    cmd.Parameters.AddWithValue("@phone", ClientPhone);
                    cmd.Parameters.AddWithValue("@address", ClientAddress);

                    cmd.ExecuteNonQuery();

                    string message = mode == "add" ? "добавлен" : "обновлён";
                    MessageBox.Show(
                        $"Клиент \"{ClientFIO}\" успешно {message}!\n\n" +
                        "Примечание: в существующих заказах останутся старые данные клиента, " +
                        "в новых заказах будут использоваться новые данные.",
                        "Успех",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    this.DialogResult = DialogResult.OK;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(ClientFIO))
            {
                MessageBox.Show("Введите ФИО клиента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxFIO.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClientEmail) || !ClientEmail.Contains("@"))
            {
                MessageBox.Show("Введите корректный адрес электронной почты!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxEmail.Focus();
                return false;
            }

            string phoneDigits = new string(maskedTextBoxPhone.Text.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length < 11)
            {
                MessageBox.Show("Введите корректный номер телефона!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBoxPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ClientAddress))
            {
                MessageBox.Show("Введите адрес клиента!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAddress.Focus();
                return false;
            }

            return true;
        }

        private void textBoxFIO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxFIO_TextChanged(object sender, EventArgs e)
        {
            int cursorPos = textBoxFIO.SelectionStart;
            string input = textBoxFIO.Text;

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

            textBoxFIO.TextChanged -= textBoxFIO_TextChanged;
            textBoxFIO.Text = formatted;
            textBoxFIO.SelectionStart = Math.Min(cursorPos, textBoxFIO.Text.Length);
            textBoxFIO.TextChanged += textBoxFIO_TextChanged;
        }

        private void textBoxAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я0-9-,.\s]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxEmail_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[a-zA-Z0-9!@#$%^&*()\-_=+\[\]{}|;:,.<>?]$"))
            {
                e.Handled = true;
            }
        }
    }
}
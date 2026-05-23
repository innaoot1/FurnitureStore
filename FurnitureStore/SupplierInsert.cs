using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class SupplierInsert : Form
    {
        private string mode;
        public int SupplierID { get; set; }
        private Form parentForm;
        public SupplierInsert(string mode, Form parentForm = null)
        {
            InitializeComponent();
            this.mode = mode;
            this.parentForm = parentForm;

            AutoLockManager.StartMonitoring();

            if (parentForm != null)
            {
                BlurEffect.ShowDimmed(parentForm);
            }

            KeyboardLayoutManager.AttachRussianLayout(textBoxSupplierName, textBoxDescription, textBoxContactPerson, textBoxAddress);

            ApplyMode();
        }

        private void ApplyMode()
        {
            switch (mode)
            {
                case "view":
                    textBoxSupplierName.ReadOnly = true;
                    textBoxContactPerson.ReadOnly = true;
                    maskedTextBoxPhone.ReadOnly = true;
                    textBoxAddress.ReadOnly = true;
                    textBoxINN.ReadOnly = true;
                    textBoxDescription.ReadOnly = true;
                    buttonWrite.Visible = false;
                    break;
                case "add":
                    textBoxSupplierName.Text = "";
                    textBoxContactPerson.Text = "";
                    maskedTextBoxPhone.Text = "";
                    textBoxAddress.Text = "";
                    textBoxINN.Text = "";
                    textBoxDescription.Text = "";
                    buttonWrite.Visible = true;
                    break;
                case "edit":
                    textBoxSupplierName.ReadOnly = false;
                    textBoxContactPerson.ReadOnly = false;
                    maskedTextBoxPhone.ReadOnly = false;
                    textBoxAddress.ReadOnly = false;
                    textBoxINN.ReadOnly = false;
                    textBoxDescription.ReadOnly = false;
                    buttonWrite.Visible = true;
                    break;
            }
        }

        public string SupplierName
        {
            get => textBoxSupplierName.Text.Trim();
            set => textBoxSupplierName.Text = value;
        }

        public string ContactPerson
        {
            get => textBoxContactPerson.Text.Trim();
            set => textBoxContactPerson.Text = value;
        }

        public string SupplierPhone
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

        public string SupplierAddress
        {
            get => textBoxAddress.Text.Trim();
            set => textBoxAddress.Text = value;
        }

        public string SupplierINN
        {
            get => textBoxINN.Text.Trim();
            set => textBoxINN.Text = value;
        }

        public string SupplierDescription
        {
            get => textBoxDescription.Text.Trim();
            set => textBoxDescription.Text = value;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(SupplierName))
            {
                MessageBox.Show("Введите название поставщика!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxSupplierName.Focus();
                return false;
            }

            if (!Regex.IsMatch(SupplierName, @"^[а-яА-Я\s""]+$"))
            {
                MessageBox.Show("Название поставщика может содержать только русские буквы, пробелы и кавычки!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxSupplierName.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(ContactPerson))
            {
                MessageBox.Show("Введите контактное лицо!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxContactPerson.Focus();
                return false;
            }

            if (!Regex.IsMatch(ContactPerson, @"^[а-яА-Я\s-]+$"))
            {
                MessageBox.Show("Контактное лицо может содержать только русские буквы, пробелы и тире!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxContactPerson.Focus();
                return false;
            }

            string[] nameParts = ContactPerson.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (nameParts.Length < 2)
            {
                MessageBox.Show("Введите полное ФИО контактного лица (минимум фамилия и имя)!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxContactPerson.Focus();
                return false;
            }

            string phoneDigits = new string(maskedTextBoxPhone.Text.Where(char.IsDigit).ToArray());
            if (string.IsNullOrWhiteSpace(maskedTextBoxPhone.Text) || phoneDigits.Length < 11)
            {
                MessageBox.Show("Введите полный номер телефона (11 цифр)!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                maskedTextBoxPhone.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(SupplierAddress))
            {
                MessageBox.Show("Введите адрес поставщика!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAddress.Focus();
                return false;
            }

            if (!Regex.IsMatch(SupplierAddress, @"^[а-яА-Я0-9\s\.,-]+$"))
            {
                MessageBox.Show("Адрес может содержать только русские буквы, цифры, пробелы, точки, запятые и тире!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxAddress.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(SupplierINN))
            {
                MessageBox.Show("Введите ИНН!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxINN.Focus();
                return false;
            }

            string innDigits = new string(SupplierINN.Where(char.IsDigit).ToArray());
            if (innDigits.Length != 10)
            {
                MessageBox.Show("ИНН должен состоять из 10 цифр!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxINN.Focus();
                return false;
            }

            if (!string.IsNullOrWhiteSpace(SupplierDescription))
            {
                if (!Regex.IsMatch(SupplierDescription, @"^[а-яА-Я0-9\s\.,!?-]+$"))
                {
                    MessageBox.Show("Описание может содержать только русские буквы, цифры, пробелы, точки, запятые, восклицательные и вопросительные знаки, тире!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBoxDescription.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool IsSupplierNameUnique(int? excludeId = null)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    string query = "SELECT COUNT(*) FROM Supplier WHERE SupplierName = @name AND IsActive = 1";
                    if (excludeId.HasValue)
                        query += " AND SupplierID != @id";

                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@name", SupplierName);
                    if (excludeId.HasValue)
                        cmd.Parameters.AddWithValue("@id", excludeId.Value);

                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    return count == 0;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
                "Вы действительно хотите сохранить запись?",
                "Подтверждение записи",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes) return;

            if (!ValidateInputs()) return;

            if (!IsSupplierNameUnique(mode == "edit" ? (int?)SupplierID : null))
            {
                MessageBox.Show("Поставщик с таким названием уже существует!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBoxSupplierName.Focus();
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    string cleanPhone = SupplierPhone;
                    string cleanINN = new string(SupplierINN.Where(char.IsDigit).ToArray());

                    if (mode == "add")
                    {
                        string query = @"INSERT INTO Supplier 
                            (SupplierName, SupplierContactPerson, SupplierPhone, SupplierAddress, SupplierINN, SupplierDescription, IsActive)
                            VALUES (@name, @contact, @phone, @address, @inn, @description, 1)";

                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@name", SupplierName);
                        cmd.Parameters.AddWithValue("@contact", ContactPerson);
                        cmd.Parameters.AddWithValue("@phone", cleanPhone);
                        cmd.Parameters.AddWithValue("@address", SupplierAddress);
                        cmd.Parameters.AddWithValue("@inn", cleanINN);
                        cmd.Parameters.AddWithValue("@description", SupplierDescription);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Поставщик \"{SupplierName}\" успешно добавлен!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else if (mode == "edit")
                    {
                        string query = @"UPDATE Supplier 
                            SET SupplierName = @name,
                                SupplierContactPerson = @contact,
                                SupplierPhone = @phone,
                                SupplierAddress = @address,
                                SupplierINN = @inn,
                                SupplierDescription = @description
                            WHERE SupplierID = @id";

                        MySqlCommand cmd = new MySqlCommand(query, con);
                        cmd.Parameters.AddWithValue("@name", SupplierName);
                        cmd.Parameters.AddWithValue("@contact", ContactPerson);
                        cmd.Parameters.AddWithValue("@phone", cleanPhone);
                        cmd.Parameters.AddWithValue("@address", SupplierAddress);
                        cmd.Parameters.AddWithValue("@inn", cleanINN);
                        cmd.Parameters.AddWithValue("@description", SupplierDescription);
                        cmd.Parameters.AddWithValue("@id", SupplierID);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show($"Поставщик \"{SupplierName}\" успешно обновлен!", "Успех",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxSupplierName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s""]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxSupplierName_TextChanged(object sender, EventArgs e)
        {
            if (textBoxSupplierName.Text.StartsWith(" "))
            {
                textBoxSupplierName.TextChanged -= textBoxSupplierName_TextChanged;
                textBoxSupplierName.Text = textBoxSupplierName.Text.TrimStart();
                textBoxSupplierName.SelectionStart = textBoxSupplierName.Text.Length;
                textBoxSupplierName.TextChanged += textBoxSupplierName_TextChanged;
            }
        }

        private void textBoxContactPerson_TextChanged(object sender, EventArgs e)
        {
            if (textBoxContactPerson.Text.StartsWith(" "))
            {
                textBoxContactPerson.TextChanged -= textBoxContactPerson_TextChanged;
                textBoxContactPerson.Text = textBoxContactPerson.Text.TrimStart();
                textBoxContactPerson.SelectionStart = textBoxContactPerson.Text.Length;
                textBoxContactPerson.TextChanged += textBoxContactPerson_TextChanged;
                return;
            }

            int cursorPos = textBoxContactPerson.SelectionStart;
            string input = textBoxContactPerson.Text;

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

            textBoxContactPerson.TextChanged -= textBoxContactPerson_TextChanged;
            textBoxContactPerson.Text = formatted;
            textBoxContactPerson.SelectionStart = Math.Min(cursorPos, textBoxContactPerson.Text.Length);
            textBoxContactPerson.TextChanged += textBoxContactPerson_TextChanged;
        }

        private void textBoxContactPerson_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я\s-]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxINN_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBoxINN_TextChanged(object sender, EventArgs e)
        {
            string text = textBoxINN.Text;
            string digitsOnly = new string(text.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length > 10)
            {
                digitsOnly = digitsOnly.Substring(0, 10);
            }

            if (text != digitsOnly)
            {
                textBoxINN.TextChanged -= textBoxINN_TextChanged;
                textBoxINN.Text = digitsOnly;
                textBoxINN.SelectionStart = textBoxINN.Text.Length;
                textBoxINN.TextChanged += textBoxINN_TextChanged;
            }
        }

        private void textBoxAddress_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я0-9\s\.,-]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxAddress_TextChanged(object sender, EventArgs e)
        {
            if (textBoxAddress.Text.StartsWith(" "))
            {
                textBoxAddress.TextChanged -= textBoxAddress_TextChanged;
                textBoxAddress.Text = textBoxAddress.Text.TrimStart();
                textBoxAddress.SelectionStart = textBoxAddress.Text.Length;
                textBoxAddress.TextChanged += textBoxAddress_TextChanged;
            }
        }

        private void textBoxDescription_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я0-9\s\.,!?-]$"))
            {
                e.Handled = true;
            }
        }

        private void textBoxDescription_TextChanged(object sender, EventArgs e)
        {
            if (textBoxDescription.Text.StartsWith(" "))
            {
                textBoxDescription.TextChanged -= textBoxDescription_TextChanged;
                textBoxDescription.Text = textBoxDescription.Text.TrimStart();
                textBoxDescription.SelectionStart = textBoxDescription.Text.Length;
                textBoxDescription.TextChanged += textBoxDescription_TextChanged;
            }
        }

        private void maskedTextBoxPhone_Click(object sender, EventArgs e)
        {
            SetCursorToEnd(maskedTextBoxPhone);
        }

        private void maskedTextBoxPhone_Enter(object sender, EventArgs e)
        {
            SetCursorToEnd(maskedTextBoxPhone);
        }

        private void SetCursorToEnd(MaskedTextBox mtb)
        {
            mtb.SelectionStart = mtb.Text.Length;
            for (int i = mtb.Text.Length - 1; i >= 0; i--)
            {
                if (char.IsDigit(mtb.Text[i]))
                {
                    mtb.SelectionStart = i + 1;
                    break;
                }
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            if (parentForm != null)
            {
                BlurEffect.HideDimmed();
            }
        }
    }
}
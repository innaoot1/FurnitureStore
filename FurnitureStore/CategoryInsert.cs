using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class CategoryInsert : Form
    {
        private string mode;
        private int recordId;
        private Form parentForm;
        public CategoryInsert(string mode, int recordId = 0, string currentValue = "", Form parentForm = null)
        {
            InitializeComponent();
            this.mode = mode;
            this.recordId = recordId;
            this.parentForm = parentForm;

            if (parentForm != null)
            {
                BlurEffect.ShowDimmed(parentForm);
            }

            KeyboardLayoutManager.AttachRussianLayout(textBoxName);

            if (mode == "edit")
            {
                textBoxName.Text = currentValue;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            string value = textBoxName.Text.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("Введите название категории!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd;

                    string duplicateQuery;
                    if (mode == "edit")
                    {
                        duplicateQuery = "SELECT COUNT(*) FROM Category WHERE CategoryName = @name AND CategoryID <> @id";
                    }
                    else
                    {
                        duplicateQuery = "SELECT COUNT(*) FROM Category WHERE CategoryName = @name";
                    }

                    using (MySqlCommand checkCmd = new MySqlCommand(duplicateQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@name", value);
                        if (mode == "edit")
                        {
                            checkCmd.Parameters.AddWithValue("@id", recordId);
                        }

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show(
                                "Категория с таким названием уже существует!",
                                "Ошибка дублирования",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (mode == "edit")
                    {
                        cmd = new MySqlCommand("UPDATE Category SET CategoryName = @name WHERE CategoryID = @id", con);
                        cmd.Parameters.AddWithValue("@name", value);
                        cmd.Parameters.AddWithValue("@id", recordId);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Категория успешно обновлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        cmd = new MySqlCommand("INSERT INTO Category (CategoryName) VALUES (@name)", con);
                        cmd.Parameters.AddWithValue("@name", value);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Категория успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBoxName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
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
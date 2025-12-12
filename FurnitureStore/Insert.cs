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
    public partial class Insert : Form
    {
        private string tableName;
        private string mode;
        private int recordId;
        public Insert(string tableName, string mode, int recordId = 0, string currentValue = "")
        {
            InitializeComponent();
            this.tableName = tableName;
            this.mode = mode;
            this.recordId = recordId;

            if (mode == "edit" && tableName == "Supplier")
            {
                textBoxName.Text = currentValue;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            string value = textBoxName.Text.Trim();
            if (string.IsNullOrWhiteSpace(value))
            {
                MessageBox.Show("Введите значение!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();
                    MySqlCommand cmd;

                    string nameColumn = "";

                    switch (tableName)
                    {
                        case "Category":
                            nameColumn = "CategoryName";
                            break;
                        case "Supplier":
                            nameColumn = "SupplierName";
                            break;
                        default:
                            throw new Exception("Неизвестная таблица!");
                    }

                    string duplicateQuery;
                    if (mode == "edit" && tableName == "Supplier")
                    {
                        duplicateQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {nameColumn} = @name AND SupplierID <> @id";
                    }
                    else
                    {
                        duplicateQuery = $"SELECT COUNT(*) FROM {tableName} WHERE {nameColumn} = @name";
                    }

                    using (MySqlCommand checkCmd = new MySqlCommand(duplicateQuery, con))
                    {
                        checkCmd.Parameters.AddWithValue("@name", value);
                        if (mode == "edit" && tableName == "Supplier")
                            checkCmd.Parameters.AddWithValue("@id", recordId);

                        int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                        if (count > 0)
                        {
                            MessageBox.Show(
                                $"Запись с таким наименованием уже существует в таблице \"{tableName}\"!",
                                "Ошибка дублирования",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            return;
                        }
                    }

                    if (mode == "edit" && tableName == "Supplier")
                    {
                        cmd = new MySqlCommand($"UPDATE {tableName} SET {nameColumn}=@name WHERE SupplierID=@id", con);
                        cmd.Parameters.AddWithValue("@name", value);
                        cmd.Parameters.AddWithValue("@id", recordId);
                        cmd.ExecuteNonQuery();

                        MessageBox.Show(
                            "Изменения успешно сохранены!\n\n" +
                            "Примечание: в существующих товарах останется старое название поставщика, " +
                            "в новых товарах будет использоваться новое название.",
                            "Успех",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else
                    {
                        cmd = new MySqlCommand($"INSERT INTO {tableName} ({nameColumn}) VALUES (@name)", con);
                        cmd.Parameters.AddWithValue("@name", value);
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Запись успешно добавлена!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.DialogResult = DialogResult.OK;
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
    }
}
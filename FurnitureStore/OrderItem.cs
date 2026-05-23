using System;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace FurnitureStore
{
    public partial class OrderItem : Form
    {
        private int orderId;

        public OrderItem(int orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadOrderItems();
        }

        private void LoadOrderItems()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                SELECT 
                    COALESCE(op.OriginalProductName, p.ProductName) AS 'Товар',
                    op.ProductCount AS 'Количество',
                    COALESCE(op.OriginalPrice, p.ProductPrice) AS 'Цена за единицу',
                    (op.ProductCount * COALESCE(op.OriginalPrice, p.ProductPrice)) AS 'Общая стоимость'
                FROM OrderProduct op
                JOIN Product p ON op.ProductID = p.ProductID
                WHERE op.OrderID = @orderId;", con);

                    cmd.Parameters.AddWithValue("@orderId", orderId);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    DataTable orderItemsTable = new DataTable();
                    da.Fill(orderItemsTable);
                    dataGridView1.DataSource = orderItemsTable;

                    label3.Text = $"Всего: {orderItemsTable.Rows.Count}";

                    LoadOrderInfo(con);
                    ConfigureDataGridViewColumns();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке состава заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void ConfigureDataGridViewColumns()
        {
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.ColumnHeadersHeight = 40;

            if (dataGridView1.Columns["Товар"] != null)
                dataGridView1.Columns["Товар"].FillWeight = 52;

            if (dataGridView1.Columns["Количество"] != null)
                dataGridView1.Columns["Количество"].FillWeight = 18;

            if (dataGridView1.Columns["Цена за единицу"] != null)
                dataGridView1.Columns["Цена за единицу"].FillWeight = 18;

            if (dataGridView1.Columns["Общая стоимость"] != null)
                dataGridView1.Columns["Общая стоимость"].FillWeight = 18;


            if (dataGridView1.Columns["Количество"] != null)
                dataGridView1.Columns["Количество"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            if (dataGridView1.Columns["Цена за единицу"] != null)
                dataGridView1.Columns["Цена за единицу"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (dataGridView1.Columns["Общая стоимость"] != null)
                dataGridView1.Columns["Общая стоимость"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            if (dataGridView1.Columns["Товар"] != null)
                dataGridView1.Columns["Товар"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

        }

        private void LoadOrderInfo(MySqlConnection con)
        {
            try
            {
                MySqlCommand cmd = new MySqlCommand(@"
                    SELECT 
                        o.OrderID,
                        o.OrderDate,
                        w.WorkerFIO,
                        c.CustomersFIO,
                        o.OrderStatus,
                        o.OrderPrice
                    FROM `Order` o
                    JOIN Worker w ON o.OrderWorker = w.WorkerID
                    LEFT JOIN Customers c ON o.OrderCustomers = c.CustomersID
                    WHERE o.OrderID = @orderId;", con);

                cmd.Parameters.AddWithValue("@orderId", orderId);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        decimal orderPrice = reader.GetDecimal("OrderPrice");

                        decimal originalPrice = CalculateOriginalPrice();
                        decimal discount = originalPrice - orderPrice;
                        decimal discountPercentage = originalPrice > 0 ? discount / originalPrice : 0;

                        label1.Text = $"Сумма: {originalPrice:C}\n" +
                                            $"{(discount > 0 ? $"Скидка: {discountPercentage:P0}\n" : "")}" +
                                            $"Итого: {orderPrice:C}";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке информации о заказе: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private decimal CalculateOriginalPrice()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.Cells["Общая стоимость"].Value != null && row.Cells["Общая стоимость"].Value != DBNull.Value)
                {
                    total += Convert.ToDecimal(row.Cells["Общая стоимость"].Value);
                }
            }
            return total;
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
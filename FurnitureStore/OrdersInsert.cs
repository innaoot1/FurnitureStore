using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace FurnitureStore
{
    public partial class OrdersInsert : Form
    {
        private string mode;
        private int currentWorkerId;
        private DataTable orderItemsTable;

        public int OrderID { get; set; }
        public DateTime OrderDate
        {
            get => dateTimePicker1.Value;
            set => dateTimePicker1.Value = value;
        }
        public string WorkerName
        {
            get => comboBoxWorker.Text;
            set => comboBoxWorker.Text = value;
        }
        public string CustomerName
        {
            get => comboBoxClient.Text;
            set => comboBoxClient.Text = value;
        }
        public string OrderStatus { get; set; }
        public int OrderPrice { get; set; }
        private Form parentForm;
        public OrdersInsert(string mode, int orderId, int currentWorkerId = 0, Form parentForm = null)
        {
            InitializeComponent();
            this.mode = mode;
            this.OrderID = orderId;
            this.currentWorkerId = currentWorkerId;
            this.parentForm = parentForm;

            AutoLockManager.StartMonitoring();

            if (parentForm != null)
            {
                BlurEffect.ShowDimmed(parentForm);
            }

            InitializeOrderItemsTable();

            KeyboardLayoutManager.AttachRussianLayout(comboBoxClient, comboBoxProduct);

            LoadComboBoxData();

            LoadOrderDetails();

            ApplyMode();
        }

        private void InitializeOrderItemsTable()
        {
            orderItemsTable = new DataTable();
            orderItemsTable.Columns.Add("ProductID", typeof(int));
            orderItemsTable.Columns.Add("Товар", typeof(string));
            orderItemsTable.Columns.Add("Количество", typeof(int));
            orderItemsTable.Columns.Add("Цена за единицу", typeof(decimal));
            orderItemsTable.Columns.Add("Общая стоимость", typeof(decimal));
        }

        private void HideProductIdColumn()
        {
            if (dataGridView1.Columns.Contains("ProductID"))
            {
                dataGridView1.Columns["ProductID"].Visible = false;
            }
        }

        private void ConfigureDataGridViewColumns()
        {
            if (dataGridView1.Columns.Contains("Товар"))
            {
                dataGridView1.Columns["Товар"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["Товар"].FillWeight = 43;
            }

            if (dataGridView1.Columns.Contains("Количество"))
            {
                dataGridView1.Columns["Количество"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["Количество"].FillWeight = 19;
                dataGridView1.Columns["Количество"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dataGridView1.Columns.Contains("Цена за единицу"))
            {
                dataGridView1.Columns["Цена за единицу"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["Цена за единицу"].FillWeight = 19;
                dataGridView1.Columns["Цена за единицу"].DefaultCellStyle.Format = "C";
                dataGridView1.Columns["Цена за единицу"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            if (dataGridView1.Columns.Contains("Общая стоимость"))
            {
                dataGridView1.Columns["Общая стоимость"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView1.Columns["Общая стоимость"].FillWeight = 19;
                dataGridView1.Columns["Общая стоимость"].DefaultCellStyle.Format = "C";
                dataGridView1.Columns["Общая стоимость"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            }
        }

        private void ApplyMode()
        {
            switch (mode)
            {
                case "view":
                    buttonWrite.Visible = false;
                    buttonPlus.Visible = false;

                    comboBoxWorker.Enabled = false;
                    comboBoxClient.Enabled = false;
                    comboBoxProduct.Enabled = false;
                    comboBoxStatus.Enabled = false;
                    textBoxProductCount.Enabled = false;
                    dateTimePicker1.Enabled = false;
                    dataGridView1.ReadOnly = true;
                    break;

                case "add":
                    buttonWrite.Visible = true;
                    buttonPlus.Visible = true;
                    dateTimePicker1.Value = DateTime.Now;

                    comboBoxStatus.Enabled = false;
                    comboBoxStatus.SelectedItem = "Новый";

                    this.Text = "Оформление заказа";

                    break;

                case "edit":
                    buttonWrite.Visible = true;
                    buttonPlus.Visible = true;

                    comboBoxClient.Enabled = false;
                    comboBoxStatus.Enabled = true;

                    this.Text = "Редактирование заказа";

                    break;
            }
        }

        private void LoadComboBoxData()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();

                    string workerQuery = mode == "edit"
                        ? "SELECT WorkerID, COALESCE(OriginalWorkerFIO, WorkerFIO) as DisplayFIO FROM Worker"
                        : "SELECT WorkerID, WorkerFIO as DisplayFIO FROM Worker";

                    MySqlCommand cmdWorkers = new MySqlCommand(workerQuery, con);
                    MySqlDataAdapter daWorkers = new MySqlDataAdapter(cmdWorkers);
                    DataTable workersTable = new DataTable();
                    daWorkers.Fill(workersTable);

                    DataRow emptyWorkerRow = workersTable.NewRow();
                    emptyWorkerRow["WorkerID"] = DBNull.Value;
                    emptyWorkerRow["DisplayFIO"] = "";
                    workersTable.Rows.InsertAt(emptyWorkerRow, 0);

                    comboBoxWorker.DisplayMember = "DisplayFIO";
                    comboBoxWorker.ValueMember = "WorkerID";
                    comboBoxWorker.DataSource = workersTable;

                    if (mode == "add" && currentWorkerId > 0)
                    {
                        foreach (DataRow row in workersTable.Rows)
                        {
                            if (row["WorkerID"] != DBNull.Value && Convert.ToInt32(row["WorkerID"]) == currentWorkerId)
                            {
                                comboBoxWorker.SelectedValue = currentWorkerId;
                                break;
                            }
                        }
                    }
                    else
                    {
                        comboBoxWorker.SelectedIndex = 0;
                    }

                    string clientQuery = mode == "edit"
                        ? "SELECT CustomersID, COALESCE(OriginalClientFIO, CustomersFIO) as DisplayFIO FROM Customers WHERE IsActive = 1"
                        : "SELECT CustomersID, CustomersFIO as DisplayFIO FROM Customers WHERE IsActive = 1";

                    MySqlCommand cmdClients = new MySqlCommand(clientQuery, con);
                    MySqlDataAdapter daClients = new MySqlDataAdapter(cmdClients);
                    DataTable clientsTable = new DataTable();
                    daClients.Fill(clientsTable);

                    DataRow emptyClientRow = clientsTable.NewRow();
                    emptyClientRow["CustomersID"] = DBNull.Value;
                    emptyClientRow["DisplayFIO"] = "";
                    clientsTable.Rows.InsertAt(emptyClientRow, 0);

                    comboBoxClient.DisplayMember = "DisplayFIO";
                    comboBoxClient.ValueMember = "CustomersID";
                    comboBoxClient.DataSource = clientsTable;
                    comboBoxClient.SelectedIndex = 0;

                    MySqlCommand cmdProducts = new MySqlCommand("SELECT ProductID, ProductName, ProductPrice FROM Product WHERE IsActive = 1 ORDER BY ProductName", con);
                    MySqlDataAdapter daProducts = new MySqlDataAdapter(cmdProducts);
                    DataTable productsTable = new DataTable();
                    daProducts.Fill(productsTable);

                    comboBoxProduct.DisplayMember = "ProductName";
                    comboBoxProduct.ValueMember = "ProductID";
                    comboBoxProduct.DataSource = productsTable;
                    comboBoxProduct.SelectedIndex = -1;

                    comboBoxStatus.Items.Clear();
                    comboBoxStatus.Items.Add("Новый");
                    comboBoxStatus.Items.Add("Выполнен");
                    comboBoxStatus.Items.Add("Отменён");

                    if (mode == "add")
                    {
                        comboBoxStatus.SelectedItem = "Новый";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadOrderDetails()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                SELECT 
                    o.OrderWorker, 
                    o.OrderCustomers, 
                    o.OrderDate, 
                    o.OrderStatus, 
                    o.OrderPrice,
                    COALESCE(w.OriginalWorkerFIO, w.WorkerFIO) as WorkerFIO,
                    COALESCE(c.OriginalClientFIO, c.CustomersFIO) as CustomerFIO
                FROM `Order` o 
                JOIN Worker w ON o.OrderWorker = w.WorkerID
                LEFT JOIN Customers c ON o.OrderCustomers = c.CustomersID
                WHERE o.OrderID = @OrderID", con);
                    cmd.Parameters.AddWithValue("@OrderID", OrderID);

                    MySqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        int workerId = reader.GetInt32("OrderWorker");

                        int? clientId = reader.IsDBNull(reader.GetOrdinal("OrderCustomers")) ?
                            (int?)null : reader.GetInt32("OrderCustomers");

                        dateTimePicker1.Value = reader.GetDateTime("OrderDate");
                        OrderPrice = reader.GetInt32("OrderPrice");

                        string statusFromDB = reader.GetString("OrderStatus");
                        string workerFIO = reader.GetString("WorkerFIO");
                        string customerFIO = reader.IsDBNull(reader.GetOrdinal("CustomerFIO")) ?
                            "" : reader.GetString("CustomerFIO");

                        WorkerName = workerFIO;
                        CustomerName = customerFIO;

                        SetComboBoxValue(comboBoxWorker, workerId, workerFIO);

                        if (clientId.HasValue)
                        {
                            SetComboBoxValue(comboBoxClient, clientId.Value, customerFIO);
                        }
                        else
                        {
                            comboBoxClient.SelectedIndex = 0;
                        }

                        for (int i = 0; i < comboBoxStatus.Items.Count; i++)
                        {
                            if (comboBoxStatus.Items[i].ToString() == statusFromDB)
                            {
                                comboBoxStatus.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    reader.Close();

                    MySqlCommand cmdProducts = new MySqlCommand(@"
                SELECT 
                    p.ProductID, 
                    COALESCE(op.OriginalProductName, p.ProductName) as ProductName, 
                    op.ProductCount,
                    COALESCE(op.OriginalPrice, p.ProductPrice) as ProductPrice
                FROM OrderProduct op
                JOIN Product p ON op.ProductID = p.ProductID
                WHERE op.OrderID = @OrderID
                ORDER BY op.OrderProductID ASC", con);
                    cmdProducts.Parameters.AddWithValue("@OrderID", OrderID);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmdProducts);
                    DataTable orderProducts = new DataTable();
                    da.Fill(orderProducts);

                    orderItemsTable.Rows.Clear();
                    foreach (DataRow row in orderProducts.Rows)
                    {
                        orderItemsTable.Rows.Add(
                            row["ProductID"],
                            row["ProductName"],
                            row["ProductCount"],
                            row["ProductPrice"],
                            Convert.ToInt32(row["ProductCount"]) * Convert.ToDecimal(row["ProductPrice"])
                        );
                    }

                    dataGridView1.DataSource = orderItemsTable;

                    HideProductIdColumn();
                    ConfigureDataGridViewColumns();
                    UpdateTotalPrice();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке деталей заказа: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetComboBoxValue(ComboBox comboBox, int id, string displayValue)
        {
            try
            {
                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i] is DataRowView row)
                    {
                        if (row.Row.Table.Columns.Contains("WorkerID") &&
                            row.Row["WorkerID"] != DBNull.Value &&
                            Convert.ToInt32(row.Row["WorkerID"]) == id)
                        {
                            comboBox.SelectedIndex = i;
                            return;
                        }

                        if (row.Row.Table.Columns.Contains("CustomersID") &&
                            row.Row["CustomersID"] != DBNull.Value &&
                            Convert.ToInt32(row.Row["CustomersID"]) == id)
                        {
                            comboBox.SelectedIndex = i;
                            return;
                        }
                    }
                }

                for (int i = 0; i < comboBox.Items.Count; i++)
                {
                    if (comboBox.Items[i] is DataRowView row)
                    {
                        string displayFIO = row.Row["DisplayFIO"].ToString();
                        if (displayFIO == displayValue)
                        {
                            comboBox.SelectedIndex = i;
                            return;
                        }
                    }
                }
                comboBox.Text = displayValue;
            }
            catch (Exception)
            {
                comboBox.Text = displayValue;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonPlus_Click(object sender, EventArgs e)
        {
            if (comboBoxProduct.SelectedValue == null)
            {
                MessageBox.Show("Выберите товар!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(textBoxProductCount.Text) || !int.TryParse(textBoxProductCount.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int productId = Convert.ToInt32(comboBoxProduct.SelectedValue);
            string productName = comboBoxProduct.Text;
            decimal price = GetProductPrice(productId);
            decimal total = price * quantity;

            var existingRow = orderItemsTable.AsEnumerable()
                .FirstOrDefault(row => Convert.ToInt32(row["ProductID"]) == productId &&
                                       Convert.ToDecimal(row["Цена за единицу"]) == price);

            if (existingRow != null)
            {
                int newQuantity = Convert.ToInt32(existingRow["Количество"]) + quantity;
                existingRow["Количество"] = newQuantity;
                existingRow["Общая стоимость"] = price * newQuantity;
            }
            else
            {
                orderItemsTable.Rows.Add(productId, productName, quantity, price, total);
            }

            UpdateTotalPrice();
            HideProductIdColumn();
            ConfigureDataGridViewColumns();

            textBoxProductCount.Text = "";
            comboBoxProduct.SelectedIndex = -1;
        }

        private decimal GetProductPrice(int productId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT ProductPrice FROM Product WHERE ProductID = @ProductID", con);
                    cmd.Parameters.AddWithValue("@ProductID", productId);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
            }
            catch
            {
                return 0;
            }
        }

        private void buttonWrite_Click(object sender, EventArgs e)
        {
            if (!ValidateInput())
                return;

            if (orderItemsTable.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.GetConnectionString("db70")))
                {
                    con.Open();

                    if (mode == "add")
                    {
                        CreateNewOrder(con);
                    }
                    else if (mode == "edit")
                    {
                        UpdateExistingOrder(con);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateInput()
        {
            if (comboBoxWorker.SelectedValue == null || comboBoxWorker.SelectedValue == DBNull.Value)
            {
                MessageBox.Show("Выберите сотрудника!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (comboBoxStatus.SelectedItem == null)
            {
                MessageBox.Show("Выберите статус заказа!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (orderItemsTable.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateTotalPrice()
        {
            decimal total = 0;
            foreach (DataRow row in orderItemsTable.Rows)
            {
                total += Convert.ToDecimal(row["Общая стоимость"]);
            }

            decimal discount = CalculateDiscount(total);
            decimal finalPrice = total - discount;

            UpdateTotalPriceLabel();
        }

        private void UpdateTotalPriceLabel()
        {
            decimal total = 0;
            foreach (DataRow row in orderItemsTable.Rows)
            {
                total += Convert.ToDecimal(row["Общая стоимость"]);
            }

            decimal discount = CalculateDiscount(total);
            decimal finalPrice = total - discount;

            if (discount > 0)
            {
                label8.Text = $"Сумма без скидки: {total:C}\nСумма со скидкой: {finalPrice:C}";
            }
            else
            {
                label8.Text = $"Сумма: {total:C}";
            }
        }

        private decimal GetDiscountPercentage(decimal total)
        {
            if (total >= 20001)
                return 0.15m;
            else if (total >= 10000)
                return 0.10m;
            else
                return 0m;
        }

        private decimal CalculateDiscount(decimal total)
        {
            return total * GetDiscountPercentage(total);
        }

        private void CreateNewOrder(MySqlConnection con)
        {
            var groupedItems = orderItemsTable.AsEnumerable()
                .GroupBy(row => new {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    Price = Convert.ToDecimal(row["Цена за единицу"]),
                    ProductName = row["Товар"].ToString()
                })
                .Select(g => new {
                    g.Key.ProductID,
                    g.Key.Price,
                    g.Key.ProductName,
                    TotalQuantity = g.Sum(row => Convert.ToInt32(row["Количество"]))
                })
                .ToList();

            decimal totalPrice = 0;
            foreach (var item in groupedItems)
            {
                totalPrice += item.Price * item.TotalQuantity;
            }

            decimal discount = CalculateDiscount(totalPrice);
            decimal finalPrice = totalPrice - discount;

            string orderQuery = @"INSERT INTO `Order` 
        (OrderDate, OrderWorker, OrderCustomers, OrderStatus, OrderPrice) 
        VALUES (@OrderDate, @OrderWorker, @OrderCustomers, @OrderStatus, @OrderPrice);
        SELECT LAST_INSERT_ID();";

            MySqlCommand orderCmd = new MySqlCommand(orderQuery, con);
            orderCmd.Parameters.AddWithValue("@OrderDate", dateTimePicker1.Value);
            orderCmd.Parameters.AddWithValue("@OrderWorker", comboBoxWorker.SelectedValue);
            orderCmd.Parameters.AddWithValue("@OrderCustomers", comboBoxClient.SelectedValue);
            orderCmd.Parameters.AddWithValue("@OrderStatus", comboBoxStatus.SelectedItem.ToString());
            orderCmd.Parameters.AddWithValue("@OrderPrice", finalPrice);

            int newOrderId = Convert.ToInt32(orderCmd.ExecuteScalar());

            foreach (var item in groupedItems)
            {
                string productQuery = @"INSERT INTO OrderProduct 
            (OrderID, ProductID, ProductCount, OriginalPrice, OriginalProductName) 
            VALUES (@OrderID, @ProductID, @ProductCount, @OriginalPrice, @OriginalProductName)";

                MySqlCommand productCmd = new MySqlCommand(productQuery, con);
                productCmd.Parameters.AddWithValue("@OrderID", newOrderId);
                productCmd.Parameters.AddWithValue("@ProductID", item.ProductID);
                productCmd.Parameters.AddWithValue("@ProductCount", item.TotalQuantity);
                productCmd.Parameters.AddWithValue("@OriginalPrice", item.Price);
                productCmd.Parameters.AddWithValue("@OriginalProductName", item.ProductName);
                productCmd.ExecuteNonQuery();
            }

            string successMessage = $"Заказ №{newOrderId} успешно создан!\n\n";
            if (discount > 0)
            {
                successMessage += $"Скидка: {GetDiscountPercentage(totalPrice):P0} ({discount:C})\n";
                successMessage += $"Итоговая сумма: {finalPrice:C}";
            }
            else
            {
                successMessage += $"Итоговая сумма: {finalPrice:C}";
            }

            MessageBox.Show(successMessage, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void UpdateExistingOrder(MySqlConnection con)
        {
            Dictionary<string, int> existingItems = new Dictionary<string, int>();
            string selectQuery = "SELECT ProductID, OriginalPrice, ProductCount FROM OrderProduct WHERE OrderID = @OrderID";
            MySqlCommand selectCmd = new MySqlCommand(selectQuery, con);
            selectCmd.Parameters.AddWithValue("@OrderID", OrderID);

            using (MySqlDataReader reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string key = $"{reader["ProductID"]}_{reader["OriginalPrice"]}";
                    existingItems[key] = Convert.ToInt32(reader["ProductCount"]);
                }
            }

            var newItems = orderItemsTable.AsEnumerable()
                .GroupBy(row => new {
                    ProductID = Convert.ToInt32(row["ProductID"]),
                    Price = Convert.ToDecimal(row["Цена за единицу"]),
                    ProductName = row["Товар"].ToString()
                })
                .Select(g => new {
                    g.Key.ProductID,
                    g.Key.Price,
                    g.Key.ProductName,
                    TotalQuantity = g.Sum(row => Convert.ToInt32(row["Количество"]))
                })
                .ToDictionary(x => $"{x.ProductID}_{x.Price}", x => x);

            foreach (var item in newItems)
            {
                string key = item.Key;
                var product = item.Value;

                if (existingItems.ContainsKey(key))
                {
                    string updateQuery = @"UPDATE OrderProduct 
                SET ProductCount = @ProductCount 
                WHERE OrderID = @OrderID AND ProductID = @ProductID AND OriginalPrice = @OriginalPrice";

                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, con);
                    updateCmd.Parameters.AddWithValue("@ProductCount", product.TotalQuantity);
                    updateCmd.Parameters.AddWithValue("@OrderID", OrderID);
                    updateCmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    updateCmd.Parameters.AddWithValue("@OriginalPrice", product.Price);
                    updateCmd.ExecuteNonQuery();
                }
                else
                {
                    string insertQuery = @"INSERT INTO OrderProduct 
                (OrderID, ProductID, ProductCount, OriginalPrice, OriginalProductName) 
                VALUES (@OrderID, @ProductID, @ProductCount, @OriginalPrice, @OriginalProductName)";

                    MySqlCommand insertCmd = new MySqlCommand(insertQuery, con);
                    insertCmd.Parameters.AddWithValue("@OrderID", OrderID);
                    insertCmd.Parameters.AddWithValue("@ProductID", product.ProductID);
                    insertCmd.Parameters.AddWithValue("@ProductCount", product.TotalQuantity);
                    insertCmd.Parameters.AddWithValue("@OriginalPrice", product.Price);
                    insertCmd.Parameters.AddWithValue("@OriginalProductName", product.ProductName);
                    insertCmd.ExecuteNonQuery();
                }
            }

            foreach (var existing in existingItems)
            {
                if (!newItems.ContainsKey(existing.Key))
                {
                    string[] parts = existing.Key.Split('_');
                    int productId = Convert.ToInt32(parts[0]);
                    decimal price = Convert.ToDecimal(parts[1]);

                    string deleteQuery = @"DELETE FROM OrderProduct 
                WHERE OrderID = @OrderID AND ProductID = @ProductID AND OriginalPrice = @OriginalPrice";

                    MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, con);
                    deleteCmd.Parameters.AddWithValue("@OrderID", OrderID);
                    deleteCmd.Parameters.AddWithValue("@ProductID", productId);
                    deleteCmd.Parameters.AddWithValue("@OriginalPrice", price);
                    deleteCmd.ExecuteNonQuery();
                }
            }

            decimal totalPrice = 0;
            foreach (var item in newItems.Values)
            {
                totalPrice += item.Price * item.TotalQuantity;
            }

            decimal discount = CalculateDiscount(totalPrice);
            decimal finalPrice = totalPrice - discount;

            string orderQuery = @"UPDATE `Order` 
        SET OrderDate = @OrderDate, 
            OrderWorker = @OrderWorker, 
            OrderStatus = @OrderStatus,
            OrderPrice = @OrderPrice
        WHERE OrderID = @OrderID";

            MySqlCommand orderCmd = new MySqlCommand(orderQuery, con);
            orderCmd.Parameters.AddWithValue("@OrderDate", dateTimePicker1.Value);
            orderCmd.Parameters.AddWithValue("@OrderWorker", comboBoxWorker.SelectedValue);
            orderCmd.Parameters.AddWithValue("@OrderStatus", comboBoxStatus.SelectedItem.ToString());
            orderCmd.Parameters.AddWithValue("@OrderPrice", finalPrice);
            orderCmd.Parameters.AddWithValue("@OrderID", OrderID);
            orderCmd.ExecuteNonQuery();

            string successMessage = $"Заказ №{OrderID} успешно обновлен!\n\n";
            if (discount > 0)
            {
                successMessage += $"Скидка: {GetDiscountPercentage(totalPrice):P0} ({discount:C})\n";
                successMessage += $"Итоговая сумма: {finalPrice:C}";
            }
            else
            {
                successMessage += $"Итоговая сумма: {finalPrice:C}";
            }

            MessageBox.Show(successMessage, "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void textBoxProductCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void comboBoxClient_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(comboBoxClient.Text)) return;

            int cursorPos = comboBoxClient.SelectionStart;
            string input = comboBoxClient.Text;

            int spaceCount = input.Count(c => c == ' ');
            int dashCount = input.Count(c => c == '-');

            if (spaceCount > 2)
            {
                int spaceCounter = 0;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == ' ')
                    {
                        spaceCounter++;
                        if (spaceCounter <= 2)
                        {
                            sb.Append(input[i]);
                        }
                        else
                        {
                            if (i < cursorPos) cursorPos--;
                        }
                    }
                    else
                    {
                        sb.Append(input[i]);
                    }
                }

                input = sb.ToString();
            }

            if (dashCount > 1)
            {
                bool firstDashFound = false;
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < input.Length; i++)
                {
                    if (input[i] == '-')
                    {
                        if (!firstDashFound)
                        {
                            sb.Append(input[i]);
                            firstDashFound = true;
                        }
                        else
                        {
                            if (i < cursorPos) cursorPos--;
                        }
                    }
                    else
                    {
                        sb.Append(input[i]);
                    }
                }

                input = sb.ToString();
            }

            string[] parts = input
                .Split(new char[] { ' ', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Length > 0 ? char.ToUpper(p[0]) + p.Substring(1).ToLower() : "")
                .ToArray();

            string result = input;
            int index = 0;
            foreach (string part in parts)
            {
                if (string.IsNullOrEmpty(part)) continue;

                int pos = result.IndexOf(part, index, StringComparison.OrdinalIgnoreCase);
                if (pos >= 0)
                {
                    result = result.Remove(pos, part.Length).Insert(pos, part);
                    index = pos + part.Length;
                }
            }
            if (comboBoxClient.Text != result)
            {
                comboBoxClient.TextChanged -= comboBoxClient_TextChanged;
                comboBoxClient.Text = result;
                comboBoxClient.SelectionStart = Math.Min(cursorPos, result.Length);
                comboBoxClient.TextChanged += comboBoxClient_TextChanged;
            }
        }

        private void comboBoxClient_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void comboBoxProduct_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я-Э\s""]$"))
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
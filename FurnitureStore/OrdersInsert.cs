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
    public partial class OrdersInsert : Form
    {
        private string mode;
        private DataTable selectedProducts;
        private int currentWorkerId;

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

        public OrdersInsert(string mode, int orderId, int currentWorkerId = 0)
        {
            InitializeComponent();
            this.mode = mode;
            this.OrderID = orderId;
            this.currentWorkerId = currentWorkerId;
            selectedProducts = new DataTable();
            InitializeSelectedProductsTable();

            LoadComboBoxData();

            LoadOrderDetails();

            ApplyMode();
        }

        private void InitializeSelectedProductsTable()
        {
            selectedProducts.Columns.Add("ProductID", typeof(int));
            selectedProducts.Columns.Add("ProductName", typeof(string));
            selectedProducts.Columns.Add("Quantity", typeof(int));
            selectedProducts.Columns.Add("Price", typeof(decimal));
            selectedProducts.Columns.Add("Total", typeof(decimal));
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

                    break;

                case "add":
                    buttonWrite.Visible = true;
                    buttonPlus.Visible = true;
                    dateTimePicker1.Value = DateTime.Now;

                    comboBoxStatus.Enabled = false;
                    comboBoxStatus.SelectedItem = "Новый";
                    break;

                case "edit":
                    buttonWrite.Visible = true;
                    buttonPlus.Visible = true;

                    comboBoxClient.Enabled = false;
                    comboBoxStatus.Enabled = true;
                    break;
            }
        }
        private void LoadComboBoxData()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
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

                    MySqlCommand cmdProducts = new MySqlCommand("SELECT ProductID, ProductName, ProductPrice FROM Product WHERE IsActive = 1", con);
                    MySqlDataAdapter daProducts = new MySqlDataAdapter(cmdProducts);
                    DataTable productsTable = new DataTable();
                    daProducts.Fill(productsTable);

                    comboBoxProduct.DisplayMember = "ProductName";
                    comboBoxProduct.ValueMember = "ProductID";
                    comboBoxProduct.DataSource = productsTable;

                    comboBoxProduct.SelectedIndex = -1;

                    comboBoxStatus.Items.Clear();
                    comboBoxStatus.Items.Add("Новый");
                    comboBoxStatus.Items.Add("В обработке");
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
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
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

                    selectedProducts.Clear();

                    MySqlCommand cmdProducts = new MySqlCommand(@"
                SELECT 
                    p.ProductID, 
                    COALESCE(op.OriginalProductName, p.ProductName) as ProductName, 
                    op.ProductCount, 
                    p.ProductPrice
                FROM OrderProduct op
                JOIN Product p ON op.ProductID = p.ProductID
                WHERE op.OrderID = @OrderID", con);
                    cmdProducts.Parameters.AddWithValue("@OrderID", OrderID);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmdProducts);
                    DataTable orderProducts = new DataTable();
                    da.Fill(orderProducts);

                    foreach (DataRow row in orderProducts.Rows)
                    {
                        selectedProducts.Rows.Add(
                            row["ProductID"],
                            row["ProductName"],
                            row["ProductCount"],
                            row["ProductPrice"],
                            Convert.ToInt32(row["ProductCount"]) * Convert.ToDecimal(row["ProductPrice"])
                        );
                    }
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
            this.DialogResult = DialogResult.OK;
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

            var existingRow = selectedProducts.AsEnumerable()
                .FirstOrDefault(row => Convert.ToInt32(row["ProductID"]) == productId);

            if (existingRow != null)
            {
                existingRow["Quantity"] = Convert.ToInt32(existingRow["Quantity"]) + quantity;
                existingRow["Total"] = Convert.ToInt32(existingRow["Quantity"]) * price;
            }
            else
            {
                selectedProducts.Rows.Add(productId, productName, quantity, price, quantity * price);
            }

            UpdateTotalPrice();

            textBoxProductCount.Text = "";
            comboBoxProduct.SelectedIndex = -1;
        }

        private decimal GetProductPrice(int productId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
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

            if (selectedProducts.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
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

            if (selectedProducts.Rows.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы один товар в заказ!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void UpdateTotalPrice()
        {
            decimal total = 0;
            foreach (DataRow row in selectedProducts.Rows)
            {
                total += Convert.ToDecimal(row["Total"]);
            }

            decimal discount = CalculateDiscount(total);
            decimal finalPrice = total - discount;

            if (selectedProducts.Rows.Count > 0)
            {
                string discountMessage = $"Общая сумма заказа: {total:C}\n";

                if (discount > 0)
                {
                    discountMessage += $"Скидка: {GetDiscountPercentage(total):P0} ({discount:C})\n";
                    discountMessage += $"Итоговая сумма: {finalPrice:C}";
                }
                else
                {
                    discountMessage += $"Итоговая сумма: {finalPrice:C}";
                }

                MessageBox.Show(discountMessage, "Информация о заказе", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            decimal totalPrice = 0;
            foreach (DataRow row in selectedProducts.Rows)
            {
                totalPrice += Convert.ToDecimal(row["Total"]);
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

            foreach (DataRow row in selectedProducts.Rows)
            {
                string getProductNameQuery = "SELECT ProductName FROM Product WHERE ProductID = @productId";
                MySqlCommand getNameCmd = new MySqlCommand(getProductNameQuery, con);
                getNameCmd.Parameters.AddWithValue("@productId", row["ProductID"]);
                string currentProductName = getNameCmd.ExecuteScalar()?.ToString() ?? row["ProductName"].ToString();

                string productQuery = @"INSERT INTO OrderProduct 
            (OrderID, ProductID, ProductCount, OriginalProductName) 
            VALUES (@OrderID, @ProductID, @ProductCount, @OriginalProductName)";

                MySqlCommand productCmd = new MySqlCommand(productQuery, con);
                productCmd.Parameters.AddWithValue("@OrderID", newOrderId);
                productCmd.Parameters.AddWithValue("@ProductID", row["ProductID"]);
                productCmd.Parameters.AddWithValue("@ProductCount", row["Quantity"]);
                productCmd.Parameters.AddWithValue("@OriginalProductName", currentProductName);
                productCmd.ExecuteNonQuery();
            }

            string successMessage = $"Заказ №{newOrderId} успешно создан!\n";
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
            this.DialogResult = DialogResult.OK;
        }

        private void UpdateExistingOrder(MySqlConnection con)
        {
            decimal totalPrice = 0;
            foreach (DataRow row in selectedProducts.Rows)
            {
                totalPrice += Convert.ToDecimal(row["Total"]);
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

            Dictionary<int, string> existingOriginalNames = new Dictionary<int, string>();

            string selectOriginalQuery = "SELECT ProductID, OriginalProductName FROM OrderProduct WHERE OrderID = @OrderID";
            MySqlCommand selectCmd = new MySqlCommand(selectOriginalQuery, con);
            selectCmd.Parameters.AddWithValue("@OrderID", OrderID);

            using (MySqlDataReader reader = selectCmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    int productId = reader.GetInt32("ProductID");
                    string originalName = reader.IsDBNull(reader.GetOrdinal("OriginalProductName"))
                        ? null
                        : reader.GetString("OriginalProductName");
                    existingOriginalNames[productId] = originalName;
                }
            }

            string deleteQuery = "DELETE FROM OrderProduct WHERE OrderID = @OrderID";
            MySqlCommand deleteCmd = new MySqlCommand(deleteQuery, con);
            deleteCmd.Parameters.AddWithValue("@OrderID", OrderID);
            deleteCmd.ExecuteNonQuery();

            foreach (DataRow row in selectedProducts.Rows)
            {
                int productId = Convert.ToInt32(row["ProductID"]);
                string originalProductName;

                if (existingOriginalNames.ContainsKey(productId) && existingOriginalNames[productId] != null)
                {
                    originalProductName = existingOriginalNames[productId];
                }
                else
                {
                    string getProductNameQuery = "SELECT ProductName FROM Product WHERE ProductID = @productId";
                    MySqlCommand getNameCmd = new MySqlCommand(getProductNameQuery, con);
                    getNameCmd.Parameters.AddWithValue("@productId", productId);
                    originalProductName = getNameCmd.ExecuteScalar()?.ToString() ?? row["ProductName"].ToString();
                }

                string productQuery = @"INSERT INTO OrderProduct 
            (OrderID, ProductID, ProductCount, OriginalProductName) 
            VALUES (@OrderID, @ProductID, @ProductCount, @OriginalProductName)";

                MySqlCommand productCmd = new MySqlCommand(productQuery, con);
                productCmd.Parameters.AddWithValue("@OrderID", OrderID);
                productCmd.Parameters.AddWithValue("@ProductID", productId);
                productCmd.Parameters.AddWithValue("@ProductCount", row["Quantity"]);
                productCmd.Parameters.AddWithValue("@OriginalProductName", originalProductName);
                productCmd.ExecuteNonQuery();
            }

            string successMessage = $"Заказ №{OrderID} успешно обновлен!\n";
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
            this.DialogResult = DialogResult.OK;
        }

        private void textBoxProductCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
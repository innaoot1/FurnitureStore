using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Word = Microsoft.Office.Interop.Word;

namespace FurnitureStore
{
    public partial class Orders : Form
    {
        private int roleId;
        private DataTable orderTable;
        private DataView currentView;
        private PaginationManager pagination;

        public Orders(int role)
        {
            InitializeComponent();
            pagination = new PaginationManager(
                dataGridView1,
                paginationPanel,
                labelPageInfo,
                label2);

            roleId = role;
            ConfigureButtons();
            AutoLockManager.StartMonitoring();

            KeyboardLayoutManager.AttachRussianLayout(textBoxSearch);
        }

        private void ConfigureButtons()
        {
            if (roleId == 3)
            {
                buttonCreate.Visible = true;
                buttonUpdate.Visible = true;
                buttonCheck.Visible = true;
                button1.Visible = true;
            }
            else if (roleId == 2)
            {
                buttonRevenue.Visible = true;
                buttonOrderItem.Visible = true;
            }
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmd = new MySqlCommand(@"
                SELECT 
                    o.OrderID AS 'ID',
                    o.OrderID AS 'Номер заказа',
                    o.OrderDate AS 'Дата заказа',
                    COALESCE(w.OriginalWorkerFIO, w.WorkerFIO) AS 'Сотрудник',
                    COALESCE(c.OriginalClientFIO, c.CustomersFIO) AS 'Клиент',
                    o.OrderStatus AS 'Статус заказа',
                    o.OrderPrice AS 'Сумма заказа'
                FROM `Order` o
                JOIN Worker w ON o.OrderWorker = w.WorkerID
                LEFT JOIN Customers c ON o.OrderCustomers = c.CustomersID
                ORDER BY o.OrderID;", con);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    orderTable = new DataTable();
                    da.Fill(orderTable);
                    currentView = new DataView(orderTable);
                    currentView.Sort = "ID ASC";

                    pagination.SetData(currentView);

                    if (dataGridView1.Columns.Contains("ID"))
                        dataGridView1.Columns["ID"].Visible = false;

                    MySqlCommand cmdCategories = new MySqlCommand("SELECT CategoryName FROM Category;", con);
                    MySqlDataReader reader = cmdCategories.ExecuteReader();

                    comboBoxFilter.Items.Clear();
                    comboBoxFilter.Items.Add("");
                    comboBoxFilter.Items.Add("Новый");
                    comboBoxFilter.Items.Add("Выполнен");
                    comboBoxFilter.Items.Add("Отменён");
                    comboBoxFilter.SelectedIndex = 0;

                    comboBoxSort.Items.Clear();
                    comboBoxSort.Items.Add("");
                    comboBoxSort.Items.Add("По возрастанию");
                    comboBoxSort.Items.Add("По убыванию");
                    comboBoxSort.SelectedIndex = 0;

                    UpdateButtonsState();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString(), "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateButtonsState()
        {
            if (dataGridView1.CurrentRow == null)
            {
                buttonUpdate.Enabled = false;
                buttonCheck.Enabled = false;
                buttonOrderItem.Enabled = false;
                return;
            }

            string status = dataGridView1.CurrentRow.Cells["Статус заказа"].Value.ToString();

            if (status == "Выполнен" || status == "Отменён")
            {
                buttonUpdate.Enabled = false;
            }
            else
            {
                buttonUpdate.Enabled = true;
            }

            if (status == "Отменён")
            {
                buttonCheck.Enabled = false;
            }
            else
            {
                buttonCheck.Enabled = true;
            }

            buttonOrderItem.Enabled = true;
        }

        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            if (orderTable == null)
                return;

            string searchText =
                textBoxSearch.Text.Trim().Replace("'", "''");

            string selectedStatus =
                comboBoxFilter.SelectedItem?.ToString() ?? "";

            string sortOption =
                comboBoxSort.SelectedItem?.ToString() ?? "";

            currentView = new DataView(orderTable);

            string filter = "";

            if (!string.IsNullOrEmpty(searchText))
            {
                filter =
                    $@"Convert([Номер заказа], 'System.String')
            LIKE '%{searchText}%'
            OR [Сотрудник] LIKE '%{searchText}%'
            OR [Клиент] LIKE '%{searchText}%'";
            }

            if (!string.IsNullOrEmpty(selectedStatus))
            {
                string statusFilter =
                    $"[Статус заказа] = '{selectedStatus}'";

                if (!string.IsNullOrEmpty(filter))
                    filter = $"({filter}) AND ({statusFilter})";
                else
                    filter = statusFilter;
            }

            currentView.RowFilter = filter;

            if (sortOption == "По возрастанию")
                currentView.Sort = "[Сумма заказа] ASC";
            else if (sortOption == "По убыванию")
                currentView.Sort = "[Сумма заказа] DESC";
            else
                currentView.Sort = "ID ASC";

            pagination.SetData(currentView);

            UpdateButtonsState();
        }

        private void buttonClearFilters_Click(object sender, EventArgs e)
        {
            textBoxSearch.Text = "";

            comboBoxFilter.SelectedIndex = 0;
            comboBoxSort.SelectedIndex = 0;

            currentView = new DataView(orderTable);

            pagination.SetData(currentView);

            UpdateButtonsState();
        }

        private void buttonCreate_Click(object sender, EventArgs e)
        {
            int currentSellerId = GetCurrentSellerId();
            OrdersInsert OrdersInsert = new OrdersInsert("add", 0, currentSellerId, this);
            this.Visible = false;
            OrdersInsert.ShowDialog();
            this.Visible = true;
            LoadOrders();
        }

        private int GetCurrentSellerId()
        {
            return CurrentUser.UserId;
        }

        private void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите заказ для редактирования!", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataGridViewRow row = dataGridView1.CurrentRow;
            string orderStatus = row.Cells["Статус заказа"].Value.ToString();

            if (orderStatus == "Выполнен")
            {
                MessageBox.Show("Заказ выполнен и не может быть изменен", "Информация",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int orderId = Convert.ToInt32(row.Cells["ID"].Value);

            OrdersInsert OrdersInsert = new OrdersInsert("edit", orderId, 0, this)
            {
                OrderDate = Convert.ToDateTime(row.Cells["Дата заказа"].Value),
                OrderPrice = Convert.ToInt32(row.Cells["Сумма заказа"].Value)
            };
            this.Visible = false;
            OrdersInsert.ShowDialog();
            this.Visible = true;
            LoadOrders();
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonRevenue_Click(object sender, EventArgs e)
        {
            Revenue Revenue = new Revenue(this);
            Revenue.ShowDialog();
        }

        private void buttonCheck_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите заказ для печати чека!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int orderId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);
            GenerateWordReceipt(orderId);
        }

        private void GenerateWordReceipt(int orderId)
        {
            Word.Application wordApp = null;
            Word.Document document = null;

            try
            {
                var orderData = GetOrderData(orderId);
                if (orderData == null)
                {
                    MessageBox.Show("Не удалось получить данные о заказе", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                wordApp = new Word.Application();
                wordApp.Visible = false;

                document = wordApp.Documents.Add();

                document.PageSetup.Orientation = Word.WdOrientation.wdOrientPortrait;
                document.PageSetup.PageWidth = wordApp.CentimetersToPoints(10f);
                document.PageSetup.PageHeight = wordApp.CentimetersToPoints(29.7f);
                document.PageSetup.TopMargin = wordApp.CentimetersToPoints(0.5f);
                document.PageSetup.BottomMargin = wordApp.CentimetersToPoints(0.5f);
                document.PageSetup.LeftMargin = wordApp.CentimetersToPoints(0.5f);
                document.PageSetup.RightMargin = wordApp.CentimetersToPoints(0.5f);

                Word.Paragraph content = document.Content.Paragraphs.Add();
                content.Format.SpaceAfter = 1f;
                content.Format.SpaceBefore = 0f;
                content.Format.LineSpacingRule = Word.WdLineSpacing.wdLineSpaceSingle;

                content.Range.Text = "МАГАЗИН ОФИСНОЙ МЕБЕЛИ";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 12;
                content.Range.Font.Bold = 1;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = "КАССОВЫЙ ЧЕК";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 10;
                content.Range.Font.Bold = 1;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = "----------------------------------------";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = $"Заказ №: {orderData.OrderNumber}";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                content.Range.InsertParagraphAfter();

                content.Range.Text = $"Дата: {orderData.OrderDate:dd.MM.yy HH:mm}";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                content.Range.InsertParagraphAfter();

                content.Range.Text = $"Продавец: {orderData.Employee}";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                content.Range.InsertParagraphAfter();

                if (!string.IsNullOrEmpty(orderData.Customer))
                {
                    content.Range.Text = $"Клиент: {orderData.Customer}";
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 9;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    content.Range.InsertParagraphAfter();
                }

                if (!string.IsNullOrEmpty(orderData.Phone))
                {
                    string formattedPhone = FormatPhoneNumber(orderData.Phone);
                    content.Range.Text = $"Телефон: {formattedPhone}";
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 9;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    content.Range.InsertParagraphAfter();
                }

                content.Range.Text = "----------------------------------------";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = "ТОВАРЫ";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 10;
                content.Range.Font.Bold = 1;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                decimal totalAmount = 0;

                foreach (var product in orderData.Products)
                {
                    string productName = product.Name;
                    content.Range.Text = productName;
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 9;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    content.Range.InsertParagraphAfter();

                    string priceLine = $"{product.Quantity} x {product.Price:C} = {product.Total:C}";
                    content.Range.Text = priceLine;
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 9;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
                    content.Range.InsertParagraphAfter();

                    totalAmount += product.Total;
                }

                decimal discount = 0;
                if (totalAmount >= 20001)
                {
                    discount = totalAmount * 0.15m;
                }
                else if (totalAmount >= 10000)
                {
                    discount = totalAmount * 0.10m;
                }

                decimal finalPrice = totalAmount - discount;

                content.Range.Text = "========================================";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = $"ИТОГО: {finalPrice:C}";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 10;
                content.Range.Font.Bold = 1;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                if (discount > 0)
                {
                    content.Range.Text = $"Скидка: {discount:C}";
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 9;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    content.Range.InsertParagraphAfter();

                    string discountInfo = "";
                    if (totalAmount >= 20001)
                        discountInfo = "Скидка 15% за сумму от 20 000₽";
                    else if (totalAmount >= 10000)
                        discountInfo = "Скидка 10% за сумму от 10 000₽";

                    content.Range.Text = discountInfo;
                    content.Range.Font.Name = "Arial";
                    content.Range.Font.Size = 8;
                    content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    content.Range.InsertParagraphAfter();
                }

                content.Range.Text = "========================================";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = "СПАСИБО ЗА ПОКУПКУ!";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 10;
                content.Range.Font.Bold = 1;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                content.Range.InsertParagraphAfter();

                content.Range.Text = "ЖДЕМ ВАС СНОВА!";
                content.Range.Font.Name = "Arial";
                content.Range.Font.Size = 9;
                content.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                wordApp.Visible = true;
                wordApp.Activate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании чека: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                if (document != null)
                {
                    document.Close(false);
                    document = null;
                }
                if (wordApp != null)
                {
                    wordApp.Quit();
                    wordApp = null;
                }
            }
            finally
            {
                try
                {
                    if (document != null)
                    {
                        if (!wordApp.Visible)
                        {
                            document.Close(false);
                        }
                        ReleaseObject(document);
                        document = null;
                    }

                    if (wordApp != null)
                    {
                        if (wordApp.Documents.Count == 0)
                        {
                            wordApp.Quit();
                        }
                        ReleaseObject(wordApp);
                        wordApp = null;
                    }
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                }
                catch (Exception)
                {

                }
            }
        }

        private void ReleaseObject(object obj)
        {
            try
            {
                if (obj != null)
                {
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(obj);
                    obj = null;
                }
            }
            catch (Exception)
            {
                obj = null;
            }
            finally
            {
                obj = null;
            }
        }

        private OrderData GetOrderData(int orderId)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(connStr.ConnectionString))
                {
                    con.Open();

                    MySqlCommand cmdOrder = new MySqlCommand(@"
                SELECT 
                    o.OrderID,
                    o.OrderDate,
                    o.OrderStatus,
                    o.OrderPrice,
                    COALESCE(w.OriginalWorkerFIO, w.WorkerFIO) as WorkerFIO,
                    COALESCE(c.OriginalClientFIO, c.CustomersFIO) as CustomersFIO,
                    c.CustomersPhone
                FROM `Order` o
                JOIN Worker w ON o.OrderWorker = w.WorkerID
                LEFT JOIN Customers c ON o.OrderCustomers = c.CustomersID
                WHERE o.OrderID = @orderId", con);
                    cmdOrder.Parameters.AddWithValue("@orderId", orderId);

                    MySqlDataReader readerOrder = cmdOrder.ExecuteReader();
                    if (!readerOrder.Read())
                    {
                        readerOrder.Close();
                        return null;
                    }

                    var orderData = new OrderData
                    {
                        OrderNumber = readerOrder.GetInt32("OrderID"),
                        OrderDate = readerOrder.GetDateTime("OrderDate"),
                        Status = readerOrder.GetString("OrderStatus"),
                        TotalAmount = readerOrder.GetDecimal("OrderPrice"),
                        Employee = readerOrder.GetString("WorkerFIO"),
                        Customer = readerOrder.IsDBNull(readerOrder.GetOrdinal("CustomersFIO")) ?
                            null : readerOrder.GetString("CustomersFIO"), 
                        Phone = readerOrder.IsDBNull(readerOrder.GetOrdinal("CustomersPhone")) ?
                            null : readerOrder.GetString("CustomersPhone")
                    };

                    readerOrder.Close();

                    MySqlCommand cmdProducts = new MySqlCommand(@"
                SELECT 
                    COALESCE(op.OriginalProductName, p.ProductName) as ProductName,
                    p.ProductPrice,
                    op.ProductCount,
                    (p.ProductPrice * op.ProductCount) as TotalPrice
                FROM OrderProduct op
                JOIN Product p ON op.ProductID = p.ProductID
                WHERE op.OrderID = @orderId", con);
                    cmdProducts.Parameters.AddWithValue("@orderId", orderId);

                    MySqlDataReader readerProducts = cmdProducts.ExecuteReader();
                    orderData.Products = new List<ProductItem>();

                    while (readerProducts.Read())
                    {
                        orderData.Products.Add(new ProductItem
                        {
                            Name = readerProducts.GetString("ProductName"),
                            Price = readerProducts.GetDecimal("ProductPrice"),
                            Quantity = readerProducts.GetInt32("ProductCount"),
                            Total = readerProducts.GetDecimal("TotalPrice")
                        });
                    }
                    readerProducts.Close();

                    return orderData;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных заказа: {ex.Message}", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        private string FormatPhoneNumber(string phone)
        {
            if (string.IsNullOrEmpty(phone))
                return "Не указан";

            string digitsOnly = new string(phone.Where(char.IsDigit).ToArray());

            if (digitsOnly.Length == 11)
            {
                return $"+{digitsOnly[0]} ({digitsOnly.Substring(1, 3)}) {digitsOnly.Substring(4, 3)}-{digitsOnly.Substring(7, 2)}-{digitsOnly.Substring(9, 2)}";
            }
            else if (digitsOnly.Length == 10)
            {
                return $"+7 ({digitsOnly.Substring(0, 3)}) {digitsOnly.Substring(3, 3)}-{digitsOnly.Substring(6, 2)}-{digitsOnly.Substring(8, 2)}";
            }
            else
            {
                return phone;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                UpdateButtonsState();
            }
        }

        private void buttonOrderItem_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
            {
                MessageBox.Show("Выберите заказ для просмотра состава!", "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int selectedOrderId = Convert.ToInt32(dataGridView1.CurrentRow.Cells["ID"].Value);

            OrderItem orderItemForm = new OrderItem(selectedOrderId);
            this.Visible = false;
            orderItemForm.ShowDialog();
            this.Visible = true;
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null) return;

            string columnName = dataGridView1.Columns[e.ColumnIndex].HeaderText;
            string text = e.Value.ToString();

            if (columnName == "Клиент" || columnName == "Сотрудник")
            {
                if (!string.IsNullOrEmpty(text))
                {
                    e.Value = FormatNameForDisplay(text);
                }
            }
        }

        private string FormatNameForDisplay(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
                return string.Empty;

            string[] nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (nameParts.Length >= 3)
            {
                return $"{nameParts[0]} {nameParts[1][0]}.{nameParts[2][0]}.";
            }
            else if (nameParts.Length == 2)
            {
                return $"{nameParts[0]} {nameParts[1][0]}.";
            }
            else
            {
                return fullName;
            }
        }

        private void textBoxSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) &&
                !System.Text.RegularExpressions.Regex.IsMatch(e.KeyChar.ToString(), @"^[а-яА-Я0-9-\s]$"))
            {
                e.Handled = true;
            }
        }

        private void dataGridView1_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var row = dataGridView1.Rows[e.RowIndex];

            if (row.Cells["Статус заказа"].Value == null)
                return;

            string status = row.Cells["Статус заказа"].Value.ToString();

            switch (status)
            {
                case "Новый":
                    row.DefaultCellStyle.BackColor = Color.LightGray;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;

                case "Выполнен":
                    row.DefaultCellStyle.BackColor = Color.LightGreen;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;

                case "Отменён":
                    row.DefaultCellStyle.BackColor = Color.LightCoral;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;

                default:
                    row.DefaultCellStyle.BackColor = Color.White;
                    row.DefaultCellStyle.ForeColor = Color.Black;
                    break;
            }
        }
    }

    public class OrderData
    {
        public int OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Employee { get; set; }
        public string Customer { get; set; }
        public string Phone { get; set; }
        public List<ProductItem> Products { get; set; }
    }

    public class ProductItem
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
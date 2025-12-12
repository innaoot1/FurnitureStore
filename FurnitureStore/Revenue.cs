using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Excel = Microsoft.Office.Interop.Excel;

namespace FurnitureStore
{
    public partial class Revenue : Form
    {
        public Revenue()
        {
            InitializeComponent();
            LoadDateRangeFromDatabase();
        }

        private void LoadDateRangeFromDatabase()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr.ConnectionString))
                {
                    connection.Open();

                    string minDateQuery = "SELECT MIN(DATE(OrderDate)) FROM `Order`";
                    using (MySqlCommand minCmd = new MySqlCommand(minDateQuery, connection))
                    {
                        var minDateResult = minCmd.ExecuteScalar();
                        if (minDateResult != null && minDateResult != DBNull.Value)
                        {
                            DateTime minDate = Convert.ToDateTime(minDateResult);
                            dateTimePicker1.MinDate = minDate;
                            dateTimePicker2.MinDate = minDate;
                        }
                        else
                        {
                            DateTime today = DateTime.Today;
                            dateTimePicker1.MinDate = today;
                            dateTimePicker2.MinDate = today;
                        }
                    }

                    string maxDateQuery = "SELECT MAX(DATE(OrderDate)) FROM `Order`";
                    using (MySqlCommand maxCmd = new MySqlCommand(maxDateQuery, connection))
                    {
                        var maxDateResult = maxCmd.ExecuteScalar();
                        if (maxDateResult != null && maxDateResult != DBNull.Value)
                        {
                            DateTime maxDate = Convert.ToDateTime(maxDateResult);
                            dateTimePicker1.MaxDate = maxDate;
                            dateTimePicker2.MaxDate = maxDate;
                        }
                        else
                        {
                            DateTime today = DateTime.Today;
                            dateTimePicker1.MaxDate = today;
                            dateTimePicker2.MaxDate = today;
                        }
                    }

                    dateTimePicker1.Value = dateTimePicker1.MinDate;
                    dateTimePicker2.Value = dateTimePicker2.MaxDate;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                DateTime today = DateTime.Today;
                dateTimePicker1.MinDate = today.AddYears(-1);
                dateTimePicker1.MaxDate = today;
                dateTimePicker2.MinDate = today.AddYears(-1);
                dateTimePicker2.MaxDate = today;
                dateTimePicker1.Value = today.AddMonths(-1);
                dateTimePicker2.Value = today;
            }
        }

        private void buttonBack_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void buttonGenerate_Click(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                MessageBox.Show("Дата 'С' не может быть больше даты 'По'", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult result = MessageBox.Show("Вы действительно хотите создать отчёт в Excel?", "Создание отчёта", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                GenerateExcelReport();
            }
        }

        private void GenerateExcelReport()
        {
            Excel.Application excelApp = null;
            Excel.Workbook workbook = null;
            Excel.Worksheet worksheet = null;

            try
            {
                DataTable dataTable = GetRevenueData();

                if (dataTable.Rows.Count == 0)
                {
                    MessageBox.Show("Нет выполненных заказов за выбранный период", "Информация", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                excelApp = new Excel.Application();
                excelApp.Visible = false;
                excelApp.DisplayAlerts = false;

                workbook = excelApp.Workbooks.Add();
                worksheet = workbook.ActiveSheet as Excel.Worksheet;
                worksheet.Name = "Отчёт по выручке";

                worksheet.Cells[1, 1] = "Магазин офисной мебели";
                SetRangeStyle(worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 6]], 16, true, Excel.XlHAlign.xlHAlignCenter);
                worksheet.Range[worksheet.Cells[1, 1], worksheet.Cells[1, 6]].Merge();

                worksheet.Cells[2, 1] = "Отчёт по выручке";
                SetRangeStyle(worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[2, 6]], 14, true, Excel.XlHAlign.xlHAlignCenter);
                worksheet.Range[worksheet.Cells[2, 1], worksheet.Cells[2, 6]].Merge();

                worksheet.Cells[3, 1] = $"Период: с {dateTimePicker1.Value:dd.MM.yyyy} по {dateTimePicker2.Value:dd.MM.yyyy}";
                SetRangeStyle(worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 6]], 12, false, Excel.XlHAlign.xlHAlignCenter);
                worksheet.Range[worksheet.Cells[3, 1], worksheet.Cells[3, 6]].Merge();

                worksheet.Cells[4, 1] = "";

                int currentRow = 5;
                string[] headers = { "Номер заказа", "Дата заказа", "Сотрудник", "Клиент", "Сумма заказа (руб.)", "Статус заказа" };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[currentRow, i + 1] = headers[i];
                    var cell = worksheet.Cells[currentRow, i + 1];
                    cell.Font.Bold = true;
                    cell.Interior.Color = Excel.XlRgbColor.rgbLightGray;
                    cell.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    cell.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                }

                currentRow++;
                decimal totalRevenue = 0;

                dataTable.DefaultView.Sort = "ДатаЗаказа ASC";
                DataTable sortedTable = dataTable.DefaultView.ToTable();

                foreach (DataRow row in sortedTable.Rows)
                {
                    worksheet.Cells[currentRow, 1] = row["НомерЗаказа"].ToString();
                    worksheet.Cells[currentRow, 2] = Convert.ToDateTime(row["ДатаЗаказа"]).ToString("dd.MM.yyyy HH:mm");
                    worksheet.Cells[currentRow, 3] = row["Сотрудник"].ToString();
                    worksheet.Cells[currentRow, 4] = row["Клиент"].ToString();

                    decimal amount = Convert.ToDecimal(row["СуммаЗаказа"]);
                    worksheet.Cells[currentRow, 5] = $"{amount:N2} руб.";
                    worksheet.Cells[currentRow, 5].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                    worksheet.Cells[currentRow, 6] = row["СтатусЗаказа"].ToString();

                    for (int col = 1; col <= 6; col++)
                    {
                        worksheet.Cells[currentRow, col].Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                    }

                    totalRevenue += amount;
                    currentRow++;
                }

                currentRow++;
                worksheet.Cells[currentRow, 1] = "ИТОГО:";
                worksheet.Cells[currentRow, 1].Font.Bold = true;
                worksheet.Cells[currentRow, 5] = $"{totalRevenue:N2} руб.";
                worksheet.Cells[currentRow, 5].Font.Bold = true;
                worksheet.Cells[currentRow, 5].HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                currentRow++;
                worksheet.Cells[currentRow, 1] = "Количество заказов:";
                worksheet.Cells[currentRow, 2] = dataTable.Rows.Count.ToString();
                worksheet.Cells[currentRow, 2].Font.Bold = true;

                worksheet.Columns.AutoFit();

                excelApp.Visible = true;

                MessageBox.Show("Отчёт успешно создан!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при создании отчёта: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);

                try
                {
                    if (workbook != null) workbook.Close(false);
                    if (excelApp != null) excelApp.Quit();
                }
                catch { }
            }
            finally
            {
                if (excelApp != null && !excelApp.Visible)
                {
                    ReleaseObject(worksheet);
                    ReleaseObject(workbook);
                    ReleaseObject(excelApp);
                }
                else
                {
                    worksheet = null;
                    workbook = null;
                    excelApp = null;
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        private void SetRangeStyle(Excel.Range range, int fontSize, bool bold, Excel.XlHAlign alignment)
        {
            range.Font.Size = fontSize;
            range.Font.Bold = bold;
            range.HorizontalAlignment = alignment;
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
        }

        private DataTable GetRevenueData()
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(connStr.ConnectionString))
                {
                    connection.Open();

                    string query = @"
                        SELECT 
                            o.OrderID AS 'НомерЗаказа',
                            o.OrderDate AS 'ДатаЗаказа',
                            w.WorkerFIO AS 'Сотрудник',
                            c.CustomersFIO AS 'Клиент',
                            o.OrderPrice AS 'СуммаЗаказа',
                            o.OrderStatus AS 'СтатусЗаказа'
                        FROM `Order` o
                        JOIN Worker w ON o.OrderWorker = w.WorkerID
                        JOIN Customers c ON o.OrderCustomers = c.CustomersID
                        WHERE DATE(o.OrderDate) BETWEEN @StartDate AND @EndDate
                        AND o.OrderStatus = 'Выполнен'
                        ORDER BY o.OrderDate ASC";

                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@StartDate", dateTimePicker1.Value.Date);
                        cmd.Parameters.AddWithValue("@EndDate", dateTimePicker2.Value.Date);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                        {
                            adapter.Fill(dataTable);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при получении данных: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return dataTable;
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > dateTimePicker2.Value)
            {
                dateTimePicker2.Value = dateTimePicker1.Value;
            }
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < dateTimePicker1.Value)
            {
                dateTimePicker1.Value = dateTimePicker2.Value;
            }
        }
    }
}
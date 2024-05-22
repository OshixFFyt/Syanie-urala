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
using MetroFramework;
using ClosedXML.Excel;

namespace Syanie_urala
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            LoadData();
        }
        private void RefreshData()
        {
            try
            {
                // Очищаем стили ячеек перед обновлением данных
                ClearCellStyles();
                // Очищаем существующие данные в DataGridView
                metroGrid1.DataSource = null;
                // Создаем экземпляр класса подключения к базе данных
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();

                // Повторно загружаем данные из базы данных
                DataTable dataTable = new DataTable();
                string query = "SELECT * FROM equipment";
                using (MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                // Устанавливаем новый источник данных для DataGridView
                metroGrid1.DataSource = dataTable;
                // Восстанавливаем красные ячейки
                SetRussianColumnHeaders();
                // Преобразуем первую букву статуса в заглавную для каждой строки
                foreach (DataGridViewRow row in metroGrid1.Rows)
                {
                    DataGridViewCell statusCell = row.Cells["Status"];
                    if (statusCell != null && statusCell.Value != null)
                    {
                        string status = statusCell.Value.ToString();
                        if (!string.IsNullOrEmpty(status))
                        {
                            status = char.ToUpper(status[0]) + status.Substring(1);
                            statusCell.Value = status;
                        }
                    }
                    // Пересчитываем и сохраняем индексы красных строк после обновления данных
                    SaveRedRows();
                }
                // Восстанавливаем красные ячейки после обновления данных
                RestoreRedRows();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении данных: {ex.Message}");
            }
        }
        private void ClearCellStyles()
        {
            foreach (DataGridViewRow row in metroGrid1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    // Сбрасываем цвет ячейки
                    cell.Style.BackColor = Color.Empty;
                }
            }
        }
        private List<int> redRows = new List<int>();
        private void SaveRedRows()
        {
            redRows.Clear(); // Очищаем список перед сохранением
            foreach (DataGridViewRow row in metroGrid1.Rows)
            {
                // Получаем ячейку со значением статуса оборудования
                DataGridViewCell statusCell = row.Cells["Status"];

                // Проверяем, что ячейка не является пустой и содержит значение
                if (statusCell != null && statusCell.Value != null)
                {
                    // Получаем значение статуса оборудования в текущей строке
                    string status = statusCell.Value.ToString();

                    // Проверяем, является ли статус "Не работает"
                    if (status == "Не работает")
                    {
                        // Если оборудование не работает, добавляем индекс строки в список
                        redRows.Add(row.Index);
                    }
                }
            }
        }
        private void RestoreRedRows()
        {
            foreach (int rowIndex in redRows)
            {
                // Проверяем, что индекс строки находится в пределах количества строк в DataGridView
                if (rowIndex >= 0 && rowIndex < metroGrid1.Rows.Count)
                {
                    // Получаем строку по индексу и устанавливаем красный цвет для ячеек
                    DataGridViewRow row = metroGrid1.Rows[rowIndex];
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        cell.Style.BackColor = Color.Red;
                    }
                }
            }
        }
        private void LoadData()
        {
            try
            {
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();
                string query = "SELECT ID, Name, Type, Status, PurchaseDate, Cost, ResponsibleEmployee FROM equipment";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                DataTable table = new DataTable();
                adapter.Fill(table);
                metroGrid1.DataSource = table;
                // Устанавливаем тип данных для столбца PurchaseDate
                metroGrid1.Columns["PurchaseDate"].DefaultCellStyle.Format = "dd-MM-yyyy"; 
                metroGrid1.Columns["PurchaseDate"].ValueType = typeof(DateTime);
                // Делаем все столбцы кликабельными
                foreach (DataGridViewColumn column in metroGrid1.Columns)
                {
                    column.ReadOnly = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
            finally
            {
                Connects.DataBase db = new Connects.DataBase();
                db.CloseConnection();
            }
        }
        private void Main_Load(object sender, EventArgs e)
        {
            metroGrid1.Columns["ID"].HeaderText = "Идентификатор";
            metroGrid1.Columns["Name"].HeaderText = "Название";
            metroGrid1.Columns["Type"].HeaderText = "Тип";
            metroGrid1.Columns["Status"].HeaderText = "Статус";
            metroGrid1.Columns["PurchaseDate"].HeaderText = "Дата покупки";
            metroGrid1.Columns["Cost"].HeaderText = "Стоимость";
            metroGrid1.Columns["ResponsibleEmployee"].HeaderText = "Ответственный сотрудник";
            // Перебираем строки DataGridView
            foreach (DataGridViewRow row in metroGrid1.Rows)
            {
                // Получаем ячейку со значением статуса оборудования
                DataGridViewCell statusCell = row.Cells["Status"];

                // Проверяем, что ячейка не является пустой и содержит значение
                if (statusCell != null && statusCell.Value != null)
                {
                    // Получаем значение статуса оборудования в текущей строке
                    string status = statusCell.Value.ToString();

                    // Проверяем, является ли статус "Не работает"
                    if (status == "Не работает")
                    {
                        // Устанавливаем цвет ячейки столбца "Status" красным
                        statusCell.Style.BackColor = Color.Red;
                    }
                }
            }

        }
        private void SetRussianColumnHeaders()
        {
            // Устанавливаем русские названия столбцов в DataGridView
            metroGrid1.Columns["ID"].HeaderText = "Идентификатор";
            metroGrid1.Columns["Name"].HeaderText = "Название";
            metroGrid1.Columns["Type"].HeaderText = "Тип";
            metroGrid1.Columns["Status"].HeaderText = "Статус";
            metroGrid1.Columns["PurchaseDate"].HeaderText = "Дата покупки";
            metroGrid1.Columns["Cost"].HeaderText = "Стоимость";
            metroGrid1.Columns["ResponsibleEmployee"].HeaderText = "Ответственный сотрудник";
        }
        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Проверяем, что событие вызвано для строки DataGridView
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                // Получаем ссылку на объект DataGridView
                DataGridView dataGridView = (DataGridView)sender;

                // Проверяем, что событие вызвано для столбца с ценой
                if (dataGridView.Columns[e.ColumnIndex].Name == "Cost")
                {
                    // Получаем значение ячейки с ценой
                    object cellValue = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;

                    // Проверяем, что значение ячейки не является null
                    if (cellValue != null)
                    {
                        // Продолжаем обработку значения ячейки
                        // Например, выводим сообщение с ценой
                        MessageBox.Show($"Цена: {cellValue.ToString()}");
                    }
                    else
                    {
                        MessageBox.Show("Значение ячейки пусто.");
                    }
                }
            }
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();

                // Создаем команду SELECT
                MySqlCommand selectCommand = new MySqlCommand("SELECT * FROM equipment", Connects.DataBase.Conn);

                // Инициализируем адаптер с командой SELECT
                MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand);

                // Создаем команды INSERT, UPDATE и DELETE с помощью MySqlCommandBuilder
                MySqlCommandBuilder builder = new MySqlCommandBuilder(adapter);
                DataTable dataTable = (DataTable)metroGrid1.DataSource;

                // Обновляем данные в источнике данных 
                if (dataTable.GetChanges() != null)
                {
                    // Обновляем данные в базе данных
                    adapter.Update(dataTable);
                    MessageBox.Show("Изменения сохранены успешно.");
                    // Обновляем данные в DataGridView и восстанавливаем красные ячейки
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении изменений: {ex.Message}");
            }
            finally
            {
                
                Connects.DataBase db = new Connects.DataBase();
                db.CloseConnection();
            } 
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            RefreshData();
        }
        private void ExportToExcel()
        {
            try
            {
                // Проверяем, что есть данные для создания отчета
                if (metroGrid1.Rows.Count > 0)
                {
                    // Создаем новую книгу Excel
                    var workbook = new XLWorkbook();
                    var worksheet = workbook.Worksheets.Add("Отчет");

                    // Добавляем заголовки столбцов
                    for (int i = 0; i < metroGrid1.Columns.Count; i++)
                    {
                        worksheet.Cell(1, i + 1).Value = metroGrid1.Columns[i].HeaderText;
                    }
                    // Добавляем данные из DataGridView
                    for (int i = 0; i < metroGrid1.Rows.Count; i++)
                    {
                        for (int j = 0; j < metroGrid1.Columns.Count; j++)
                        {
                            // Проверяем, что ячейка не пустая
                            if (metroGrid1.Rows[i].Cells[j].Value != null)
                            {
                                worksheet.Cell(i + 2, j + 1).Value = metroGrid1.Rows[i].Cells[j].Value.ToString();
                            }
                            else
                            {
                                // Если ячейка пустая, записываем пустую строку
                                worksheet.Cell(i + 2, j + 1).Value = "";
                            }
                        }
                    }
                    // Сохраняем книгу Excel
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel Files|*.xlsx";
                    saveFileDialog.Title = "Сохранить отчет";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        workbook.SaveAs(saveFileDialog.FileName);
                        MessageBox.Show("Отчет успешно сгенерирован и сохранен.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Нет данных для создания отчета.", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при генерации отчета: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            try
            {
                // Проверяем, что выбрана хотя бы одна строка
                if (metroGrid1.SelectedRows.Count > 0)
                {
                    // Получаем выбранную строку
                    DataGridViewRow selectedRow = metroGrid1.SelectedRows[0];

                    // Запрашиваем подтверждение удаления у пользователя
                    DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить выбранную строку?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    // Если пользователь подтвердил удаление, удаляем строку из базы данных и DataGridView
                    if (result == DialogResult.Yes)
                    {
                        Connects.DataBase db = new Connects.DataBase();
                        db.OpenConnection();

                        // Получаем ID удаляемого оборудования из выбранной строки
                        int equipmentId = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                        // Создаем команду DELETE
                        MySqlCommand deleteCommand = new MySqlCommand($"DELETE FROM equipment WHERE ID = {equipmentId}", Connects.DataBase.Conn);

                        // Выполняем команду DELETE
                        deleteCommand.ExecuteNonQuery();

                        // Удаляем выбранную строку из DataGridView
                        metroGrid1.Rows.Remove(selectedRow);

                        // Обновляем данные в DataGridView и восстанавливаем красные ячейки
                        RefreshData();

                        MessageBox.Show("Строка успешно удалена.");
                    }
                }
                else
                {
                    MessageBox.Show("Не выбрана ни одна строка.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при удалении строки: {ex.Message}");
            }
            finally
            {
                Connects.DataBase db = new Connects.DataBase();
                db.CloseConnection();
            }
        }
    }
    
}

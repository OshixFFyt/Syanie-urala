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
using ClosedXML.Excel;

namespace Syanie_urala
{
    public partial class Orders : Form
    {
        private Connects.DataBase db = new Connects.DataBase();

        public Orders()
        {
            InitializeComponent();
        }
        private void LoadOrders()
        {
            try
            {
                // Открываем соединение с базой данных
                db.OpenConnection();

                // Создаем команду для выборки данных
                string query = "SELECT * FROM orders";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());

                // Создаем адаптер данных
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                // Создаем пустой DataTable для хранения данных
                DataTable dt = new DataTable();

                // Заполняем DataTable данными из базы данных
                adapter.Fill(dt);

                // Привязываем данные к элементу DataGridView на форме
                dataGridViewOrders.DataSource = dt;
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при загрузке данных: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }

        private void Orders_Load(object sender, EventArgs e)
        {
            LoadOrders();
            // Добавляем столбцы один раз при загрузке формы
            dataGridViewOrders.Columns["ID"].HeaderText = "Идентификатор";
            dataGridViewOrders.Columns["INN"].HeaderText = "ИНН";
            dataGridViewOrders.Columns["Address"].HeaderText = "Адрес";
            dataGridViewOrders.Columns["CompanyName"].HeaderText = "Название организации";
            dataGridViewOrders.Columns["TotalCost"].HeaderText = "Общая стоимость";
            dataGridViewOrders.Columns["MaterialID"].HeaderText = "Название материала";
            dataGridViewOrders.Columns["Quantity"].HeaderText = "Количество";

            
           
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            Addorders addOrderForm = new Addorders();
            addOrderForm.ShowDialog();
            LoadOrders();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Перебираем все выбранные строки
                foreach (DataGridViewRow selectedRow in dataGridViewOrders.SelectedRows)
                {
                    // Получаем ID выбранной строки
                    int orderID = Convert.ToInt32(selectedRow.Cells["ID"].Value);

                    // Открываем соединение с базой данных
                    db.OpenConnection();

                    // Создаем SQL-запрос для удаления записи из таблицы orders
                    string query = "DELETE FROM orders WHERE ID = @OrderID";
                    MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                    command.Parameters.AddWithValue("@OrderID", orderID);

                    // Выполняем запрос на удаление записи
                    command.ExecuteNonQuery();

                    // Обновляем данные в DataGridView
                    LoadOrders();
                }

                MessageBox.Show("Записи успешно удалены.");
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при удалении записей: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            if (dataGridViewOrders.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewOrders.SelectedRows[0];
                int orderID = Convert.ToInt32(selectedRow.Cells["ID"].Value);
                string inn = selectedRow.Cells["INN"].Value.ToString();
                string address = selectedRow.Cells["Address"].Value.ToString();
                string companyName = selectedRow.Cells["CompanyName"].Value.ToString();
                string totalCost = selectedRow.Cells["TotalCost"].Value.ToString();
                string materialID = selectedRow.Cells["MaterialID"].Value.ToString();
                string quantity = selectedRow.Cells["Quantity"].Value.ToString();

                EditOrder editForm = new EditOrder(orderID, inn, address, companyName, totalCost, materialID, quantity);
                editForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Выберите строку для редактирования.");
            }
        }
       

        private void metroButton6_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            try
            {
                // Открытие диалогового окна сохранения файла
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Excel файлы (*.xlsx)|*.xlsx|Все файлы (*.*)|*.*";
                saveFileDialog.Title = "Сохранить как Excel файл";
                saveFileDialog.FileName = "Заказы"; // Имя файла по умолчанию
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Создание нового документа Excel
                    using (XLWorkbook workbook = new XLWorkbook())
                    {
                        // Добавление листа
                        IXLWorksheet worksheet = workbook.Worksheets.Add("Заказы");
                        // Заполнение заголовков столбцов
                        for (int j = 0; j < dataGridViewOrders.Columns.Count; j++)
                        {
                            worksheet.Cell(1, j + 1).Value = dataGridViewOrders.Columns[j].HeaderText;
                        }
                        // Заполнение данных
                        for (int i = 0; i < dataGridViewOrders.Rows.Count; i++)
                        {
                            for (int j = 0; j < dataGridViewOrders.Columns.Count; j++)
                            {
                                object cellValue = dataGridViewOrders.Rows[i].Cells[j].Value;
                                worksheet.Cell(i + 2, j + 1).Value = (cellValue != null) ? cellValue.ToString() : "";
                            }
                        }
                        // Сохранение документа Excel
                        workbook.SaveAs(saveFileDialog.FileName);
                    }
                    MessageBox.Show("Данные успешно экспортированы в Excel файл.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте данных в Excel: " + ex.Message);
            }
        }
       

        private void metroButton5_Click(object sender, EventArgs e)
        {
           
            
            this.Close();
 
        }

        private void dataGridViewOrders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}

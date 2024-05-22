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

namespace Syanie_urala
{
    public partial class Addorders : Form
    {
        private Connects.DataBase db = new Connects.DataBase();

        public Addorders()
        {
            InitializeComponent();
            FillMaterialComboBox();
        }
        private void FillMaterialComboBox()
        {
            try
            {
                // Открываем соединение с базой данных
                db.OpenConnection();

                // Создаем SQL-запрос для выборки всех материалов
                string query = "SELECT MaterialName FROM materialaccounting";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());

                // Создаем адаптер данных
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                // Создаем пустой DataTable для хранения данных
                DataTable dt = new DataTable();

                // Заполняем DataTable данными из базы данных
                adapter.Fill(dt);

                // Очищаем ComboBox
                materialComboBox.Items.Clear();

                // Добавляем все материалы в ComboBox
                foreach (DataRow row in dt.Rows)
                {
                    materialComboBox.Items.Add(row["MaterialName"].ToString());
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при загрузке материалов: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }


        private void Addorders_Load(object sender, EventArgs e)
        {

        }

        private void innTextBox_TextChanged(object sender, EventArgs e)
        {
        
        }
        private void materialComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateTotalCost();
        }
        private void UpdateTotalCost()
        {
            if (!string.IsNullOrEmpty(materialComboBox.Text) && !string.IsNullOrEmpty(quantityTextBox.Text))
            {
                try
                {
                    decimal unitCost = GetUnitCost(materialComboBox.Text);
                    int quantity = int.Parse(quantityTextBox.Text);
                    decimal totalCost = unitCost * quantity;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка при расчете общей стоимости: " + ex.Message);
                }
            }
            else
            {
                
                
            }
        }
        private decimal GetUnitCost(string materialName)
        {
            try
            {
                // Открываем соединение с базой данных
                db.OpenConnection();

                // Создаем SQL-запрос для выборки стоимости материала
                string query = "SELECT UnitCost FROM materialaccounting WHERE MaterialName = @MaterialName";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                command.Parameters.AddWithValue("@MaterialName", materialName);

                // Выполняем запрос и получаем результат
                object result = command.ExecuteScalar();

                if (result != null)
                {
                    // Если стоимость найдена, возвращаем её
                    return Convert.ToDecimal(result);
                }
                else
                {
                    // Если стоимость не найдена, возвращаем 0
                    return 0.0m;
                }
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при получении стоимости материала: " + ex.Message);
                return 0.0m;
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }

        private void metroButton1_Click_1(object sender, EventArgs e)
        {

            try
            {
                // Собираем данные о заказе
                int inn = int.Parse(innTextBox.Text);
                string address = addressTextBox.Text;
                string companyName = companyNameTextBox.Text;
                string materialName = materialComboBox.Text;
                int quantity = int.Parse(quantityTextBox.Text);
                decimal unitCost = GetUnitCost(materialName);
                decimal totalCost = unitCost * quantity;



                // Открываем соединение с базой данных
                db.OpenConnection();

                // Создаем SQL-запрос на добавление данных
                string query = "INSERT INTO orders (INN, Address, CompanyName, TotalCost, MaterialID, Quantity) VALUES (@INN, @Address, @CompanyName, @TotalCost, @MaterialID, @Quantity)";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                command.Parameters.AddWithValue("@INN", inn);
                command.Parameters.AddWithValue("@Address", address);
                command.Parameters.AddWithValue("@CompanyName", companyName);
                command.Parameters.AddWithValue("@TotalCost", totalCost);
                command.Parameters.AddWithValue("@MaterialID", materialName);
                command.Parameters.AddWithValue("@Quantity", quantity);

                // Выполняем запрос на добавление данных
                command.ExecuteNonQuery();

                // Сообщение об успешном добавлении заказа
                MessageBox.Show("Заказ успешно добавлен.");

                // Закрываем форму
                this.Close();
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при добавлении заказа: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }
    }
}

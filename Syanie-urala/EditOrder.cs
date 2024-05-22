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
    public partial class EditOrder : Form
    {
        private Connects.DataBase db = new Connects.DataBase();
        private int orderID;
        private string inn;
        private string address;
        private string companyName;
        private string totalCost;
        private string materialID;
        private string quantity;
        public EditOrder(int orderID, string inn, string address, string companyName, string totalCost, string materialID, string quantity)
        {
            InitializeComponent();
            // Устанавливаем переданные значения в текстовые поля формы

            innTextBox.Text = inn;
            addressTextBox.Text = address;
            companyNameTextBox.Text = companyName;
            totalCostTextBox.Text = totalCost;
            materialIDTextBox.Text = materialID;
            quantityTextBox.Text = quantity;

            // Сохраняем переданные значения в приватные поля для дальнейшего использования
            this.orderID = orderID;
            this.inn = inn;
            this.address = address;
            this.companyName = companyName;
            this.totalCost = totalCost;
            this.materialID = materialID;
            this.quantity = quantity;
        }

        private void EditOrder_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            try
            {
                // Получаем измененные значения из текстовых полей
                string newINN = innTextBox.Text;
                string newAddress = addressTextBox.Text;
                string newCompanyName = companyNameTextBox.Text;
                string newTotalCost = totalCostTextBox.Text;
                string newMaterialID = materialIDTextBox.Text;
                string newQuantity = quantityTextBox.Text;

                // Открываем соединение с базой данных
                db.OpenConnection();

                // Создаем SQL-запрос для обновления записи в таблице orders
                string query = "UPDATE orders SET INN = @INN, Address = @Address, CompanyName = @CompanyName, TotalCost = @TotalCost, MaterialID = @MaterialID, Quantity = @Quantity WHERE ID = @OrderID";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                command.Parameters.AddWithValue("@INN", newINN);
                command.Parameters.AddWithValue("@Address", newAddress);
                command.Parameters.AddWithValue("@CompanyName", newCompanyName);
                command.Parameters.AddWithValue("@TotalCost", newTotalCost);
                command.Parameters.AddWithValue("@MaterialID", newMaterialID);
                command.Parameters.AddWithValue("@Quantity", newQuantity);
                command.Parameters.AddWithValue("@OrderID", orderID);

                // Выполняем запрос на обновление записи
                command.ExecuteNonQuery();

                // Сообщение об успешном сохранении изменений
                MessageBox.Show("Изменения успешно сохранены.");

                // Закрываем форму редактирования после сохранения изменений
                this.Close();
            }
            catch (Exception ex)
            {
                // Обрабатываем ошибку, если что-то пошло не так
                MessageBox.Show("Ошибка при сохранении изменений: " + ex.Message);
            }
            finally
            {
                // Закрываем соединение с базой данных
                db.CloseConnection();
            }
        }
    }
    
}

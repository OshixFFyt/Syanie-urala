using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework;
using MySql.Data.MySqlClient;

namespace Syanie_urala
{
    public partial class Reg : Form
    {
        public Reg()
        {
            InitializeComponent();
        }
        static string sha256(string randomString)
        {
            //Тут происходит криптографическая магия. Смысл данного метода заключается в том, что строка залетает в метод
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(randomString));
            foreach (byte theByte in crypto)
            {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
        private void Reg_Load(object sender, EventArgs e)
        {

        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            string firstName = metroTextBox1.Text;
            string lastName = metroTextBox2.Text;
            string username = metroTextBox3.Text;
            string password = sha256(metroTextBox4.Text);
            string role = metroComboBox1.SelectedItem.ToString(); // Получаем выбранную роль из ComboBox

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) || string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("Пожалуйста, заполните все поля.");
                return;
            }

            try
            {
                // Хешируем пароль
                

                // Подключаемся к базе данных
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();

                // Создаем SQL-запрос для вставки нового пользователя
                string query = "INSERT INTO employees (FirstName, LastName, Username, Password, Role) VALUES (@FirstName, @LastName, @Username, @Password, @Role)";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);

                // Передаем параметры запроса
                command.Parameters.AddWithValue("@FirstName", firstName);
                command.Parameters.AddWithValue("@LastName", lastName);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                command.Parameters.AddWithValue("@Role", role);

                // Выполняем запрос
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Регистрация успешна!");
                    this.Close(); // Закрываем форму регистрации после успешной регистрации
                    UpdateEmployeesTable();
                }
                else
                {
                    MessageBox.Show("Ошибка при регистрации.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при выполнении запроса: {ex.Message}");
            }
            finally
            {
                // В любом случае закрываем подключение к базе данных
                Connects.DataBase db = new Connects.DataBase();
                db.CloseConnection();
            }
        }
        private void UpdateEmployeesTable()
        {
            // Получаем форму, на которой расположен MetroGrid с сотрудниками (например, форма админа)
            Admin adminForm = Application.OpenForms.OfType<Admin>().FirstOrDefault();

            // Если форма админа открыта, обновляем таблицу сотрудников
            if (adminForm != null)
            {
                adminForm.FillEmployeesGrid();
            }
        }
        private void metroTextBox4_Click(object sender, EventArgs e)
        {

        }
    }
    
}

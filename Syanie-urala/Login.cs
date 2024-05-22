using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using static Syanie_urala.Materials;

namespace Syanie_urala
{
    public partial class Login : Form
    {
        private EmployeeDataAccess dataAccess = new EmployeeDataAccess();

        public Login()
        {
            InitializeComponent();
        }
        private string CalculateSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
        private string GetUserRole(string username)
        {
            try
            {
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();

                string query = "SELECT Role FROM employees WHERE Username = @Username";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);
                command.Parameters.AddWithValue("@Username", username);
                object result = command.ExecuteScalar();
                db.CloseConnection();
                if (result != null)
                {
                    return result.ToString();
                }
                
                else
                {
                    // Если пользователь не найден, вернуть null или другое значение по умолчанию
                    return null;
                }
                
            }
            catch
            {
                
                return null;
            }
            finally
            {
                
            }
           
        }

        private void Login_Load(object sender, EventArgs e)
        {

        }
        private void MetroButton1_Click_1(object sender, EventArgs e)
        {
            string username = metroTextBox1.Text;
            string password = metroTextBox2.Text;
            EmployeeDataAccess dataAccess = new EmployeeDataAccess();

            // Получаем результат аутентификации и роль пользователя
            var authResult = dataAccess.AuthenticateUser(username, password);
            string userRole = authResult.role;
            if (authResult.success)
            {
                // Устанавливаем значение роли текущего пользователя
                CurrentUser.Role = GetUserRole(metroTextBox1.Text);

                if (authResult.role == "Administrator")
                {
                    MessageBox.Show("Авторизация успешна! Добро пожаловать, администратор!");
                    Admin adminForm = new Admin();
                    adminForm.ShowDialog();
                    this.Hide();
                }
                else if (authResult.role == "User")
                {
                    MessageBox.Show("Авторизация успешна! Добро пожаловать, пользователь!");
                    Materials materialsForm = new Materials();
                    materialsForm.ShowDialog();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Ошибка! Роль пользователя не определена.");
                }
            }
            else
            {
                MessageBox.Show("Ошибка авторизации. Неправильный логин или пароль.");
            }
        }

        private void metroTextBox3_Click(object sender, EventArgs e)
        {
            
        }

        private void metroTextBox2_Click(object sender, EventArgs e)
        {

        }
    }
    
}

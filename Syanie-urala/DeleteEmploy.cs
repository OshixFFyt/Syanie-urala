using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Syanie_urala
{
    public partial class DeleteEmploy : Form
    {
        public DeleteEmploy()
        {
            InitializeComponent();
            metroComboBox1.SelectedIndexChanged += metroComboBox1_SelectedIndexChanged;
        }

        private string GetUserInfo(int userID)
        {
            string userInfo = string.Empty;
            Connects.DataBase db = new Connects.DataBase();
            db.OpenConnection();

            // SQL-запрос для получения информации о пользователе
            string selectQuery = $"SELECT FirstName, LastName, Username, Role FROM employees WHERE ID = @UserID";

            // Создаем команду для выполнения SQL-запроса
            using (var command = new MySqlCommand(selectQuery, db.GetConnection()))
            {
                // Добавляем параметр для userID
                command.Parameters.AddWithValue("@UserID", userID);

                // Выполняем запрос и получаем данные о пользователе
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string firstName = reader["FirstName"].ToString();
                        string lastName = reader["LastName"].ToString();
                        string username = reader["Username"].ToString();
                        string role = reader["Role"].ToString();
                        userInfo = $"Имя: {firstName}\nФамилия: {lastName}\nИмя пользователя: {username}\nРоль: {role}";
                    }
                    else
                    {
                        userInfo = "Пользователь с указанным ID не найден.";
                    }
                }
            }

            db.CloseConnection();
            return userInfo;
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            int selectedUserID = (int)metroComboBox1.SelectedItem; // Получаем выбранный ID из ComboBox
            string userInfo = metroLabel1.Text;

            // Проверяем, была ли получена информация о пользователе
            if (!string.IsNullOrEmpty(userInfo))
            {
                // Показываем информацию о пользователе в сообщении
                string confirmationMessage = $"Вы уверены, что хотите удалить следующего пользователя?\n\n{userInfo}";
                DialogResult result = MessageBox.Show(confirmationMessage, "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                // Если пользователь подтвердил удаление, выполняем удаление
                if (result == DialogResult.Yes)
                {
                    DeleteUser(selectedUserID);
                    // Очищаем информацию о пользователе из Label после удаления
                    metroLabel1.Text = string.Empty;
                    // Обновляем данные в ComboBox после удаления пользователя
                    FillComboBoxWithUserIDs();
                }
            }
            else
            {
                MessageBox.Show("Получите информацию о пользователе.");
            }
        }
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedUserID = (int)metroComboBox1.SelectedItem; // Получаем выбранный ID из ComboBox
            string userInfo = GetUserInfo(selectedUserID);
            metroLabel1.Text = userInfo;
        }


        private void DeleteUser(int userID)
        {
            Connects.DataBase db = new Connects.DataBase();
            db.OpenConnection();

            // SQL-запрос для удаления пользователя с определенным ID
            string deleteQuery = $"DELETE FROM employees WHERE ID = @UserID";

            // Создаем команду для выполнения SQL-запроса
            using (var command = new MySqlCommand(deleteQuery, db.GetConnection()))
            {
                // Добавляем параметр для userID
                command.Parameters.AddWithValue("@UserID", userID);

                // Выполняем запрос
                int rowsAffected = command.ExecuteNonQuery();

                // Проверяем количество затронутых строк
                if (rowsAffected > 0)
                {
                    MessageBox.Show("Пользователь успешно удален.");
                    Admin adminForm = new Admin();
                    adminForm.Show();
                    this.Hide(); // Скрываем текущую форму удаления
                }
                else
                {
                    MessageBox.Show("Пользователь с таким ID не найден.");
                }
            }

            db.CloseConnection();
        }
        private void FillComboBoxWithUserIDs()
        {
            // Очистка ComboBox перед заполнением
            metroComboBox1.Items.Clear();

            // Получение ID пользователей из базы данных и добавление их в ComboBox
            Connects.DataBase db = new Connects.DataBase();
            db.OpenConnection();
            string selectQuery = "SELECT ID FROM employees";
            using (var command = new MySqlCommand(selectQuery, db.GetConnection()))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int userID = reader.GetInt32(0); // Получаем ID из результата запроса
                        metroComboBox1.Items.Add(userID); // Добавляем ID в ComboBox
                    }
                }
            }
            db.CloseConnection();
        }

        private void DeleteEmploy_Load(object sender, EventArgs e)
        {
            FillComboBoxWithUserIDs();
        }
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Syanie_urala
{
    public class EmployeeDataAccess
    {
        private Connects.DataBase db = new Connects.DataBase();

        public (bool success, string role) AuthenticateUser(string username, string password)
        {
            string userRole = null;
            try
            {
                db.OpenConnection();

                string query = "SELECT COUNT(*) FROM employees WHERE Username = @Username AND Password = @PasswordHash";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);

                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@PasswordHash", CalculateSHA256(password));

                object result = command.ExecuteScalar();
                int count = result != null ? Convert.ToInt32(result) : 0;

                if (count > 0)
                {
                    // Получаем роль пользователя из базы данных
                    userRole = GetUserRole(username);
                    return (true, userRole);
                }
                else
                {
                    return (false, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
                return (false, null);
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private string GetUserRole(string username)
        {
            try
            {
                db.OpenConnection();

                string query = "SELECT Role FROM employees WHERE Username = @Username";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);
                command.Parameters.AddWithValue("@Username", username);

                object result = command.ExecuteScalar();
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
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при получении роли пользователя: {ex.Message}");
                return null;
            }
            finally
            {
                db.CloseConnection();
            }
        }

        private string CalculateSHA256(string input)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                foreach (byte b in hashBytes)
                {
                    stringBuilder.Append(b.ToString("x2"));
                }
                return stringBuilder.ToString();
            }
        }
        public DataTable GetEmployeesData()
        {
            DataTable dt = new DataTable();
            try
            {
                db.OpenConnection();

                string query = "SELECT * FROM employees";
                MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn);

                MySqlDataAdapter adapter = new MySqlDataAdapter(command);

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении запроса: {ex.Message}");
            }
            finally
            {
                db.CloseConnection();
            }
            return dt;
        }
    }
}


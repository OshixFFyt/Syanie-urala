using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Syanie_urala
{
    internal class Connects
    {
        public class DataBase 
        {

            public static MySqlConnection Conn { get; set; } = new MySqlConnection("server=localhost;port=3306;user=root;database=assets;password=");

            public void OpenConnection()
            {
                if (Conn.State == ConnectionState.Closed)
                    Conn.Open();
            }

            public void CloseConnection()
            {
                if (Conn.State == ConnectionState.Open)
                    Conn.Close();
            }

            public MySqlConnection GetConnection()
            {
                return Conn;
            }

            // Метод для выполнения SQL-запроса и возвращения результатов в виде DataTable
            public DataTable ExecuteQuery(string query)
            {
                DataTable dataTable = new DataTable();
                using (MySqlCommand command = new MySqlCommand(query, Conn))
                {
                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                return dataTable;
            }
        }
    }
}

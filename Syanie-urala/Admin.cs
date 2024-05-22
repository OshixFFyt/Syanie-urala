using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;





namespace Syanie_urala
{
    public partial class Admin : Form
    {
        private System.Windows.Forms.Timer timer;
        private PerformanceCounter cpuCounter;
        private PerformanceCounter memoryCounter;

        public Admin()
        {
            InitializeComponent();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            memoryCounter = new PerformanceCounter("Memory", "Committed Bytes");
            timer = new System.Windows.Forms.Timer { Interval = 5000 };
            timer.Tick += timer1_Tick;
            timer.Start();
        }


        private void metroButton1_Click(object sender, EventArgs e)
        {
            Reg osn = new Reg();
            osn.ShowDialog();
        }
        private void FillComboBox()
        {
            // Очистить ComboBox перед заполнением
            metroComboBox1.Items.Clear();

            try
            {
                // Открыть соединение с базой данных
                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();

                // Выполнить запрос для получения имен компьютеров из таблицы equipment
                string query = "SELECT Name FROM equipment";
                    using (MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn))
                    {
                        using (MySqlDataReader reader = command.ExecuteReader())
                        {
                            // Заполнить ComboBox значениями из столбца Name
                            while (reader.Read())
                            {
                                metroComboBox1.Items.Add(reader.GetString("Name"));
                            }
                        }
                    }

                    db.CloseConnection();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                // Получите текущие значения загрузки CPU и памяти
                float cpuLoad = cpuCounter.NextValue();
                float memoryLoad = memoryCounter.NextValue() / 1024 / 1024; // конвертировать в МБ

                // Обновите значения в UI
                UpdateUI(cpuLoad, memoryLoad);

                // Обновите значения ProgressBar
                UpdateProgressBar(cpuLoad, memoryLoad);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public ulong GetTotalMemorySize()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT TotalVisibleMemorySize FROM Win32_OperatingSystem");
            ManagementObjectCollection collection = searcher.Get();
            foreach (ManagementObject obj in collection)
            {
                return (ulong)obj["TotalVisibleMemorySize"] / (1024 * 1024); // Преобразовать в мегабайты
            }
            return 0;
        }
        public ulong GetAvailableMemorySize()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            foreach (ManagementObject obj in searcher.Get())
            {
                return (ulong)obj["FreePhysicalMemory"] / (1024 * 1024); // Преобразовать в мегабайты
            }
            return 0;
        }

        private void UpdateUI(float cpuLoad, float memoryLoad)
        {
            metroCPU.Text = $"Загрузка ЦП: {cpuLoad:F2}%";
            metroMemory.Text = $"Загрузка памяти: {memoryLoad:F2} МБ";
        }

        private void UpdateProgressBar(float cpuLoad, float memoryLoad)
        {
            // Получить общее количество оперативной памяти
            ulong totalMemory = GetTotalMemorySize();

            // Получить количество доступной оперативной памяти
            ulong availableMemory = GetAvailableMemorySize();

            // Вычислить используемую память как разницу между общим количеством и доступной
            ulong usedMemory = totalMemory - availableMemory;

            // Преобразуем memoryLoad в проценты относительно maxMemory
            int memoryPercentage = (int)Math.Round((usedMemory / (double)totalMemory) * 100);


            Debug.WriteLine($"Memory Load: {memoryLoad}, Memory Percentage: {memoryPercentage}");

            // Установим значения прогресс-бара
            metroProgressBarCPU.Value = (int)Math.Round(cpuLoad);
            metroProgressBarMemory.Value = memoryPercentage;
        }

        private void metroButton2_Click_1(object sender, EventArgs e)
        {
            try
            {
                timer.Stop();
                float cpuLoad = cpuCounter.NextValue();
                float memoryLoad = memoryCounter.NextValue() / 1024 / 1024;
                string selectedName = metroComboBox1.SelectedItem?.ToString();


                Connects.DataBase db = new Connects.DataBase();
                db.OpenConnection();
                string query = "SELECT id FROM equipment WHERE Name = @Name";
                using (MySqlCommand command = new MySqlCommand(query, Connects.DataBase.Conn))
                {
                    command.Parameters.AddWithValue("@Name", selectedName);
                    object equipmentID = command.ExecuteScalar();
                    if (equipmentID != null)
                    {
                        string insertQuery = "INSERT INTO resourcemonitoring (EquipmentID, DateTime, CPU_Load, Memory_Load) VALUES (@EquipmentID, @DateTime, @CPULoad, @MemoryLoad)";
                    command.CommandText = insertQuery;
                    command.Parameters.Clear();
                    command.Parameters.AddWithValue("@EquipmentID", equipmentID);
                    command.Parameters.AddWithValue("@DateTime", DateTime.Now);
                    command.Parameters.AddWithValue("@CPULoad", cpuLoad);
                    command.Parameters.AddWithValue("@MemoryLoad", memoryLoad);
                    command.ExecuteNonQuery();
                        MessageBox.Show("Сохранение завершено!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                UpdateProgressBar(cpuLoad, memoryLoad);
                timer.Start();
                db.CloseConnection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        public void FillEmployeesGrid()
        {
            metroGrid1.Rows.Clear();
            Connects.DataBase db = new Connects.DataBase();
            db.OpenConnection();
            string selectQuery = "SELECT ID, FirstName AS 'Имя', LastName AS 'Фамилия', Username AS 'Имя пользователя', Role AS 'Роль' FROM employees";
            using (var command = new MySqlCommand(selectQuery, db.GetConnection()))
            {
                using (var reader = command.ExecuteReader())
                {
                    // Добавляем столбцы к MetroGrid
                    metroGrid1.Columns.Add("ID", "ID");
                    metroGrid1.Columns.Add("FirstName", "Имя");
                    metroGrid1.Columns.Add("LastName", "Фамилия");
                    metroGrid1.Columns.Add("Username", "Имя пользователя");
                    metroGrid1.Columns.Add("Role", "Роль");

                    while (reader.Read())
                    {
                        // Создаем новую строку для MetroGrid и добавляем данные о сотруднике
                        metroGrid1.Rows.Add(reader["ID"], reader["Имя"], reader["Фамилия"], reader["Имя пользователя"], reader["Роль"]);
                    }
                }
            }
            db.CloseConnection();
        }

        private void Admin_Load(object sender, EventArgs e)
        {
            FillComboBox();
            FillEmployeesGrid();
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Close();
            
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            DeleteEmploy deleteForm = new DeleteEmploy();
            deleteForm.Show();
            
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
           Main main = new Main();
            main.Show();
            

        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            Materials materials = new Materials();
            materials.Show();
            
            
        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            Orders orders = new Orders();
            orders.Show();
           
        }

        private void metroButton8_Click(object sender, EventArgs e)
        {
            ResourceMonitoring resourceMonitoring = new ResourceMonitoring();
            resourceMonitoring.Show();
        }
    }
}
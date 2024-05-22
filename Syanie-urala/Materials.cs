using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ClosedXML.Excel;
using static Syanie_urala.Connects;

namespace Syanie_urala
{
    public partial class Materials : Form
    {
        private DataBase db = new DataBase();
        public Materials()
        {
            InitializeComponent();
        }
        private void BindDataToGrid()
        {
            try
            {
                db.OpenConnection();
                string query = "SELECT * FROM materialaccounting";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                MySqlDataReader reader = command.ExecuteReader();

                metroGrid1.Rows.Clear();
                while (reader.Read())
                {
                    metroGrid1.Rows.Add(
                        reader["ID"].ToString(), // Add ID column
                        reader["MaterialName"].ToString(),
                        reader["UnitOfMeasurement"].ToString(),
                        reader["Quantity"].ToString(),
                        reader["UnitCost"].ToString()
                    );
                }


                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при чтении данных из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void ImportFromExcel(string filePath)
        {
            try
            {
                using (var workbook = new XLWorkbook(filePath))
                {
                    var worksheet = workbook.Worksheet(1); // Предполагается, что данные находятся в первом листе

                    for (int row = 2; row <= worksheet.LastRowUsed().RowNumber(); row++)
                    {
                        string materialName = worksheet.Cell(row, 1).Value.ToString();
                        string unitOfMeasurement = worksheet.Cell(row, 2).Value.ToString();

                        int quantity;
                        if (!int.TryParse(worksheet.Cell(row, 3).Value.ToString(), out quantity))
                        {
                            MessageBox.Show($"Невозможно преобразовать количество в строке {row}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        decimal unitCost;
                        if (!decimal.TryParse(worksheet.Cell(row, 4).Value.ToString(), out unitCost))
                        {
                            MessageBox.Show($"Невозможно преобразовать стоимость в строке {row}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            continue;
                        }

                        InsertDataIntoDatabase(materialName, unitOfMeasurement, quantity, unitCost);
                    }

                    MessageBox.Show("Данные успешно импортированы из файла Excel.", "Импорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при импорте данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            BindDataToGrid();
        }
        private void InsertDataIntoDatabase(string materialName, string unitOfMeasurement, int quantity, decimal unitCost)
        {
            try
            {
                db.OpenConnection();
                string query = "INSERT INTO materialaccounting (MaterialName, UnitOfMeasurement, Quantity, UnitCost) VALUES (@MaterialName, @UnitOfMeasurement, @Quantity, @UnitCost)";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                command.Parameters.AddWithValue("@MaterialName", materialName);
                command.Parameters.AddWithValue("@UnitOfMeasurement", unitOfMeasurement);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@UnitCost", unitCost);
                command.ExecuteNonQuery();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при записи данных в базу данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        public void LoadDataFromDatabase()
        {
            try
            {
                db.OpenConnection();
                string query = "SELECT * FROM materialaccounting";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                MySqlDataReader reader = command.ExecuteReader();

                metroGrid1.Rows.Clear();

                while (reader.Read())
                {
                    int id = int.Parse(reader["ID"].ToString());
                    string materialName = reader["MaterialName"].ToString();
                    string unitOfMeasurement = reader["UnitOfMeasurement"].ToString();
                    int quantity = int.Parse(reader["Quantity"].ToString());
                    decimal unitCost = decimal.Parse(reader["UnitCost"].ToString());

                    metroGrid1.Rows.Add(id, materialName, unitOfMeasurement, quantity, unitCost);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке данных из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }



        private void Materials_Load(object sender, EventArgs e)
        {
            metroGrid1.Columns.Add("id", "ID");
            metroGrid1.Columns.Add("MaterialName", "Название");
            metroGrid1.Columns.Add("UnitOfMeasurement", "Единица измерения");
            metroGrid1.Columns.Add("Quantity", "Количество");
            metroGrid1.Columns.Add("UnitCost", "Стоимость");

            // Установка ширины столбцов (опционально)
            metroGrid1.Columns["MaterialName"].Width = 200;
            metroGrid1.Columns["UnitOfMeasurement"].Width = 150;
            metroGrid1.Columns["Quantity"].Width = 100;
            metroGrid1.Columns["UnitCost"].Width = 150;
            LoadDataFromDatabase();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel Files|*.xlsx;*.xls";
            openFileDialog.Title = "Выберите файл Excel";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ImportFromExcel(filePath);
                LoadDataFromDatabase();
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            DeleteMaterials materials = new DeleteMaterials();
            materials.Show();
            this.Hide();
        }
        public class CurrentUser
        {
            public static string Role { get; set; }
        }
        private void ExportToExcel()
        {
            try
            {
                using (XLWorkbook workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Materials");

                    // Set header row
                    worksheet.Cell(1, 1).Value = "ID";
                    worksheet.Cell(1, 2).Value = "Название материала";
                    worksheet.Cell(1, 3).Value = "Единица измерения";
                    worksheet.Cell(1, 4).Value = "Количество";
                    worksheet.Cell(1, 5).Value = "Стоимость";

                    // Export data from grid to Excel
                    for (int i = 0; i < metroGrid1.Rows.Count; i++)
                    {
                        if (metroGrid1.Rows[i].Cells["id"].Value != null)
                        {
                            worksheet.Cell(i + 2, 1).Value = metroGrid1.Rows[i].Cells["id"].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cell(i + 2, 1).Value = "";
                        }

                        if (metroGrid1.Rows[i].Cells["MaterialName"].Value != null)
                        {
                            worksheet.Cell(i + 2, 2).Value = metroGrid1.Rows[i].Cells["MaterialName"].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cell(i + 2, 2).Value = "";
                        }

                        if (metroGrid1.Rows[i].Cells["UnitOfMeasurement"].Value != null)
                        {
                            worksheet.Cell(i + 2, 3).Value = metroGrid1.Rows[i].Cells["UnitOfMeasurement"].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cell(i + 2, 3).Value = "";
                        }

                        if (metroGrid1.Rows[i].Cells["Quantity"].Value != null)
                        {
                            worksheet.Cell(i + 2, 4).Value = metroGrid1.Rows[i].Cells["Quantity"].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cell(i + 2, 4).Value = "";
                        }

                        if (metroGrid1.Rows[i].Cells["UnitCost"].Value != null)
                        {
                            worksheet.Cell(i + 2, 5).Value = metroGrid1.Rows[i].Cells["UnitCost"].Value.ToString();
                        }
                        else
                        {
                            worksheet.Cell(i + 2, 5).Value = "";
                        }
                    }

                    // Save the workbook to a file
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
                    saveFileDialog.FileName = "ExportedData.xlsx";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = saveFileDialog.FileName;
                        workbook.SaveAs(filePath);
                    }

                    MessageBox.Show("Данные успешно экспортированы в Excel!", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте данных в Excel: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            if (CurrentUser.Role == "Administrator")
            {
                this.Close();
            }
            else if (CurrentUser.Role == "User")
            {
                Login loginForm = new Login();
                loginForm.Show();
                this.Close();
            }
            else
            {
                // If role is not set, redirect to default form (e.g. Login)
                Login loginForm = new Login();
                loginForm.Show();
                this.Close();
            }
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void metroButton7_Click(object sender, EventArgs e)
        {
            Orders order = new Orders();
            order.Show();
            
        }

        private void metroButton6_Click(object sender, EventArgs e)
        {
            Main main = new Main();
            main.Show();
        }
    }

}

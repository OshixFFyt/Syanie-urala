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
using static Syanie_urala.Connects;

namespace Syanie_urala
{

    public partial class DeleteMaterials : Form
    {
        private DataBase db = new DataBase();
        public DeleteMaterials()
        {
            InitializeComponent();
            FillComboBox();
        }
        private void FillComboBox()
        {
            try
            {
                db.OpenConnection();
                string query = "SELECT ID, MaterialName FROM materialaccounting";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    int id = int.Parse(reader["ID"].ToString());
                    string materialName = reader["MaterialName"].ToString();
                    metroComboBox1.Items.Add(new KeyValuePair<int, string>(id, materialName));
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при загрузке ID и названий материалов из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (metroComboBox1.SelectedItem != null)
            {
                var selectedMaterial = (KeyValuePair<int, string>)metroComboBox1.SelectedItem;
                string materialName = selectedMaterial.Value;
                metroLabel1.Text = materialName;
            }
        }

        private void DeleteRowByID(int id)
        {
            try
            {
                db.OpenConnection();
                string query = "DELETE FROM materialaccounting WHERE ID = @ID";
                MySqlCommand command = new MySqlCommand(query, db.GetConnection());
                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при удалении строки из базы данных: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                db.CloseConnection();
            }
        }
        private void RefreshDataOnOtherForm(int idToDelet, string materialName)
        {
            // Получаем ссылку на форму Materials и вызываем метод обновления данных на ней
            if (Application.OpenForms["Materials"] is Materials materialsForm)
            {
                materialsForm.LoadDataFromDatabase();
            }
        }

        private void DeleteMaterials_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            var selectedMaterial = (KeyValuePair<int, string>)metroComboBox1.SelectedItem;
            int idToDelete = selectedMaterial.Key;
            string materialName = selectedMaterial.Value;

            DeleteRowByID(idToDelete);
            RefreshDataOnOtherForm(idToDelete, materialName);
            MessageBox.Show("Строка успешно удалена.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Materials materials = new Materials();
            materials.Show();
            this.Hide();

        }

    }
}

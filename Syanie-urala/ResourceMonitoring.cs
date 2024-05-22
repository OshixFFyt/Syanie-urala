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
    public partial class ResourceMonitoring : Form
    {
        private Connects.DataBase db;

        public ResourceMonitoring()
        {
            InitializeComponent();
            db = new Connects.DataBase();
        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ResourceMonitoring_Load(object sender, EventArgs e)
        {
            try
            {
                db.OpenConnection();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void LoadData()
        {
            try
            {
                // Загрузка данных из базы данных и отображение их в DataGridView
                DataTable dataTable = db.ExecuteQuery("SELECT * FROM resourcemonitoring");

                // Задаем заголовки столбцов на русском языке
                dataTable.Columns["ID"].ColumnName = "Идентификатор";
                dataTable.Columns["EquipmentID"].ColumnName = "Идентификатор оборудования";
                dataTable.Columns["DateTime"].ColumnName = "Дата и время";
                dataTable.Columns["CPU_Load"].ColumnName = "Загрузка ЦП";
                dataTable.Columns["Memory_Load"].ColumnName = "Загрузка памяти";

                metroGrid1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}

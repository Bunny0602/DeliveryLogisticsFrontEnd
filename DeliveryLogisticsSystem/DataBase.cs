using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DeliveryLogisticsSystem
{
    public class DataBase
    {
        private MySqlConnection connection;
        private string server = "localhost";
        private string database = "DLTsystem";
        private string username = "root";
        private string password = "";

        public DataBase()
        {
            string connectionString = $"Server={server};Database={database};User ID={username};Password={password};";
            connection = new MySqlConnection(connectionString);
        }

        public MySqlConnection GetConnection()
        {
            return connection;
        }

        public void OpenConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Closed)
                {
                    connection.Open();
                    MessageBox.Show("Connection Successful");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Connection Failed: " + ex.Message);
            }
        }

        public void CloseConnection()
        {
            try
            {
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                    MessageBox.Show("Connection Closed");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error Closing Connection: " + ex.Message);
            }
        }
    }
}

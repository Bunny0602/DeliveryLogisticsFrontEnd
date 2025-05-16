using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace DeliveryLogisticsSystem
{
    public partial class AdminDashboard : Form
    {

        private string userId;

        public AdminDashboard(string userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserData();
            LoadPersonnelData();
        }

        private void LoadUserData()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "SELECT user_id AS 'User_ID', email AS 'Email', fullname AS 'Full Name', address AS 'Address', phonenumber AS 'Phone Number', role AS 'Role' FROM users WHERE role <> 'admin'";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable usertable = new DataTable();
                    adapter.Fill(usertable);
                    dataUser.DataSource = usertable;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading user data: " + ex.Message);
                }
            }
        }

        private void LoadPersonnelData()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "SELECT personnel_id AS 'Personnel ID', email AS 'Email', fullname AS 'Full Name', address AS 'Address', phonenumber AS 'Phone Number', status AS 'Status', role AS 'Role' FROM personnel";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable personTable = new DataTable();
                    adapter.Fill(personTable);
                    dataPersonnel.DataSource = personTable;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading personnel data: " + ex.Message);
                }
            }
        }

        private void UserRefresh_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        private void PersonnelRefresh_Click(object sender, EventArgs e)
        {
            LoadPersonnelData();
        }

        private void btnViewUser_Click(object sender, EventArgs e)
        {
            if (dataUser.SelectedRows.Count > 0)
            {

                // Get the selected User ID
                string userId = dataUser.SelectedRows[0].Cells["User_ID"].Value.ToString();

                // Open the ViewUserData form and pass the User ID to it
                ViewUserData viewUserData = new ViewUserData(userId);
                viewUserData.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a user to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeleteUser_Click(object sender, EventArgs e)
        {
            if (dataUser.SelectedRows.Count > 0)
            {

                string userId = dataUser.SelectedRows[0].Cells["User_ID"].Value.ToString();
                string userEmail = dataUser.SelectedRows[0].Cells["Email"].Value.ToString();

                DialogResult result = MessageBox.Show($"Are you sure you want to delete user {userEmail}?", "Delete User", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DeleteUser(userId);
                    LoadUserData();
                }
            }
            else
            {
                MessageBox.Show("Please select a user to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteUser(string userId)
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM users WHERE user_id = @userId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("User deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("User not found or already deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error deleting user: " + ex.Message);
                }
            }
        }
    }
}

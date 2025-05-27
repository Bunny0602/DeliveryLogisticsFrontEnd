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
        private string adminId;
        private string adminEmail;
        private string userEmail;
        private string userRole;

        // Constructor initializes the dashboard and loads user and personnel data.
        public AdminDashboard(string userId, string adminEmail, string userRole)
        {
            InitializeComponent();

            this.userId = userId;
            this.adminId = userId;
            this.adminEmail = adminEmail;
            this.userRole = userRole;

            LoadUserData();
            LoadPersonnelData();
            LoadOrderData();
            LoadTotalUsers();
            LoadTotalOrders();
            LoadTotalMoney();
        }

        private void LoadUserData(string searchQuery = "")
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT user_id AS 'User_ID', email AS 'Email', fullname AS 'Full Name', 
                               address AS 'Address', phonenumber AS 'Phone Number', role AS 'Role'
                        FROM users 
                        WHERE role <> 'admin'
                          AND (email LIKE @search OR fullname LIKE @search OR phonenumber LIKE @search 
                               OR @search = '')";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
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

        private void txtSearchUser_TextChanged(object sender, EventArgs e)
        {
            LoadUserData(txtSearchUser.Text.Trim());
        }

        // Loads all personnel records from the database and displays them in the dataPersonnel DataGridView.
        private void LoadPersonnelData(string searchQuery = "")
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT personnel_id AS 'Personnel ID', email AS 'Email', fullname AS 'Full Name', 
                               address AS 'Address', phonenumber AS 'Phone Number', status AS 'Status', role AS 'Role' 
                        FROM personnel 
                        WHERE email LIKE @search OR fullname LIKE @search OR phonenumber LIKE @search 
                              OR @search = ''";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
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

        private void txtPersonnel_TextChanged(object sender, EventArgs e)
        {
            LoadPersonnelData(txtPersonnel.Text.Trim());
        }

        private void LoadOrderData(string searchQuery = "")
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT
                            orders.order_id,
                            users.email, 
                            orders.name, 
                            orders.pickup_location, 
                            orders.dropoff_name, 
                            orders.dropoff_location, 
                            orders.personnelName, 
                            orders.phonenumber, 
                            orders.price, 
                            orders.status
                        FROM orders
                        LEFT JOIN users ON orders.user_id = users.user_id
                        WHERE users.email LIKE @search OR orders.name LIKE @search 
                              OR orders.pickup_location LIKE @search OR orders.dropoff_name LIKE @search
                              OR orders.dropoff_location LIKE @search OR orders.personnelName LIKE @search 
                              OR @search = ''";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@search", "%" + searchQuery + "%");

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable orderTable = new DataTable();
                    adapter.Fill(orderTable);

                    dataOrder.DataSource = orderTable;

                    if (orderTable.Columns.Count >= 10)
                    {
                        dataOrder.Columns[0].HeaderText = "Order ID";
                        dataOrder.Columns[1].HeaderText = "User Email";
                        dataOrder.Columns[2].HeaderText = "Name";
                        dataOrder.Columns[3].HeaderText = "Pickup Location";
                        dataOrder.Columns[4].HeaderText = "Dropoff Name";
                        dataOrder.Columns[5].HeaderText = "Dropoff Location";
                        dataOrder.Columns[6].HeaderText = "Personnel Name";
                        dataOrder.Columns[7].HeaderText = "Phone Number";
                        dataOrder.Columns[8].HeaderText = "Price";
                        dataOrder.Columns[9].HeaderText = "Status";

                        dataOrder.Columns[0].Width = 60;
                        dataOrder.Columns[1].Width = 150;
                        dataOrder.Columns[2].Width = 150;
                        dataOrder.Columns[3].Width = 150;
                        dataOrder.Columns[4].Width = 150;
                        dataOrder.Columns[5].Width = 150;
                        dataOrder.Columns[6].Width = 150;
                        dataOrder.Columns[7].Width = 150;
                        dataOrder.Columns[8].Width = 80;
                        dataOrder.Columns[9].Width = 100;
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading order data: " + ex.Message);
                }
            }
        }

        private void txtSearchOrder_TextChanged(object sender, EventArgs e)
        {
            LoadOrderData(txtSearchOrder.Text.Trim());
        }

        // Refreshes the user data when the refresh button is clicked.
        private void UserRefresh_Click(object sender, EventArgs e)
        {
            LoadUserData();
        }

        // Refreshes the personnel data when the refresh button is clicked.
        private void PersonnelRefresh_Click(object sender, EventArgs e)
        {
            LoadPersonnelData();
        }

        // Opens the ViewUserData form for the selected user.
        private void btnViewUser_Click(object sender, EventArgs e)
        {
            if (dataUser.SelectedRows.Count > 0)
            {
                string userId = dataUser.SelectedRows[0].Cells["User_ID"].Value.ToString();
                ViewUserData viewUserData = new ViewUserData(userId);
                viewUserData.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a user to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Deletes the selected user after confirmation.
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

        // Helper method to delete a user from the database.
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

        // Opens the AddPersonnelForm to add a new personnel and refreshes the personnel data after closing.
        private void pictureBox15_Click(object sender, EventArgs e)
        {
            AddPersonnelForm addPersonnelForm = new AddPersonnelForm();
            addPersonnelForm.ShowDialog();
            LoadPersonnelData();
        }

        // Opens the ViewPersonnelForm for the selected personnel.
        private void btnViewPersonnel_Click(object sender, EventArgs e)
        {
            if (dataPersonnel.SelectedRows.Count > 0)
            {
                string personnelId = dataPersonnel.SelectedRows[0].Cells["Personnel ID"].Value.ToString();
                ViewPersonnelForm viewPersonnelForm = new ViewPersonnelForm(personnelId);
                viewPersonnelForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a personnel to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // Opens the UpdatePersonnelForm to edit the selected personnel and refreshes the personnel data after closing.
        private void btnEditPersonnel_Click(object sender, EventArgs e)
        {
            if (dataPersonnel.SelectedRows.Count > 0)
            {
                string personnelId = dataPersonnel.SelectedRows[0].Cells["Personnel ID"].Value.ToString();
                UpdatePersonnelForm editPersonnelForm = new UpdatePersonnelForm(personnelId);
                editPersonnelForm.ShowDialog();
                LoadPersonnelData();
            }
            else
            {
                MessageBox.Show("Please select a personnel to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnDeletePersonnel_Click(object sender, EventArgs e)
        {
            if (dataPersonnel.SelectedRows.Count > 0)
            {
                string personnelId = dataPersonnel.SelectedRows[0].Cells["Personnel ID"].Value.ToString();
                string FullName = dataPersonnel.SelectedRows[0].Cells["Full Name"].Value.ToString();
                DialogResult result = MessageBox.Show($"Are you sure you want to delete personnel {FullName}?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DeletePersonnel(personnelId);
                }
            }
            else
            {
                MessageBox.Show("Please select a personnel to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeletePersonnel(string personnelId)
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "DELETE FROM personnel WHERE personnel_id = @PersonnelID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelID", personnelId);
                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Personnel deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadPersonnelData();
                    }
                    else
                    {
                        MessageBox.Show("Personnel not found or already deleted.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error deleting personnel: " + ex.Message);
                }
            }
        }

        private void LoadTotalUsers()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role = 'user'";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int totalUsers = Convert.ToInt32(cmd.ExecuteScalar());
                    totalUserCount.Text = " " + totalUsers;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total users: " + ex.Message);
                }
            }
        }

        private void LoadTotalOrders()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM orders";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    int totalOrders = Convert.ToInt32(cmd.ExecuteScalar());
                    totalOrderCount.Text = " " + totalOrders;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total orders: " + ex.Message);
                }
            }
        }

        private void LoadTotalMoney()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT IFNULL(SUM(price), 0) FROM orders";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    decimal totalMoney = Convert.ToDecimal(cmd.ExecuteScalar());
                    totalIncomeCount.Text = " " + totalMoney.ToString("N2");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading total money: " + ex.Message);
                }
            }
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            CreateOrderForm createOrderForm = new CreateOrderForm(userId, userEmail, userRole);
            createOrderForm.Show();
            this.Hide();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            this.Close();
        }

        private void btnViewCompleteOrder_Click(object sender, EventArgs e)
        {
            if (dataOrder.SelectedRows.Count > 0)
            {
                string status = dataOrder.SelectedRows[0].Cells["status"].Value.ToString();
                string orderId = dataOrder.SelectedRows[0].Cells["order_id"].Value.ToString();

                if (status.Equals("Delivered", StringComparison.OrdinalIgnoreCase))
                {
                    
                    ViewCompleOrderForm viewOrderComplete = new ViewCompleOrderForm(orderId);
                    viewOrderComplete.ShowDialog();
                }
                else
                {
                    MessageBox.Show("This order is not yet delivered.", "Cannot View", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to view.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}


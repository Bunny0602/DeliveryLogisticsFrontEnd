using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows.Forms;
using System.Timers;

namespace DeliveryLogisticsSystem
{
    public partial class UserDashboardForm : Form
    {
        private string userId;
        private string userEmail;
        private string userRole;
        private int orderId;
        private string role;

        private System.Timers.Timer refreshTimer;

        public UserDashboardForm(string userId, string userEmail, string userRole, int orderId, string role)
        {
            InitializeComponent();
            this.userId = userId;
            this.userEmail = userEmail;
            this.userRole = userRole;

            LoadUserOrders();
            InitializeRefreshTimer();
            this.orderId = orderId;
            this.role = role;
        }

        public UserDashboardForm(string userId, string userEmail, string role)
        {
            InitializeComponent();
            this.userId = userId;
            this.userEmail = userEmail;
            this.role = role;


            LoadUserOrders();
            InitializeRefreshTimer();
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            int newOrderId = 0;
            CreateOrderForm createOrderForm = new CreateOrderForm(userId, userEmail, userRole, newOrderId);
            createOrderForm.Show();
            this.Hide();
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            UserProfileForm userProfileForm = new UserProfileForm(userId);
            userProfileForm.Show();
        }

        private void LoadUserOrders()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"
                        SELECT 
                            order_id AS 'OrderID',
                            email AS 'Email',
                            name AS 'Name',
                            pickup_location AS 'Pickup Location',
                            dropoff_name AS 'DropOff Name',
                            dropoff_location AS 'Dropoff Location',
                            phonenumber AS 'Phone Number',
                            price AS 'Price',
                            status AS 'Status'
                        FROM orders 
                        WHERE user_id = @UserId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserId", userId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataUserOrder.DataSource = dt;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading orders: " + ex.Message);
                }
            }
        }

        private void InitializeRefreshTimer()
        {
            refreshTimer = new System.Timers.Timer(60000); // 1 minute
            refreshTimer.Elapsed += RefreshTimer_Elapsed;
            refreshTimer.Start();
        }

        private void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => LoadUserOrders()));
            }
            else
            {
                LoadUserOrders();
            }
        }

        private void UserDashboardForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (dataUserOrder.SelectedRows.Count > 0)
            {
                // Use the display name 'OrderID' as defined in the SELECT statement alias
                int orderId = Convert.ToInt32(dataUserOrder.SelectedRows[0].Cells["OrderID"].Value);

                EditOrderForm editOrderForm = new EditOrderForm(userId, orderId);
                editOrderForm.ShowDialog();

                LoadUserOrders();
            }
            else
            {
                MessageBox.Show("Please select an order to edit.");
            }
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            this.Close();
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            if (dataUserOrder.SelectedRows.Count > 0)
            {
                string email = dataUserOrder.SelectedRows[0].Cells["Email"].Value.ToString();
                string pickup = dataUserOrder.SelectedRows[0].Cells["Pickup Location"].Value.ToString();
                string dropoffName = dataUserOrder.SelectedRows[0].Cells["DropOff Name"].Value.ToString();
                string dropoff = dataUserOrder.SelectedRows[0].Cells["Dropoff Location"].Value.ToString();
                string phone = dataUserOrder.SelectedRows[0].Cells["Phone Number"].Value.ToString();
                string status = dataUserOrder.SelectedRows[0].Cells["Status"].Value.ToString();

                string personnel = "N/A";

                ViewOrderForm viewOrderForm = new ViewOrderForm(email, pickup, dropoffName, dropoff, personnel, phone, status);
                viewOrderForm.Show();
            }
            else
            {
                MessageBox.Show("Please select an order to view.");
            }
        }

        private void btndelete_Click(object sender, EventArgs e)
        {
            if (dataUserOrder.SelectedRows.Count > 0)
            {
                // Get selected Order ID
                int selectedOrderId = Convert.ToInt32(dataUserOrder.SelectedRows[0].Cells["OrderID"].Value);

                // Confirm delete
                DialogResult result = MessageBox.Show("Are you sure you want to delete this order?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    DataBase db = new DataBase();
                    using (MySqlConnection conn = db.GetConnection())
                    {
                        try
                        {
                            conn.Open();

                            string query = "DELETE FROM orders WHERE order_id = @OrderId AND user_id = @UserId";

                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            cmd.Parameters.AddWithValue("@OrderId", selectedOrderId);
                            cmd.Parameters.AddWithValue("@UserId", userId);

                            int rowsAffected = cmd.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                MessageBox.Show("Order deleted successfully.");
                                LoadUserOrders(); // Refresh the table
                            }
                            else
                            {
                                MessageBox.Show("Failed to delete order. It may not belong to you.");
                            }
                        }
                        catch (MySqlException ex)
                        {
                            MessageBox.Show("Error deleting order: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.");
            }
        }
    }
}

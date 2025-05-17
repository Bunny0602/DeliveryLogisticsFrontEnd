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

        private System.Timers.Timer refreshTimer;

        public UserDashboardForm(string userId, string userEmail, string userRole)
        {
            InitializeComponent();
            this.userId = userId;
            this.userEmail = userEmail;
            this.userRole = userRole;

            LoadUserOrders();
            InitializeRefreshTimer();
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            CreateOrderForm createOrderForm = new CreateOrderForm(userId, userEmail, userRole);
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
                            order_id AS 'Order ID',
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
    }
}

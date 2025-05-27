using MySql.Data.MySqlClient;
using System;
using System.Windows.Forms;

namespace DeliveryLogisticsSystem
{
    public partial class ViewCompleOrderForm : Form
    {
        private string orderId;

        public ViewCompleOrderForm(string orderId)
        {
            InitializeComponent();
            this.orderId = orderId;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            try
            {
                DataBase db = new DataBase();

                using (MySqlConnection conn = db.GetConnection())
                {
                    conn.Open();

                    string query = @"
                SELECT o.email, u.fullname, o.pickup_location, o.dropoff_location, o.dropoff_name, o.personnelName, o.price, o.status
                FROM orders o
                JOIN users u ON o.email = u.email
                WHERE o.order_id = @orderId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@orderId", orderId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmail.Text = reader["email"].ToString();
                                txtFullName.Text = reader["fullname"].ToString();
                                txtPickLocation.Text = reader["pickup_location"].ToString();
                                txtDropLocation.Text = reader["dropoff_location"].ToString();
                                txtDropName.Text = reader["dropoff_name"].ToString();
                                txtPersonnel.Text = reader["personnelName"].ToString();
                                txtPrice.Text = reader["price"].ToString();
                                txtStatus.Text = reader["status"].ToString();
                            }
                            else
                            {
                                MessageBox.Show("Order not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                this.Close();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading order details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }




        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

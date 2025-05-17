using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Timers;
using System.Windows.Forms;

namespace DeliveryLogisticsSystem
{
    public partial class PersonnelDashboardForm : Form
    {
        private string personnelId;
        private string personnelEmail;
        private string personnelRole;
        private System.Timers.Timer statusTimer;

        public PersonnelDashboardForm(string personnelId, string personnelEmail, string personnelRole)
        {
            InitializeComponent();
            this.personnelId = personnelId;
            this.personnelEmail = personnelEmail;
            this.personnelRole = personnelRole;

            LoadAssignedOrders();
            InitializeStatusTimer();
        }

        private void LoadAssignedOrders()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"
                        SELECT order_id AS 'Order ID', email AS 'Email', pickup_location AS 'Pickup Location',
                               dropoff_name AS 'DropOff Name', dropoff_location AS 'Dropoff Location', 
                               phonenumber AS 'Phone Number', price AS 'Price', status AS 'Status'
                        FROM orders
                        WHERE status = 'Pending' 
                           OR (personnel_id = @PersonnelId AND status IN ('To Ship', 'To Receive', 'Delivered'))";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelId", personnelId);

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable orderTable = new DataTable();
                    adapter.Fill(orderTable);

                    dataPersonnelOrders.DataSource = orderTable;
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading assigned orders: " + ex.Message);
                }
            }
        }

        private void InitializeStatusTimer()
        {
            statusTimer = new System.Timers.Timer(20000);
            statusTimer.Elapsed += UpdateOrderStatus;
            statusTimer.Start();
        }

        private void UpdateOrderStatus(object sender, ElapsedEventArgs e)
        {
            try
            {
                using (MySqlConnection conn = new DataBase().GetConnection())
                {
                    conn.Open();
                    string query = "SELECT order_id, status FROM orders WHERE personnel_id = @PersonnelId";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelId", personnelId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        var ordersToUpdate = new List<(int orderId, string newStatus)>();

                        while (reader.Read())
                        {
                            int orderId = reader.GetInt32("order_id");
                            string status = reader.GetString("status");

                            if (status == "To Ship")
                                ordersToUpdate.Add((orderId, "To Receive"));
                            else if (status == "To Receive")
                                ordersToUpdate.Add((orderId, "Delivered"));
                        }

                        reader.Close();

                        foreach (var order in ordersToUpdate)
                        {
                            UpdateOrderStatusInDatabase(order.orderId, order.newStatus);
                        }
                    }
                }

                if (this.InvokeRequired)
                {
                    this.Invoke(new Action(() => LoadAssignedOrders()));
                }
                else
                {
                    LoadAssignedOrders();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating order status: " + ex.Message);
            }
        }

        private void UpdateOrderStatusInDatabase(int orderId, string newStatus)
        {
            using (MySqlConnection conn = new DataBase().GetConnection())
            {
                conn.Open();
                string query = "UPDATE orders SET status = @Status WHERE order_id = @OrderId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Status", newStatus);
                cmd.Parameters.AddWithValue("@OrderId", orderId);
                cmd.ExecuteNonQuery();
            }
        }

        private void btnViewOrder_Click(object sender, EventArgs e)
        {
            if (dataPersonnelOrders.SelectedRows.Count > 0)
            {
                string email = dataPersonnelOrders.SelectedRows[0].Cells["Email"].Value.ToString();
                string pickup = dataPersonnelOrders.SelectedRows[0].Cells["Pickup Location"].Value.ToString();
                string dropoffName = dataPersonnelOrders.SelectedRows[0].Cells["DropOff Name"].Value.ToString();
                string dropoff = dataPersonnelOrders.SelectedRows[0].Cells["Dropoff Location"].Value.ToString();
                string phone = dataPersonnelOrders.SelectedRows[0].Cells["Phone Number"].Value.ToString();
                string status = dataPersonnelOrders.SelectedRows[0].Cells["Status"].Value.ToString();

                ViewOrderForm viewOrderForm = new ViewOrderForm(email, pickup, dropoffName, dropoff, personnelEmail, phone, status);
                viewOrderForm.Show();
            }
            else
            {
                MessageBox.Show("Please select an order to view.");
            }
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            if (dataPersonnelOrders.SelectedRows.Count > 0)
            {
                int orderId = Convert.ToInt32(dataPersonnelOrders.SelectedRows[0].Cells["Order ID"].Value);
                string status = dataPersonnelOrders.SelectedRows[0].Cells["Status"].Value.ToString();

                if (status == "Pending")
                {
                    using (MySqlConnection conn = new DataBase().GetConnection())
                    {
                        conn.Open();
                        string query = @"
                            UPDATE orders 
                            SET personnel_id = @PersonnelId, status = 'To Ship' 
                            WHERE order_id = @OrderId AND status = 'Pending'";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@PersonnelId", personnelId);
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order accepted successfully!");
                    LoadAssignedOrders();
                }
                else
                {
                    MessageBox.Show("This order is not available for acceptance.");
                }
            }
            else
            {
                MessageBox.Show("Please select an order to accept.");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            if (dataPersonnelOrders.SelectedRows.Count > 0)
            {
                int orderId = Convert.ToInt32(dataPersonnelOrders.SelectedRows[0].Cells["Order ID"].Value);
                string status = dataPersonnelOrders.SelectedRows[0].Cells["Status"].Value.ToString();

                if (status != "Delivered")
                {
                    using (MySqlConnection conn = new DataBase().GetConnection())
                    {
                        conn.Open();
                        string query = @"
                            UPDATE orders 
                            SET personnel_id = NULL, status = 'Pending' 
                            WHERE order_id = @OrderId";

                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Order canceled successfully!");
                    LoadAssignedOrders();
                }
                else
                {
                    MessageBox.Show("Delivered orders cannot be canceled.");
                }
            }
            else
            {
                MessageBox.Show("Please select an order to cancel.");
            }
        }
    }
}

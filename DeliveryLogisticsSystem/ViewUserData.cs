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

namespace DeliveryLogisticsSystem
{
    public partial class ViewUserData : Form
    {

        private string userId;

        public ViewUserData(string userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadUserData();
        }

        private void LoadUserData()
        {
            // Create a new database connection and retrieve user data based on the provided userId.
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT email, fullname, phonenumber, address, profile_image FROM users WHERE user_id = @userId";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Populate the form fields with the user's data from the database.
                            txtEmail.Text = reader["email"].ToString();
                            txtFullName.Text = reader["fullname"].ToString();
                            txtAddress.Text = reader["address"].ToString();
                            txtPhone.Text = reader["phonenumber"].ToString();

                            // If a profile image exists, convert the byte array to an image and display it.
                            if (reader["profile_image"] != DBNull.Value)
                            {
                                byte[] imageBytes = (byte[])reader["profile_image"];
                                using (var ms = new System.IO.MemoryStream(imageBytes))
                                {
                                    UserProfileImage.Image = System.Drawing.Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                UserProfileImage.Image = null; // or set a default image
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    // Show an error message if there is a problem loading user data.
                    MessageBox.Show("Error loading user data: " + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

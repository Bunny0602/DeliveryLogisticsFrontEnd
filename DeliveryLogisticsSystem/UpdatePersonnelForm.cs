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
    public partial class UpdatePersonnelForm : Form
    {

        private string personnelId;
        private byte[] newProfileImage;

        public UpdatePersonnelForm(string personnelId)
        {
            InitializeComponent();
            this.personnelId = personnelId;
            LoadPersonnelData();
        }

        /// <summary>
        /// Loads the personnel data from the database using the provided personnel ID
        /// and populates the form fields with the retrieved information.
        /// </summary>
        private void LoadPersonnelData()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT email, fullname, phonenumber, password, role, status, address, profile_image
                                    FROM personnel
                                    WHERE personnel_id = @PersonnelID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelID", personnelId);


                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inputEmail.Text = reader["email"].ToString();
                            inputFullName.Text = reader["fullname"].ToString();
                            inputPhone.Text = reader["phonenumber"].ToString();
                            inputPassword.Text = reader["password"].ToString();
                            inputAddress.Text = reader["address"].ToString();

                            if (reader["profile_image"] != DBNull.Value)
                            {
                                byte[] imgBytes = (byte[])reader["profile_image"];
                                using (var ms = new System.IO.MemoryStream(imgBytes))
                                {
                                    ProfileContainer.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                ProfileContainer.Image = null;
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error loading personnel data: " + ex.Message);
                }
            }
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png",
                Title = "Select Profile Image"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                ProfileContainer.Image = Image.FromFile(filePath);
                newProfileImage = System.IO.File.ReadAllBytes(filePath);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            string fullName = inputFullName.Text.Trim();
            string address = inputAddress.Text.Trim();
            string phoneNumber = inputPhone.Text.Trim();
            string password = inputPassword.Text.Trim();

            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"UPDATE personnel
                                    SET fullname = @FullName, phonenumber = @Phone, password = @Password, address = @Address, profile_image = @ProfileImage
                                    WHERE personnel_id = @PersonnelID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Phone", phoneNumber);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@PersonnelID", personnelId);

                    if (newProfileImage != null)
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", newProfileImage);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Personnel updated successfully.");
                    this.Close(); // Close the form after updating
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error updating personnel: " + ex.Message);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHome_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            this.Close();
        }
    }
}

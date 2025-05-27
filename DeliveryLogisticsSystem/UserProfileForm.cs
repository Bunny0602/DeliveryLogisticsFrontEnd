using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeliveryLogisticsSystem
{
    public partial class UserProfileForm : Form
    {

        private string userId;
        private byte[] profileImageData;

        public UserProfileForm(string userId)
        {
            InitializeComponent();
            this.userId = userId;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT email, fullname, phonenumber, address, profile_image FROM users WHERE user_id = @UserID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtEmail.Text = reader["email"].ToString();
                            txtFullName.Text = reader["fullname"].ToString();
                            txtPhone.Text = reader["phonenumber"].ToString();
                            txtAddress.Text = reader["address"].ToString();

                            if (reader["profile_image"] != DBNull.Value)
                            {
                                byte[] imgBytes = (byte[])reader["profile_image"];
                                using (MemoryStream ms = new MemoryStream(imgBytes))
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
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading profile data: " + ex.Message);
                }
            }
        }

        private void btnEditProfile_Click(object sender, EventArgs e)
        {
            // Enable editing
            txtFullName.ReadOnly = false;
            txtPhone.ReadOnly = false;
            txtAddress.ReadOnly = false;

            // Show Update button
            btnUpdateProfile.Visible = true;

            // Disable Edit button
            btnEditProfile.Enabled = false;
        }

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files (*.jpg; *.jpeg; *.png)|*.jpg; *.jpeg; *.png";
            openFileDialog.Title = "Select Profile Image";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                try
                {
                    // Display the selected image
                    ProfileContainer.Image = new Bitmap(filePath);

                    // Convert image to byte array for database storage
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ProfileContainer.Image.Save(ms, ProfileContainer.Image.RawFormat);
                        profileImageData = ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
        }

        private void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"UPDATE users SET fullname = @FullName, phonenumber = @Phone, address = @Address, profile_image = @ProfileImage WHERE user_id = @UserID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@FullName", txtFullName.Text);
                    cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                    cmd.Parameters.AddWithValue("@Address", txtAddress.Text);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    if (profileImageData != null)
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", profileImageData);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();

                    MessageBox.Show("Profile updated successfully.");
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error updating profile: " + ex.Message);
                }
                finally
                {
                    // Disable editing
                    txtFullName.ReadOnly = true;
                    txtPhone.ReadOnly = true;
                    txtAddress.ReadOnly = true;
                    // Hide Update button
                    btnUpdateProfile.Visible = false;
                    // Enable Edit button
                    btnEditProfile.Enabled = true;
                }
            }
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

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {
            CreateOrderForm createOrderForm = new CreateOrderForm(userId, txtEmail.Text, "User", 0);
            createOrderForm.Show();
        }
    }
}

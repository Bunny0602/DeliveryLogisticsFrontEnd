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
    public partial class AddPersonnelForm : Form
    {

        private byte[] profileImageData;

        public AddPersonnelForm()
        {
            InitializeComponent();
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
                profileImageData = System.IO.File.ReadAllBytes(filePath);
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Handles the Add button click event. 
        /// Validates input fields, checks for duplicate email, and inserts new personnel data (including optional profile image) into the database.
        /// Shows appropriate messages for validation errors or success, and closes the form on successful addition.
        /// </summary>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            string email = inputEmail.Text.Trim();
            string fullName = inputFullName.Text.Trim();
            string address = inputAddress.Text.Trim();
            string phoneNumber = inputPhone.Text.Trim();
            string password = inputPass.Text.Trim();
            string role = "driver"; // Default role for new personnel
            string status = "Available"; // Default status for new personnel

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName) ||
                string.IsNullOrEmpty(address) || string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(phoneNumber))
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (phoneNumber.Length != 11)
            {
                MessageBox.Show("Phone number must be 11 digits long.");
                return;
            }

            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string checkEmailQuery = "SELECT COUNT(*) FROM personnel WHERE email = @Email";
                    MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, conn);
                    checkEmailCmd.Parameters.AddWithValue("@Email", email);
                    int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                    if (emailCount > 0)
                    {
                        MessageBox.Show("Email already exists. Please use a different email.");
                        return;
                    }

                    string query = @"INSERT INTO personnel (email, fullname, address, phonenumber, password, role, status, profile_image)
                                    VALUES (@Email, @FullName, @Address, @PhoneNumber, @Password, @Role, @Status, @ProfileImage)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@FullName", fullName);
                    cmd.Parameters.AddWithValue("@Password", password);
                    cmd.Parameters.AddWithValue("@Address", address);
                    cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                    cmd.Parameters.AddWithValue("@Role", role);
                    cmd.Parameters.AddWithValue("@Status", status);

                    if (profileImageData != null)
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", profileImageData);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ProfileImage", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Personnel added successfully.");
                    this.Close();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Error adding personnel: " + ex.Message);
                }
                finally
                {
                    if (conn.State == ConnectionState.Open)
                    {
                        conn.Close();
                    }
                }
            }
        }
    }
}
// This form allows the user to add new personnel to the system, including uploading a profile image and validating input fields.

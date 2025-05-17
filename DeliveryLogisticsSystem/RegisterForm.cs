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
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        // Handles the registration logic, including validation and database operations
        private void btnRegister_Click(object sender, EventArgs e)
        {
            string email = inputEmail.Text.Trim();
            string password = inputPass.Text.Trim();
            string phoneNumber = inputPhone.Text.Trim();

            if (email == "" || password == "" || phoneNumber == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, @"^09\d{9}$"))
            {
                MessageBox.Show("Phone number must be in the format 09xxxxxxxxx and exactly 11 digits long.");
                return;
            }

            DataBase db = new DataBase();
            MySqlConnection conn = db.GetConnection();

            try
            {
                conn.Open();

                // Check for duplicate email in both users and personnel tables
                string checkEmailQuery = "SELECT COUNT(*) FROM users WHERE email = @Email " +
                                          "UNION " +
                                          "SELECT COUNT(*) FROM personnel WHERE email = @Email";
                MySqlCommand checkEmailCmd = new MySqlCommand(checkEmailQuery, conn);
                checkEmailCmd.Parameters.AddWithValue("@Email", email);
                int emailCount = Convert.ToInt32(checkEmailCmd.ExecuteScalar());

                if (emailCount > 0)
                {
                    MessageBox.Show("Email already exists. Please use a different one.");
                    return;
                }

                // Check for duplicate phone number in users table
                string checkPhoneQuery = "SELECT COUNT(*) FROM users WHERE phonenumber = @PhoneNumber";
                MySqlCommand checkCmd = new MySqlCommand(checkPhoneQuery, conn);
                checkCmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);
                int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                if (count > 0)
                {
                    MessageBox.Show("Phone number already exists. Please use a different one.");
                    return;
                }

                // Insert new user into users table
                string query = "INSERT INTO users (email, password, phonenumber, role) VALUES (@Email, @Password, @PhoneNumber, 'user')";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);
                cmd.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                cmd.ExecuteNonQuery();

                MessageBox.Show("Registration successful! You can now log in.");

                LoginForm loginForm = new LoginForm();
                loginForm.Show();
                this.Hide();

                // Clear fields
                inputEmail.Clear();
                inputPass.Clear();
                inputPhone.Clear();
                showPassword.Checked = false;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                conn.Close();
            }
        }

        private void showPassword_CheckedChanged(object sender, EventArgs e)
        {
            inputPass.PasswordChar = showPassword.Checked ? '\0' : '*';
        }

        private void linkLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();
            this.Hide();
        }
    }
}

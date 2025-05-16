using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DeliveryLogisticsSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string email = inputEmail.Text.Trim();
            string password = inputPass.Text.Trim();

            if (email == "" || password == "")
            {
                MessageBox.Show("Please fill in all fields.");
                return;
            }

            DataBase db = new DataBase();
            MySqlConnection conn = db.GetConnection();

            try
            {
                conn.Open();

                string query = @"SELECT user_id, role FROM users WHERE email = @Email AND password = @Password
                                UNION
                                SELECT personnel_id AS user_id, role FROM personnel WHERE email = @Email AND password = @Password";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", password);

                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string userId = reader["user_id"].ToString();
                        string role = reader["role"].ToString();
                        switch (role)
                        {
                            case "admin":
                                MessageBox.Show("Welcome Admin!");
                                AdminDashboard admindashboard = new AdminDashboard(userId);
                                admindashboard.Show();
                                this.Hide();
                                break;
                            case "driver":
                                MessageBox.Show("Welcome Driver!");
                                PersonnelDashboardForm persooneldashboard = new PersonnelDashboardForm(userId);
                                persooneldashboard.Show();
                                this.Hide();
                                break;
                            case "user":
                                MessageBox.Show("Welcome User!");
                                UserDashboardForm userdashboard = new UserDashboardForm(userId);
                                userdashboard.Show();
                                this.Hide();
                                break;
                            default:
                                MessageBox.Show("Invalid role. Please contact support.");
                                break;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Invalid email or password. Please try again.");
                    }
                }
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

        private void linkRegister_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm registerForm = new RegisterForm();
            registerForm.Show();
            this.Hide();
        }
    }
}

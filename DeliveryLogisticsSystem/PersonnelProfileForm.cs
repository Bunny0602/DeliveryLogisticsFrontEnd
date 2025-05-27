using MySql.Data.MySqlClient;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DeliveryLogisticsSystem
{
    public partial class PersonnelProfileForm : Form
    {
        private string personnelId;
        private byte[] profileImageData;

        public PersonnelProfileForm(string personnelId)
        {
            InitializeComponent();
            this.personnelId = personnelId;
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
                    string query = "SELECT email, fullname, phonenumber, address, profile_image, role, status FROM personnel WHERE personnel_id = @PersonnelID";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelID", personnelId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtEmail.Text = reader["email"].ToString();
                            txtFullName.Text = reader["fullname"].ToString();
                            txtPhone.Text = reader["phonenumber"].ToString();
                            txtAddress.Text = reader["address"].ToString();
                            txtRole.Text = reader["role"].ToString();
                            txtStatus.Text = reader["status"].ToString();

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

        private void btnUploadImage_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files (*.jpg;*.png)|*.jpg;*.png";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    ProfileContainer.Image = new Bitmap(dialog.FileName);
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ProfileContainer.Image.Save(ms, ProfileContainer.Image.RawFormat);
                        profileImageData = ms.ToArray();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error uploading image: " + ex.Message);
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

        private void btnHome_Click_1(object sender, EventArgs e)
        {
            PersonnelDashboardForm personnelDashboardForm = new PersonnelDashboardForm(personnelId, txtEmail.Text, txtRole.Text);
            personnelDashboardForm.Show();

            this.Close();
        }

        private void btnLogout_Click_1(object sender, EventArgs e)
        {
            LoginForm loginForm = new LoginForm();
            loginForm.Show();

            this.Close();
        }
    }
}

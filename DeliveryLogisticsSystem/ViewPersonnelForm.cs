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
    public partial class ViewPersonnelForm : Form
    {

        private string personnelId;

        public ViewPersonnelForm(string personnelId)
        {
            InitializeComponent();
            this.personnelId = personnelId;
            LoadPersonnelData();
        }

        private void LoadPersonnelData()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();

                    string query = @"SELECT email, fullname, phonenumber, role, status, address, profile_image
                                    FROM personnel
                                    WHERE personnel_id = @PersonnelID";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@PersonnelID", personnelId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            txtEmail.Text = reader["email"].ToString();
                            txtFullName.Text = reader["fullname"].ToString();
                            txtPhone.Text = reader["phonenumber"].ToString();
                            txtRole.Text = reader["role"].ToString();
                            txtStatus.Text = reader["status"].ToString();
                            txtAddress.Text = reader["address"].ToString();

                            if (reader["profile_image"] != DBNull.Value)
                            {
                                byte[] imgBytes = (byte[])reader["profile_image"];
                                using (var ms = new System.IO.MemoryStream(imgBytes))
                                {
                                    PersonnelProfileImage.Image = Image.FromStream(ms);
                                }
                            }
                            else
                            {
                                PersonnelProfileImage.Image = null; // Set to a default image or null if no image is available
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



        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

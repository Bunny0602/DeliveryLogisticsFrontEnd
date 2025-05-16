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
    public partial class UserDashboardForm : Form
    {

        private string userId;

        public UserDashboardForm(string userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void btnCreateOrder_Click(object sender, EventArgs e)
        {

        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            UserProfileForm userProfileForm = new UserProfileForm(userId);
            userProfileForm.Show();
        }
    }
}

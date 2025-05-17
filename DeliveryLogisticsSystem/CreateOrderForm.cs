using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DeliveryLogisticsSystem
{
    public partial class CreateOrderForm : Form
    {
        private string userId;
        private string userEmail;
        private string userRole;
        private string pickupCoordinates = "";
        private string dropoffCoordinates = "";

        public CreateOrderForm(string userId, string userEmail, string userRole)
        {
            InitializeComponent();
            this.userId = userId;
            this.userEmail = userEmail;
            this.userRole = userRole;

            txtEmail.Text = userEmail;
            txtEmail.ReadOnly = true;

            txtPickUpAddress.Leave += LocationTextBox_Leave;
            txtDropOffAddress.Leave += LocationTextBox_Leave;

            _ = InitializeWebViewAsync();
        }

        private async Task InitializeWebViewAsync()
        {
            await WBBMap.EnsureCoreWebView2Async(null);
            WBBMap.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WBBMap.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
            LoadMap();
        }

        private void CoreWebView2_WebMessageReceived(object sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message = e.TryGetWebMessageAsString();

            if (double.TryParse(message, out double distanceMeters))
            {
                double distanceKm = distanceMeters / 1000.0;
                double baseFare = 50; // Base fare in PHP
                double perKmRate = 10; // Per kilometer rate in PHP

                double price = baseFare + (perKmRate * distanceKm);

                this.Invoke(new Action(() =>
                {
                    txtPrice.Text = $"₱{price:F2}";
                }));
            }
        }


        private void LoadMap()
        {
            string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <title>Map</title>
    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
</head>
<body>
    <div id='map' style='width: 440px; height: 570px;'></div>
    <script>
        var map = L.map('map').setView([12.8797, 121.7740], 6);
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            maxZoom: 18
        }).addTo(map);
    </script>
</body>
</html>";
            WBBMap.CoreWebView2.NavigateToString(htmlContent);
        }

        private async void LocationTextBox_Leave(object sender, EventArgs e)
        {
            string pickupLocation = txtPickUpAddress.Text.Trim();
            string dropoffLocation = txtDropOffAddress.Text.Trim();

            if (!string.IsNullOrEmpty(pickupLocation))
            {
                var pickupCoords = await GeocodeAddressAsync(pickupLocation);
                if (pickupCoords.HasValue)
                {
                    pickupCoordinates = $"{pickupCoords.Value.lat},{pickupCoords.Value.lon}";
                }
            }

            if (!string.IsNullOrEmpty(dropoffLocation))
            {
                var dropoffCoords = await GeocodeAddressAsync(dropoffLocation);
                if (dropoffCoords.HasValue)
                {
                    dropoffCoordinates = $"{dropoffCoords.Value.lat},{dropoffCoords.Value.lon}";
                }
            }
        }

        private async Task<(double lat, double lon)?> GeocodeAddressAsync(string address)
        {
            try
            {
                string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    string responseData = await response.Content.ReadAsStringAsync();
                    JArray json = JArray.Parse(responseData);

                    if (json.Count > 0)
                    {
                        return (double.Parse(json[0]["lat"].ToString()), double.Parse(json[0]["lon"].ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geocoding error: {ex.Message}");
            }

            return null;
        }

        private void btnSubmitOrder_Click(object sender, EventArgs e)
        {
            string pickup = txtPickUpAddress.Text.Trim();
            string dropName = txtDropOffName.Text.Trim();
            string dropoff = txtDropOffAddress.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string priceText = txtPrice.Text.Replace("₱", "").Trim();

            if (string.IsNullOrEmpty(pickup) || string.IsNullOrEmpty(dropoff) || string.IsNullOrEmpty(dropName) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(priceText))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!decimal.TryParse(priceText, out decimal price))
            {
                MessageBox.Show("Invalid price format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = @"
                    INSERT INTO orders (user_id, email, name, pickup_location, dropoff_name, dropoff_location, phonenumber, price) 
                    VALUES (@userId, @userEmail, @dropName, @pickup, @dropName, @dropoff, @phone, @price)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@userEmail", userEmail);
                cmd.Parameters.AddWithValue("@dropName", dropName);
                cmd.Parameters.AddWithValue("@pickup", pickup);
                cmd.Parameters.AddWithValue("@dropoff", dropoff);
                cmd.Parameters.AddWithValue("@phone", phone);
                cmd.Parameters.AddWithValue("@price", price);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Order created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ClearFields();
            }
        }

        private void ClearFields()
        {
            txtPickUpAddress.Clear();
            txtDropOffName.Clear();
            txtDropOffAddress.Clear();
            txtPhone.Clear();
            txtPrice.Clear();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Hide();

            if (userRole == "Admin")
            {
                AdminDashboard adminDashboard = new AdminDashboard(userId, userEmail, userRole);
                adminDashboard.Show();
            }
            else if (userRole == "User")
            {
                UserDashboardForm userDashboard = new UserDashboardForm(userId, userEmail, userRole);
                userDashboard.Show();
            }
            else if (userRole == "Personnel")
            {
                PersonnelDashboardForm personnelDashboard = new PersonnelDashboardForm(userId, userEmail, userRole);
                personnelDashboard.Show();
            }
            else
            {
                MessageBox.Show("Unknown user role. Cannot navigate back.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }
    }
}

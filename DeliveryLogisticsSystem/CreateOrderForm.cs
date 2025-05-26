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
                double baseFare = 50;
                double perKmRate = 10; 

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
                                        <meta charset='utf-8' />
                                        <title>Route Map</title>
                                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                        <link rel='stylesheet' href='https://unpkg.com/leaflet@1.7.1/dist/leaflet.css' />
                                        <script src='https://unpkg.com/leaflet@1.7.1/dist/leaflet.js'></script>
                                    </head>
                                    <body>
                                        <div id='map' style='width: 440px; height: 570px;'></div>
                                    <script>
                                        var map = L.map('map').setView([12.8797, 121.7740], 6);
                                        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
                                            maxZoom: 18,
                                        }).addTo(map);

                                        var pickupMarker = null;
                                        var dropoffMarker = null;
                                        var routeLine = null;

                                        // Uniform marker styles
                                        const redIcon = L.icon({
                                            iconUrl: 'https://maps.google.com/mapfiles/ms/icons/red-dot.png',
                                            iconSize: [32, 32],
                                            iconAnchor: [16, 32],
                                            popupAnchor: [0, -32]
                                        });

                                        const blueIcon = L.icon({
                                            iconUrl: 'https://maps.google.com/mapfiles/ms/icons/blue-dot.png',
                                            iconSize: [32, 32],
                                            iconAnchor: [16, 32],
                                            popupAnchor: [0, -32]
                                        });

                                        window.chrome.webview.addEventListener('message', event => {
                                            const data = event.data;

                                            if (data.startsWith('pickup:')) {
                                                const [lat, lon] = data.replace('pickup:', '').split(',').map(Number);
                                                if (pickupMarker) {
                                                    pickupMarker.setLatLng([lat, lon]);
                                                } else {
                                                    pickupMarker = L.marker([lat, lon], { icon: redIcon }).addTo(map).bindPopup('Pickup').openPopup();
                                                }
                                                map.setView([lat, lon], 13);
                                            }

                                            if (data.startsWith('dropoff:')) {
                                                const [lat, lon] = data.replace('dropoff:', '').split(',').map(Number);
                                                if (dropoffMarker) {
                                                    dropoffMarker.setLatLng([lat, lon]);
                                                } else {
                                                    dropoffMarker = L.marker([lat, lon], { icon: blueIcon }).addTo(map).bindPopup('Drop-off').openPopup();
                                                }
                                                map.setView([lat, lon], 13);
                                            }

                                            if (data.startsWith('route:')) {
                                                const coords = data.replace('route:', '').split(';');
                                                const [pickupLat, pickupLon] = coords[0].split(',').map(Number);
                                                const [dropoffLat, dropoffLon] = coords[1].split(',').map(Number);

                                                fetch(`https://router.project-osrm.org/route/v1/driving/${pickupLon},${pickupLat};${dropoffLon},${dropoffLat}?overview=full&geometries=geojson`)
                                                    .then(response => response.json())
                                                    .then(data => {
                                                        const routeCoords = data.routes[0].geometry.coordinates.map(coord => [coord[1], coord[0]]);
                                                        if (routeLine) map.removeLayer(routeLine);

                                                        routeLine = L.polyline(routeCoords, { color: 'red', weight: 4 }).addTo(map);
                                                        map.fitBounds(routeLine.getBounds());
                                                    })
                                                    .catch(err => console.error('Routing error:', err));
                                            }
                                        });
                                    </script>
                                    </body>
                                    </html>";
            WBBMap.CoreWebView2.NavigateToString(htmlContent);
        }

        private async void LocationTextBox_Leave(object sender, EventArgs e)
        {
            string pickupLocation = txtPickUpAddress.Text.Trim();
            string dropoffLocation = txtDropOffAddress.Text.Trim();

            bool updated = false;

            if (!string.IsNullOrEmpty(pickupLocation))
            {
                var pickupCoords = await GeocodeAddressAsync(pickupLocation);
                if (pickupCoords.HasValue)
                {
                    pickupCoordinates = $"{pickupCoords.Value.lat},{pickupCoords.Value.lon}";
                    WBBMap.CoreWebView2.PostWebMessageAsString($"pickup:{pickupCoords.Value.lat},{pickupCoords.Value.lon}");
                    updated = true;
                }
            }

            if (!string.IsNullOrEmpty(dropoffLocation))
            {
                var dropoffCoords = await GeocodeAddressAsync(dropoffLocation);
                if (dropoffCoords.HasValue)
                {
                    dropoffCoordinates = $"{dropoffCoords.Value.lat},{dropoffCoords.Value.lon}";
                    WBBMap.CoreWebView2.PostWebMessageAsString($"dropoff:{dropoffCoords.Value.lat},{dropoffCoords.Value.lon}");
                    updated = true;
                }
            }

            if (!string.IsNullOrEmpty(pickupCoordinates) && !string.IsNullOrEmpty(dropoffCoordinates) && updated)
            {
                WBBMap.CoreWebView2.PostWebMessageAsString($"route:{pickupCoordinates};{dropoffCoordinates}");
            }
        }

        private async Task<(double lat, double lon)?> GeocodeAddressAsync(string address)
        {
            try
            {
                string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(address)}&format=json&limit=1";
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("DeliveryLogisticsSystem/1.0");

                    var response = await client.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show($"Geocoding failed with status code: {response.StatusCode}", "Geocoding Error");
                        return null;
                    }

                    string responseData = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(responseData))
                    {
                        MessageBox.Show("Geocoding returned an empty response.", "Geocoding Error");
                        return null;
                    }

                    try
                    {
                        JArray json = JArray.Parse(responseData);

                        if (json.Count > 0)
                        {
                            double lat = double.Parse(json[0]["lat"].ToString());
                            double lon = double.Parse(json[0]["lon"].ToString());
                            return (lat, lon);
                        }
                        else
                        {
                            MessageBox.Show("Address not found during geocoding.", "Geocoding Warning");
                            return null;
                        }
                    }
                    catch (Exception parseEx)
                    {
                        MessageBox.Show($"Geocoding JSON parse error: {parseEx.Message}", "Geocoding Error");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Geocoding error: {ex.Message}", "Geocoding Exception");
                return null;
            }
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
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                conn.Open();
                string query = "SELECT role FROM users WHERE user_id = @UserId";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@UserId", userId);
                string role = cmd.ExecuteScalar()?.ToString()?.Trim().ToLower();
                conn.Close();

                if (role == "user")
                {
                    UserDashboardForm userDashboard = new UserDashboardForm(userId, userEmail, role);
                    userDashboard.Show();
                    this.Close();
                }
                else if (role == "admin")
                {
                    AdminDashboard adminDashboard = new AdminDashboard(userId, userEmail, role);
                    adminDashboard.Show();
                    this.Close();
                }
                else if (role == "personnel")
                {
                    PersonnelDashboardForm personnelDashboard = new PersonnelDashboardForm(userId, userEmail, role);
                    personnelDashboard.Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Unknown user role. Please contact support.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
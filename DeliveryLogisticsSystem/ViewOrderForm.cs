using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace DeliveryLogisticsSystem
{
    public partial class ViewOrderForm : Form
    {
        public ViewOrderForm(string email, string pickup, string dropoffName, string dropoff, string personnel, string phone, string status)
        {
            InitializeComponent();

            txtEmail.Text = email;
            txtPickUPLocation.Text = pickup;
            txtDrpOffname.Text = dropoffName;
            txtDropOffLocation.Text = dropoff;
            txtPersonnel.Text = personnel;
            txtPhone.Text = phone;
            txtStatus.Text = status;

            _ = InitializeWebViewAsync(pickup, dropoff);
        }

        private async Task InitializeWebViewAsync(string pickup, string dropoff)
        {
            await ViewOrderMap.EnsureCoreWebView2Async();
            var pickupCoordinates = await GetCoordinates(pickup);
            var dropoffCoordinates = await GetCoordinates(dropoff);

            if (pickupCoordinates != null && dropoffCoordinates != null)
            {
                string htmlContent = GenerateMapHtml(pickupCoordinates.Value.lat, pickupCoordinates.Value.lon, dropoffCoordinates.Value.lat, dropoffCoordinates.Value.lon);
                ViewOrderMap.CoreWebView2.NavigateToString(htmlContent);
            }
            else
            {
                MessageBox.Show("Unable to get coordinates. Please verify addresses.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task<(double lat, double lon)?> GetCoordinates(string location)
        {
            string url = $"https://nominatim.openstreetmap.org/search?q={Uri.EscapeDataString(location)}&format=json&limit=1";

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", "DeliveryLogisticsApp/1.0 (contact@example.com)");

                try
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseData = await response.Content.ReadAsStringAsync();
                        JArray json = JArray.Parse(responseData);

                        if (json.Count > 0)
                        {
                            double lat = double.Parse(json[0]["lat"].ToString());
                            double lon = double.Parse(json[0]["lon"].ToString());
                            return (lat, lon);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            return null;
        }

        private string GenerateMapHtml(double pickupLat, double pickupLon, double dropoffLat, double dropoffLon)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <title>Order Map</title>
                    <meta charset='utf-8' />
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
                    <link rel='stylesheet' href='https://unpkg.com/leaflet-routing-machine/dist/leaflet-routing-machine.css' />
                    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
                    <script src='https://unpkg.com/leaflet-routing-machine/dist/leaflet-routing-machine.js'></script>
                </head>
                <body>
                    <div id='map' style='width: 100%; height: 500px;'></div>

                    <script>
                        var map = L.map('map').setView([{pickupLat}, {pickupLon}], 13);

                        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png', {{
                            maxZoom: 18,
                            attribution: '© OpenStreetMap contributors'
                        }}).addTo(map);

                        var pickupMarker = L.marker([{pickupLat}, {pickupLon}]).addTo(map).bindPopup('Pickup Location').openPopup();
                        var dropoffMarker = L.marker([{dropoffLat}, {dropoffLon}]).addTo(map).bindPopup('Dropoff Location').openPopup();

                        L.Routing.control({{
                            waypoints: [
                                L.latLng({pickupLat}, {pickupLon}),
                                L.latLng({dropoffLat}, {dropoffLon})
                            ],
                            lineOptions: {{
                                styles: [{{
                                    color: 'blue',
                                    weight: 4
                                }}]
                            }},
                            createMarker: function() {{ return null; }},
                            routeWhileDragging: false,
                            show: false
                        }}).addTo(map);
                    </script>
                </body>
                </html>";
        }
    }
}

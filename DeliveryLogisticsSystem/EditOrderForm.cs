using Microsoft.Web.WebView2.Core;
using MySql.Data.MySqlClient;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace DeliveryLogisticsSystem
{
    public partial class EditOrderForm : Form
    {
        private string userId;
        private int orderId;

        private string pickupCoordinates = "";
        private string dropoffCoordinates = "";

        public EditOrderForm(string userId, int orderId)
        {
            InitializeComponent();
            this.userId = userId;
            this.orderId = orderId;
            this.Load += EditOrderForm_Load;

            TxtPickUp.Leave += LocationTextBox_Leave;
            TxtDropOff.Leave += LocationTextBox_Leave;
        }

        private async void EditOrderForm_Load(object sender, EventArgs e)
        {
            await WBBMap.EnsureCoreWebView2Async(null);
            WBBMap.CoreWebView2.Settings.IsWebMessageEnabled = true;
            WBBMap.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;

            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM orders WHERE order_id = @OrderId AND user_id = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                txtEmail.Text = reader["email"].ToString();
                                TxtPickUp.Text = reader["pickup_location"].ToString();
                                TxtDropName.Text = reader["dropoff_name"].ToString();
                                TxtDropOff.Text = reader["dropoff_location"].ToString();
                                txtPhone.Text = reader["phonenumber"].ToString();

                                string pickup = TxtPickUp.Text;
                                string dropoff = TxtDropOff.Text;

                                string htmlContent = GenerateMapHtml(pickup, dropoff);
                                WBBMap.NavigateToString(htmlContent);
                            }
                            else
                            {
                                MessageBox.Show("Order not found or you do not have permission to edit this order.");
                                this.Close();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void CoreWebView2_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            // You can handle messages from JS here if needed
        }

        private async void LocationTextBox_Leave(object sender, EventArgs e)
        {
            string pickupLocation = TxtPickUp.Text.Trim();
            string dropoffLocation = TxtDropOff.Text.Trim();
            bool updated = false;

            if (!string.IsNullOrEmpty(pickupLocation))
            {
                var pickupCoords = await GeocodeAddressAsync(pickupLocation);
                if (pickupCoords.HasValue)
                {
                    pickupCoordinates = $"{pickupCoords.Value.lat},{pickupCoords.Value.lon}";
                    WBBMap.CoreWebView2.PostWebMessageAsString($"pickup:{pickupCoordinates}");
                    updated = true;
                }
            }

            if (!string.IsNullOrEmpty(dropoffLocation))
            {
                var dropoffCoords = await GeocodeAddressAsync(dropoffLocation);
                if (dropoffCoords.HasValue)
                {
                    dropoffCoordinates = $"{dropoffCoords.Value.lat},{dropoffCoords.Value.lon}";
                    WBBMap.CoreWebView2.PostWebMessageAsString($"dropoff:{dropoffCoordinates}");
                    updated = true;
                }
            }

            if (!string.IsNullOrEmpty(pickupCoordinates) &&
                !string.IsNullOrEmpty(dropoffCoordinates) && updated)
            {
                WBBMap.CoreWebView2.PostWebMessageAsString($"route:{pickupCoordinates};{dropoffCoordinates}");
            }
        }

        private async Task<(double lat, double lon)?> GeocodeAddressAsync(string address)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"https://nominatim.openstreetmap.org/search?format=json&q={Uri.EscapeDataString(address)}";
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var results = JArray.Parse(json);

                        if (results.Count > 0)
                        {
                            var item = results[0];
                            double lat = double.Parse(item["lat"].ToString());
                            double lon = double.Parse(item["lon"].ToString());
                            return (lat, lon);
                        }
                    }
                }
            }
            catch { }

            return null;
        }

        private void btnUpdateOrder_Click(object sender, EventArgs e)
        {
            DataBase db = new DataBase();
            using (MySqlConnection conn = db.GetConnection())
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE orders SET pickup_location = @PickupAddress, dropoff_name = @DropoffName, dropoff_location = @DropoffAddress, phonenumber = @Phone WHERE order_id = @OrderId AND user_id = @UserId";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PickupAddress", TxtPickUp.Text);
                        cmd.Parameters.AddWithValue("@DropoffName", TxtDropName.Text);
                        cmd.Parameters.AddWithValue("@DropoffAddress", TxtDropOff.Text);
                        cmd.Parameters.AddWithValue("@Phone", txtPhone.Text);
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@UserId", userId);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Order updated successfully.");
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Failed to update order. Please check your input and try again.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void btnback_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string GenerateMapHtml(string pickup, string dropoff)
        {
            return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='utf-8'>
    <title>Edit Order Map</title>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <link rel='stylesheet' href='https://unpkg.com/leaflet/dist/leaflet.css' />
    <link rel='stylesheet' href='https://unpkg.com/leaflet-routing-machine/dist/leaflet-routing-machine.css' />
    <style>
        html, body, #map {{
            height: 100%;
            margin: 0;
            padding: 0;
        }}
    </style>
</head>
<body>
    <div id='map'></div>
    <script src='https://unpkg.com/leaflet/dist/leaflet.js'></script>
    <script src='https://unpkg.com/leaflet-routing-machine/dist/leaflet-routing-machine.min.js'></script>
    <script>
        const map = L.map('map').setView([13.41, 122.56], 7);
        L.tileLayer('https://{{s}}.tile.openstreetmap.org/{{z}}/{{x}}/{{y}}.png').addTo(map);

        let routeControl = null;
        let pickupMarker = null;
        let dropoffMarker = null;

        async function geocode(location) {{
            const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${{location}}`);
            const data = await response.json();
            if (data.length > 0) {{
                return [parseFloat(data[0].lat), parseFloat(data[0].lon)];
            }}
            return null;
        }}

        async function showRoute(pickup, dropoff) {{
            if (routeControl) map.removeControl(routeControl);
            routeControl = L.Routing.control({{
                waypoints: [L.latLng(pickup[0], pickup[1]), L.latLng(dropoff[0], dropoff[1])],
                routeWhileDragging: false
            }}).addTo(map);
        }}

        window.chrome.webview.addEventListener('message', event => {{
            const msg = event.data;
            if (msg.startsWith('pickup:')) {{
                const coords = msg.replace('pickup:', '').split(',');
                if (pickupMarker) map.removeLayer(pickupMarker);
                pickupMarker = L.marker([parseFloat(coords[0]), parseFloat(coords[1])]).addTo(map);
                map.panTo(pickupMarker.getLatLng());
            }} else if (msg.startsWith('dropoff:')) {{
                const coords = msg.replace('dropoff:', '').split(',');
                if (dropoffMarker) map.removeLayer(dropoffMarker);
                dropoffMarker = L.marker([parseFloat(coords[0]), parseFloat(coords[1])]).addTo(map);
                map.panTo(dropoffMarker.getLatLng());
            }} else if (msg.startsWith('route:')) {{
                const [pickupStr, dropoffStr] = msg.replace('route:', '').split(';');
                const pickup = pickupStr.split(',').map(parseFloat);
                const dropoff = dropoffStr.split(',').map(parseFloat);
                showRoute(pickup, dropoff);
            }}
        }});

        (async () => {{
            const pickup = await geocode('{pickup}');
            const dropoff = await geocode('{dropoff}');
            if (pickup && dropoff) {{
                showRoute(pickup, dropoff);
            }}
        }})();
    </script>
</body>
</html>";
        }
    }
}

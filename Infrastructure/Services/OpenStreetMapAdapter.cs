using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services
{
    public class OpenStreetMapAdapter : IGeolocationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<OpenStreetMapAdapter> _logger;

        public OpenStreetMapAdapter(HttpClient httpClient, ILogger<OpenStreetMapAdapter> logger)
        {
            _httpClient = httpClient;
            _logger = logger;

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add(
                    "User-Agent",
                    "IdentityManagerAPI/1.0 (basmalaelshabrawy4@gmail.com)"
                );
            }
        }

        public async Task<string?> GetAddressFromCoordinates(double latitude, double longitude)
        {
            string url = $"https://nominatim.openstreetmap.org/reverse?format=json&lat={latitude}&lon={longitude}";

            try
            {
                _logger.LogInformation("Requesting address for Lat={Lat}, Lon={Lon}", latitude, longitude);

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                // قراءة المحتوى مع UTF-8
                var content = await response.Content.ReadAsStringAsync();

                var json = JsonDocument.Parse(content);

                if (json.RootElement.TryGetProperty("display_name", out var displayName))
                {
                    var address = displayName.GetString();
                    _logger.LogInformation("Address found: {Address}", address ?? "null");
                    return address;
                }
                else
                {
                    _logger.LogWarning("display_name not found in response for Lat={Lat}, Lon={Lon}", latitude, longitude);
                    return "Address not found";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting address for Lat={Lat}, Lon={Lon}", latitude, longitude);
                return "Address not found";
            }
        }

        public double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            double r = 6371;
            double dLat = ToRadians(lat2 - lat1);
            double dLon = ToRadians(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return r * c;
        }

        private double ToRadians(double degrees) => degrees * (Math.PI / 180);
    }
}

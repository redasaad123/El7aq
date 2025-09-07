

namespace Infrastructure.Services
{
    public interface IGeolocationService
    {
        Task<string?> GetAddressFromCoordinates(double latitude, double longitude);
        double CalculateDistance(double lat1, double lon1, double lat2, double lon2);
    }
}

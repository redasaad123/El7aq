using Web.Models;

namespace Web.Models.Trip
{
    public class TripDetailsViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public StationViewModel OriginStation { get; set; } = new StationViewModel();
        public StationViewModel DestinationStation { get; set; } = new StationViewModel();
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public DriverViewModel Driver { get; set; } = new DriverViewModel();
        public bool IsUserLoggedIn { get; set; }
        public int BookingsCount { get; set; }
        public bool IsUpcoming { get; set; }

        public bool HasAvailableSeats => AvailableSeats > 0;
        public string FormattedDepartureTime => DepartureTime.ToString("dd/MM/yyyy HH:mm");
        public string FormattedPrice => $"{Price:F2} Pound";
        public double SeatOccupancyPercentage => TotalSeats > 0 ? (double)BookedSeats / TotalSeats * 100 : 0;
    }
}

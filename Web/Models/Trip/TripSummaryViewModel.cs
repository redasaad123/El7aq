namespace Web.Models.Trip
{
    public class TripSummaryViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public string OriginStation { get; set; } = string.Empty;
        public string DestinationStation { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public string DriverPhone { get; set; } = string.Empty;
        public int AvailableSeats { get; set; }

        public string FormattedDepartureTime => DepartureTime.ToString("dd/MM/yyyy HH:mm");
        public string FormattedPrice => $"{Price:F2} Pound";
        public string Route => $"{OriginStation} → {DestinationStation}";
    }
}

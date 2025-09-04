namespace Web.Models.Trip
{
    public class TripResultViewModel
    {
        public int TripId { get; set; }
        public DateTime DepartureTime { get; set; }
        public string OriginStation { get; set; } = string.Empty;
        public string DestinationStation { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AvailableSeats { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public bool HasAvailableSeats => AvailableSeats > 0;

        public string FormattedDepartureTime => DepartureTime.ToString("dd/MM/yyyy HH:mm");
        public string FormattedPrice => $"{Price:F2} Pounds";
    }
}

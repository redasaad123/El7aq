namespace Web.Models.Trip
{
    public class TripDetailsViewModel
    {

        public string TripId { get; set; }
        public DateTime DepartureTime { get; set; }
        public StationViewModel OriginStation { get; set; } = new StationViewModel();
        public StationViewModel DestinationStation { get; set; } = new StationViewModel();
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public DriverViewModel Driver { get; set; }

        public bool HasAvailableSeats => AvailableSeats > 0;
        public string FormattedDepartureTime => DepartureTime.ToString("dd/MM/yyyy HH:mm");
        public string FormattedPrice => $"{Price:F2} Pound";
        public double SeatOccupancyPercentage => TotalSeats > 0 ? (double)BookedSeats / TotalSeats * 100 : 0;
    }
}

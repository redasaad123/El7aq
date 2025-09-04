namespace Web.Models.Trip
{
    public class TripSummaryViewModel
    {
        public DateTime DepartureTime { get; set; }
        public string OriginStation { get; set; }
        public string DestinationStation { get; set; }
        public decimal Price { get; set; }
        public string DriverName { get; set; }
        public string CarNumber { get; set; }
        public string DriverPhone { get; set; }

        public string FormattedDepartureTime => DepartureTime.ToString("dd/MM/yyyy HH:mm");
        public string FormattedPrice => $"{Price:F2} Pound";
        public string Route => $"{OriginStation} → {DestinationStation}";
    }
}

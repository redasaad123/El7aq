namespace El7aq.Domain.Entities
{
    public class Trip
    {
        public string Id { get; set; }
        public string RouteId { get; set; }
        public string DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }

        // Navigation
        public Route Route { get; set; }
        public DriverProfile Driver { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }


}

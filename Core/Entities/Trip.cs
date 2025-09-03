namespace El7aq.Domain.Entities
{
    public class Trip
    {
        public int Id { get; set; }
        public int RouteId { get; set; }
        public int DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }

        // Navigation
        public Route Route { get; set; }
        public DriverProfile Driver { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }


}

namespace El7aq.Domain.Entities
{
    public class Route
    {
        public int Id { get; set; }
        public int StartStationId { get; set; }
        public int EndStationId { get; set; }
        public decimal Price { get; set; }

        // Navigation
        public Station StartStation { get; set; }
        public Station EndStation { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }


}

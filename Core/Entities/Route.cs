namespace El7aq.Domain.Entities
{
    public class Route
    {
        public string Id { get; set; }
        public string StartStationId { get; set; }
        public string EndStationId { get; set; }
        public decimal Price { get; set; }

        // Navigation
        public Station StartStation { get; set; }
        public Station EndStation { get; set; }
        public ICollection<Trip> Trips { get; set; }

    }


}

namespace El7aq.Domain.Entities
{
    public class Station
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        // Navigation
        public ICollection<Route> RoutesFrom { get; set; }
        public ICollection<Route> RoutesTo { get; set; }
        public ICollection<StaffProfile> Staff { get; set; }

    }


}

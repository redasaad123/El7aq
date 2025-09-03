namespace El7aq.Domain.Entities
{
    public class Station
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        // Navigation
        public ICollection<Route> Routes { get; set; }
        public ICollection<StaffProfile> Staff { get; set; }
    }


}

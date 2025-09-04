namespace Web.Models.Trip
{
    public class StationViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string FullName => $"{Name} - {City}";
    }
}

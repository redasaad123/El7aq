using Core.Entities;

namespace El7aq.Domain.Entities
{
    public class DriverProfile
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string LicenseNumber { get; set; }
        public string CarNumber { get; set; }

        // Navigation
        public AppUsers appUsers  { get; set; }
        public ICollection<Trip> Trips { get; set; }
    }


}

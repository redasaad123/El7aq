using Core.Entities;

namespace El7aq.Domain.Entities
{
    public class StaffProfile
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string StationId { get; set; }

        // Navigation
        public AppUsers User { get; set; }
        public Station Station { get; set; }
    }


}

using Core.Entities;

namespace El7aq.Domain.Entities
{
    public class StaffProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int StationId { get; set; }

        // Navigation
        public AppUsers User { get; set; }
        public Station Station { get; set; }
    }


}

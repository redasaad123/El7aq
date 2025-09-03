using Core.Entities;

namespace El7aq.Domain.Entities
{
    public class PassengerProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        // Navigation
        public AppUsers User { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }


}

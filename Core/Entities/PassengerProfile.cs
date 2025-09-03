using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
{
    public class PassengerProfile
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        // Navigation
        public AppUsers? User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }


}

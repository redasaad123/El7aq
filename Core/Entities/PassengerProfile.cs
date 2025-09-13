using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class PassengerProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; }

        // Navigation
        public AppUsers? User { get; set; }
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }


}

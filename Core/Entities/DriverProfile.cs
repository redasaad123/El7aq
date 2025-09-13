using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DriverProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(appUsers))]
        public required string UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public required string LicenseNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public required string CarNumber { get; set; }

        // Navigation
        public AppUsers? appUsers  { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public ICollection<TripDriverQueue>? TripQueues { get; set; } = new List<TripDriverQueue>();
        public double Lat { get; set; }
        public double Long { get; set; }
    }


}

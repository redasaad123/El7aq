using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class TripDriverQueue
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(Trip))]
        public required string TripId { get; set; }

        [Required]
        [ForeignKey(nameof(Driver))]
        public required string DriverId { get; set; }

        [Required]
        public int QueueOrder { get; set; }

        [Required]
        public DriverStatus Status { get; set; } = DriverStatus.Queued;

        public DateTime? AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Navigation
        public Trip? Trip { get; set; }
        public DriverProfile? Driver { get; set; }
    }

    public enum DriverStatus
    {
        Queued = 0,
        Assigned = 1,
        Started = 2,
        Completed = 3,
        Cancelled = 4
    }
}

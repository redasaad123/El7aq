using Core.Entities;

namespace Web.Models.Staff
{
    public class TripDriverQueueViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string TripId { get; set; } = string.Empty;
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public int QueueOrder { get; set; }
        public DriverStatus Status { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        
        public string StatusText => Status switch
        {
            DriverStatus.Queued => "In Queue",
            DriverStatus.Assigned => "Assigned",
            DriverStatus.Started => "Started Trip",
            DriverStatus.Completed => "Completed",
            DriverStatus.Cancelled => "Cancelled",
            _ => "Unknown"
        };
        
        public string StatusBadgeClass => Status switch
        {
            DriverStatus.Queued => "bg-secondary",
            DriverStatus.Assigned => "bg-primary",
            DriverStatus.Started => "bg-success",
            DriverStatus.Completed => "bg-info",
            DriverStatus.Cancelled => "bg-danger",
            _ => "bg-secondary"
        };
    }
}

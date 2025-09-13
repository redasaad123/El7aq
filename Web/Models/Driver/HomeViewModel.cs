using Core.Entities;

namespace Web.Models.Driver
{
    public class HomeViewModel
    {
        public int Index { get; set; }
    }

    public class DriverHomeViewModel
    {
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationCity { get; set; } = string.Empty;
        public TripDetailsViewModel? CurrentTrip { get; set; }
        public int QueuePosition { get; set; }
        public bool IsMyTurn { get; set; }
        public bool CanStartTrip { get; set; }
        public string? TripQueueId { get; set; }
    }

    public class TripDetailsViewModel
    {
        public string TripId { get; set; } = string.Empty;
        public string RouteId { get; set; } = string.Empty;
        public string StartStation { get; set; } = string.Empty;
        public string EndStation { get; set; } = string.Empty;
        public string StartCity { get; set; } = string.Empty;
        public string EndCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int AvailableSeats { get; set; }
        public int BookedSeats { get; set; }
        public decimal Price { get; set; }
        public DriverStatus Status { get; set; }
    }

    public class DriverQueueViewModel
    {
        public string DriverId { get; set; } = string.Empty;
        public string DriverName { get; set; } = string.Empty;
        public int CurrentPosition { get; set; }
        public int TotalDrivers { get; set; }
        public bool IsMyTurn { get; set; }
        public List<DriverQueueItemViewModel> DriversInQueue { get; set; } = new List<DriverQueueItemViewModel>();
        public List<TripQueueInfoViewModel> TodayTrips { get; set; } = new List<TripQueueInfoViewModel>();
    }

    public class DriverQueueItemViewModel
    {
        public int Position { get; set; }
        public string DriverName { get; set; } = string.Empty;
        public bool IsCurrentDriver { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TripQueueInfoViewModel
    {
        public string TripId { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int QueueCount { get; set; }
    }
}

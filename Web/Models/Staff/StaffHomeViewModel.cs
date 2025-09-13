using Web.Models.Trip;

namespace Web.Models.Staff
{
    public class StaffHomeViewModel
    {
        public string StaffName { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationCity { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public int DriversCount { get; set; }
        public List<TripSummaryViewModel> UpcomingTrips { get; set; } = new List<TripSummaryViewModel>();
    }
}

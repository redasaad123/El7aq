using Web.Models.Trip;

namespace Web.Models.Staff
{
    public class ViewTripsViewModel
    {
        public string StationName { get; set; } = string.Empty;
        public List<TripDetailsViewModel> Trips { get; set; } = new List<TripDetailsViewModel>();
    }
}

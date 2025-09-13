using System.ComponentModel.DataAnnotations;

namespace Web.Models.Staff
{
    public class CreateTripWithQueueViewModel
    {
        public string StationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a route")]
        [Display(Name = "Route")]
        public string RouteId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Departure time is required")]
        [Display(Name = "Departure Time")]
        public DateTime DepartureTime { get; set; } = DateTime.Now.AddHours(1);

        [Required(ErrorMessage = "Available seats is required")]
        [Range(1, 50, ErrorMessage = "Available seats must be between 1 and 50")]
        [Display(Name = "Available Seats")]
        public int AvailableSeats { get; set; } = 1;

        [Required(ErrorMessage = "Please select at least one driver")]
        [Display(Name = "Drivers (in order of priority)")]
        public List<string> SelectedDriverIds { get; set; } = new List<string>();

        public List<RouteViewModel> Routes { get; set; } = new List<RouteViewModel>();
        public List<DriverViewModel> Drivers { get; set; } = new List<DriverViewModel>();
    }
}

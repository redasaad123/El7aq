using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace Web.Models.Trip
{
    public class SearchTripsViewModel
    {
        [Required]
        [Display(Name = "From")]
        public string OriginCityId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "To")]
        public string DestinationCityId { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date")]
        public DateTime Date { get; set; } = DateTime.Today;


        // Search Results
        public List<TripResultViewModel> SearchResults { get; set; } = new List<TripResultViewModel>();
        public bool HasSearched => SearchResults.Any();
    }
}

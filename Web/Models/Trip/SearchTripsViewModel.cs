using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using RequiredAttribute = Microsoft.Build.Framework.RequiredAttribute;

namespace Web.Models.Trip
{
    public class SearchTripsViewModel
    {
        [Required]
        [Display(Name = "From")]
        public string OriginStationId { get; set; } = string.Empty;

        [Required]
        [Display(Name = "To")]
        public string DestinationStationId { get; set; } = string.Empty;

     
    


        // Search Results
        public List<TripResultViewModel> SearchResults { get; set; } = new List<TripResultViewModel>();
        public List<SelectListItem> Stations { get; set; } = new List<SelectListItem>();
        //public bool HasSearched => SearchResults.Any();
    }
}

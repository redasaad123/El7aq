using System.ComponentModel.DataAnnotations;

namespace Web.Models.Staff
{
    public class AddDriverByCarNumberViewModel
    {
        public string StationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Car number is required")]
        [Display(Name = "Car Number")]
        [StringLength(50, ErrorMessage = "Car number cannot exceed 50 characters")]
        public string CarNumber { get; set; } = string.Empty;
    }
}



using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateDriverViewModel
    {
        [Required(ErrorMessage = "Please select a user")]
        [Display(Name = "Select User")]
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "License number is required")]
        [Display(Name = "License Number")]
        [StringLength(50, ErrorMessage = "License number cannot exceed 50 characters")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Car number is required")]
        [Display(Name = "Car Number")]
        [StringLength(50, ErrorMessage = "Car number cannot exceed 50 characters")]
        public string CarNumber { get; set; } = string.Empty;

        public List<UserViewModel> AvailableUsers { get; set; } = new List<UserViewModel>();
    }
}

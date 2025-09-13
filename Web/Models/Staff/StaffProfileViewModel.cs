using System.ComponentModel.DataAnnotations;

namespace Web.Models.Staff
{
    public class StaffProfileViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        public string StationId { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public string StationCity { get; set; } = string.Empty;
    }
}

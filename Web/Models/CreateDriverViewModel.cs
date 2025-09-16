using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class CreateDriverViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format")]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

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

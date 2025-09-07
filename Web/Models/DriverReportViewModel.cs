using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class DriverReportViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; }

        [Display(Name = "Car Number")]
        public string CarNumber { get; set; }

        [Display(Name = "Trips")]
        public int TotalTrips { get; set; }

        // Computed
        public string FullName => $"{FirstName} {LastName}";
    }
}



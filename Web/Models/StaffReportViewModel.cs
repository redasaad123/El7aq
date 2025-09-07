using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class StaffReportViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string StationId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Station")]
        public string StationName { get; set; }

        [Display(Name = "Assigned City")]
        public string StationCity { get; set; }

        // Computed
        public string FullName => $"{FirstName} {LastName}";
    }
}



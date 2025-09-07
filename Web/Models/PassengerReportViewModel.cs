using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class PassengerReportViewModel
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
        
        [Display(Name = "Total Bookings")]
        public int TotalBookings { get; set; }
        
        [Display(Name = "Total Payments")]
        public int TotalPayments { get; set; }
        
        [Display(Name = "Total Spent")]
        public decimal TotalSpent { get; set; }
        
        [Display(Name = "Last Booking")]
        public DateTime? LastBookingDate { get; set; }
        
        [Display(Name = "Registration Date")]
        public DateTime? RegistrationDate { get; set; }
        
        [Display(Name = "Status")]
        public bool IsActive { get; set; }
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public string StatusText => IsActive ? "Active" : "Inactive";
        public string StatusClass => IsActive ? "text-success" : "text-muted";
        public string LastBookingText => LastBookingDate?.ToString("MMM dd, yyyy") ?? "Never";
        public string RegistrationText => RegistrationDate?.ToString("MMM dd, yyyy") ?? "Unknown";
    }
}

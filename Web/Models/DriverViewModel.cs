namespace Web.Models
{
    public class DriverViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;
        public string CarNumber { get; set; } = string.Empty;
        public int TripsCount { get; set; }
        
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}

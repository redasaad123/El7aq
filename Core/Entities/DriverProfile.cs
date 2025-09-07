using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class DriverProfile
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(appUsers))]
        public string UserId { get; set; }

        [Required]
        [MaxLength(50)]
        public string LicenseNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string CarNumber { get; set; }

        // Navigation
        public AppUsers? appUsers  { get; set; }
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public double Lat { get; set; }
        public double Long { get; set; }
    }


}

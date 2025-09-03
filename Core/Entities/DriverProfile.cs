using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
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
    }


}

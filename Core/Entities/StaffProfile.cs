using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
{
    public class StaffProfile
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Station))]
        public string StationId { get; set; }

        // Navigation
        public AppUsers? User { get; set; }
        public Station? Station { get; set; }
    }


}

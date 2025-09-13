using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class StaffProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(User))]
        public required string UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Station))]
        public required string StationId { get; set; }

        // Navigation
        public AppUsers? User { get; set; }
        public Station? Station { get; set; }
    }


}

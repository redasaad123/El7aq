using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities
{
    public class ManagerProfile
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Department { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // Navigation
        public AppUsers? User { get; set; }
    }
}

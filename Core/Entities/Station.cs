using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Station
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required]
        [MaxLength(100)]
        public required string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public required string City { get; set; }

        // Navigation
        public ICollection<Route>? RoutesFrom { get; set; } = new List<Route>();
        public ICollection<Route>? RoutesTo { get; set; } = new List<Route>();
        public ICollection<StaffProfile>? Staff { get; set; } = new List<StaffProfile>();

    }


}

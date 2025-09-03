using System.ComponentModel.DataAnnotations;

namespace El7aq.Domain.Entities
{
    public class Station
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        // Navigation
        public ICollection<Route>? RoutesFrom { get; set; } = new List<Route>();
        public ICollection<Route>? RoutesTo { get; set; } = new List<Route>();
        public ICollection<StaffProfile>? Staff { get; set; } = new List<StaffProfile>();

    }


}

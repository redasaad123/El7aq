using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace El7aq.Domain.Entities
{
    public class Route
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [ForeignKey(nameof(StartStation))]
        public string StartStationId { get; set; }

        [Required]
        [ForeignKey(nameof(EndStation))]
        public string EndStationId { get; set; }
        public decimal Price { get; set; }

        // Navigation
        public Station? StartStation { get; set; }
        public Station? EndStation { get; set; }
        public ICollection<Trip>? Trips { get; set; } = new List<Trip>();

    }


}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Notification
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [ForeignKey("User")]
        public required string UserId { get; set; }

        // Navigation Property
        public AppUsers? User { get; set; }

        public required string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

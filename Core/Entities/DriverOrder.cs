using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class DriverOrder
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public required string DriverId { get; set; }

    }
}

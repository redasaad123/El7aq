using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class AppUsers : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

    } 
}

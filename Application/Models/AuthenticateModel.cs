using System.ComponentModel.DataAnnotations;

namespace Tada.Application.Models
{
    public class AuthenticateModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

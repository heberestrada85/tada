using System.ComponentModel.DataAnnotations;

namespace Tada.Application.Models
{
    public class AuthenticateModel
    {
        [Required]
        [EmailAddress]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

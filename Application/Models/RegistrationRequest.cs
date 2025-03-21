using System.ComponentModel.DataAnnotations;

namespace Tada.Application.Models
{
    public class RegistrationRequest
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Firstname { get; set; }

        [Required]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

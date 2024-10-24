using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tada.Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }

        public string Surname { get; set; }

        public string SecondSurname { get; set; }

        public int IdDepartment { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}

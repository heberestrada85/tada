using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Tada.Domain.Entities
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string Email { get; set; }

        public string Username { get; set; }

        public string Firstname { get; set; }

        public string Surname { get; set; }

        public string  SecondSurname { get; set; }

        public int IdDepartment { get; set; }
    }
}


using System;
using Tada.Domain.Entities;
using Tada.Application.Interface;
using System.Collections.Generic;

namespace Tada.Application.Models
{
    public class UserApp : IMapFrom<ApplicationUser>
    {
        public string UserId { get; set; }

        public string Email { get; set;}

        public string Username { get; set; }

        public string  Firstname { get; set; }

        public string Surname { get; set; }

        public string SecondSurname { get; set; }

        public string Password { get; set; }

        public int IdDepartment { get; set; }

        public string TimeZoneId { get { return "Mountain Standard Time (Mexico)"; } }

        public string CultureInfo
        {
            get { return "es-MX"; }
        }

        public DateTime ExpLastLoginTimeireAt { get; set; }

        public AccessToken AccessToken { get; set; }

        public RefreshAccessToken RefreshToken { get; set; }

        public DateTime LastLoginTime { get; set; }

        public IList<System.Security.Claims.Claim> ClaimsAssign { get; set; }
    }
}


using System;
using System.Linq;
using System.Text.Json;
using Tada.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using Tada.Application.Models;
using System.Security.Principal;
using Tada.Application.Interface;
using System.Collections.Generic;
using Tada.Application.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tada.Infrastructure.Persistence;
using System.IdentityModel.Tokens.Jwt;

namespace Tada.Infrastructure.Identity.Auth
{
    internal sealed class JwtFactory : IJwtFactory
    {
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly JwtIssuerOptions _jwtOptions;
        private RoleManager<ApplicationRole> _roleManager;
        private UserManager<ApplicationUser> _userManager;

        public JwtFactory(IJwtTokenHandler jwtTokenHandler, JwtIssuerOptions jwtIssuerOptions, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _jwtOptions = jwtIssuerOptions;
            _userManager = userManager;
            _roleManager = roleManager;

        }

        public async Task<AccessToken> GenerateEncodedToken(ApplicationUser user)
        {
            ClaimsIdentity identity = GenerateClaimsIdentity(user.Id, user.Username);

            List<Claim> claims = new List<Claim>(new[]
            {
                new Claim(ClaimTypes.Sid, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            });
            IList<string> roles = await _userManager.GetRolesAsync(user);


            foreach (string u in roles)
            {
                ApplicationRole role = await _roleManager.FindByNameAsync(u);
                IList<System.Security.Claims.Claim> claim = await _roleManager.GetClaimsAsync(role);
                foreach (Claim claim1 in claim)
                {
                    var temp = new Claim(ClaimTypes.Role, claim1.Value);
                    claims.Add(temp);
                }

            }

            JwtSecurityToken jwt = new JwtSecurityToken(
                _jwtOptions.Issuer,
                _jwtOptions.Audience,
                claims,
                _jwtOptions.NotBefore,
                _jwtOptions.Expiration,
                _jwtOptions.SigningCredentials);

            var token = _jwtTokenHandler.WriteToken(jwt);
            var time = (int)_jwtOptions.ValidFor.TotalSeconds;

            return new AccessToken(token, time);
        }
        private static ClaimsIdentity GenerateClaimsIdentity(string id, string userName)
        {
            return new ClaimsIdentity(new GenericIdentity(userName, "Token"), new[]
            {
                new Claim("id", id),
            });
        }

        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() -
                               new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero))
                              .TotalSeconds);
        /// <param name="options"></param>
        private static void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}

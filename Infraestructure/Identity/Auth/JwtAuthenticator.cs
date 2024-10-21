using System.Reflection.Metadata;
using System;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;
using Tada.Application.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Tada.Infrastructure.Identity.Auth
{
    public class JwtAuthenticator : IJwtAuthenticator
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtFactory _jwtFactory;
        private readonly ITokenFactory _tokenFactory;
        private readonly IDateTime _dateTimeService;
        private readonly IIdentityService _identity;

        public JwtAuthenticator(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager
            , IJwtFactory jwtFactory, ITokenFactory tokenFactory, IDateTime dateTimeService, IIdentityService identity)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtFactory = jwtFactory;
            _tokenFactory = tokenFactory;
            _dateTimeService = dateTimeService;
            _identity = identity;
        }

        public async Task<(Result result, UserApp user)> AuthenticateUser(AuthenticateModel authenticateUser)
        {
            (Result result, var user, var listroles, var listclaims) = await _identity.AuthenticateUser(authenticateUser);

            if(!result.Succeeded)
            {
                return (result, null);
            }


            (Result res, RefreshToken refreshToken) = await _tokenFactory.GenerateToken(user);
            if (!res.Succeeded)
            {
                return (res, null);
            }

            AccessToken token = await _jwtFactory.GenerateEncodedToken(user);
            IList<string> roles = await _userManager.GetRolesAsync(user);
            List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();

            foreach (string u in roles)
            {
                ApplicationRole role = await _roleManager.FindByNameAsync(u);
                IList<System.Security.Claims.Claim> claim = await _roleManager.GetClaimsAsync(role);
                claims.AddRange(claim);
            }

            var userApp = new UserApp()
            {
                AccessToken = token,
                Firstname = user.Firstname,
                Surname = user.Surname,
                UserId = user.Id,
                LastLoginTime = _dateTimeService.Now,
                RefreshToken = new RefreshAccessToken()
                {
                    ExpireAt = new DateTimeOffset(refreshToken.ExpireAt).ToUnixTimeMilliseconds(),
                    Token = refreshToken.Token
                },
            };
            await _userManager.UpdateAsync(user);

            return (Result.Success(), userApp);
        }

        public async Task<(Result result, UserApp user)> RefreshToken(string token)
        {
            RefreshToken rToken = await _tokenFactory.SearchToken(token);

            if (rToken == null)
            {
                return (Result.Failure(new List<string>() { "Token inválido!" }), null);
            }

            var user = rToken.User;

            IList<string> roles = await _userManager.GetRolesAsync(user);
            List<System.Security.Claims.Claim> claims = new List<System.Security.Claims.Claim>();

            foreach (string u in roles)
            {
                ApplicationRole role = await _roleManager.FindByNameAsync(u);
                IList<System.Security.Claims.Claim> claim = await _roleManager.GetClaimsAsync(role);
                claims.AddRange(claim);
            }

            AccessToken jwt = await _jwtFactory.GenerateEncodedToken(user);

            var userApp = new UserApp()
            {
                AccessToken = jwt,
                Firstname = user.Firstname,
                Surname = user.Surname,
                SecondSurname = user.SecondSurname,
                UserId = user.Id,
                RefreshToken = new RefreshAccessToken()
                {
                    ExpireAt = new DateTimeOffset(rToken.ExpireAt).ToUnixTimeMilliseconds(),
                    Token = rToken.Token
                }
            };

            return (Result.Success(), userApp);
        }
    }
}

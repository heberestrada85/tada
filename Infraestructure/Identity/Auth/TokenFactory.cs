using System;
using System.Linq;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tada.Application.Models;
using Tada.Application.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tada.Infrastructure.Identity.Auth
{
    internal sealed class TokenFactory : ITokenFactory
    {
        private readonly int _expirationIn;

        private readonly IApplicationDbContext _context;

        private readonly IDateTime _dateTime;

        private UserManager<ApplicationUser> _userManager;

        private readonly IServiceScopeFactory _scopeFactory;

        public TokenFactory(IConfiguration config, IDateTime dateTime, IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
            _context = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IApplicationDbContext>();
            _expirationIn = 86400;
            _dateTime = dateTime;
            _userManager = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        }

        public async Task<(Result result, RefreshToken token)> GenerateToken(ApplicationUser user, CancellationToken cancellationToken = default, int size = 32)
        {
            Console.WriteLine("Genera un token con expiracion: " + _expirationIn );
            DateTime today = _dateTime.Now;

            var existingTokens = await _context.RefreshToken
                .Where(t => t.UserId == user.Id && t.Valid && t.ExpireAt < today)
                .ToListAsync();

            foreach (var existingToken in existingTokens)
            {
                existingToken.Valid = false;
            }

            await _context.SaveChangesAsync(cancellationToken);
            RefreshToken token = new RefreshToken
            {
                Valid = true,
                UserId = user.Id,
                ExpireAt = today.AddSeconds(_expirationIn),
                Token = GetTokenString(size)
            };

            try
            {
                _context.RefreshToken.Add(token);
                await _context.SaveChangesAsync(cancellationToken);

                return (Result.Success(), token);
            }
            catch(Exception ex)
            {
                return (Result.Failure(new List<string>() { ex.Message }), null);
            }
        }

        public async Task<RefreshToken> SearchToken(string token, CancellationToken cancellationToken = default)
        {
            var today = _dateTime.Now;
            var trimmedToken = token.Trim();
            Console.WriteLine($"Searching for token: '{trimmedToken}'");
            Console.WriteLine($"Current time: {today}");

            var tokens = await _context.RefreshToken
                .AsQueryable()
                .Where(t => t.Valid == true && t.Token == trimmedToken)
                .ToListAsync(cancellationToken);

            foreach (var t in tokens)
            {
                Console.WriteLine($"Token: {t.Token}, ExpireAt: {t.ExpireAt}, Valid: {t.Valid}");
            }

            var result = tokens
                .FirstOrDefault(t => t.ExpireAt >= today);

            if (result != null)
            {
                Console.WriteLine("Token found and valid. Updating expiry time.");
                result.ExpireAt = _dateTime.Now.AddSeconds(_expirationIn);
                await _context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                Console.WriteLine("Token not found or is invalid/expired.");
            }

            return result;
        }

        private string GetTokenString(int size)
        {
            var randomNumber = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}

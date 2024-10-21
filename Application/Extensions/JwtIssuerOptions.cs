using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace Tada.Application.Extensions
{
    public class JwtIssuerOptions
    {
        public string Issuer { get; set; }

        public string Subject { get; set; }

        public string Audience { get; set; }

        public DateTime Expiration => IssuedAt.Add(ValidFor);

        public DateTime NotBefore => DateTime.UtcNow;

        public DateTime IssuedAt => DateTime.UtcNow;

        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(720);

        public Func<Task<string>> JtiGenerator =>
            () => Task.FromResult(Guid.NewGuid().ToString());

        public SigningCredentials SigningCredentials
        {
            get => new SigningCredentials(SignInKey, SecurityAlgorithms.HmacSha256);
        }

        public SymmetricSecurityKey SignInKey => new SymmetricSecurityKey(SecretKeyEncoding);

        public string SecretKey { get; set; }


        private byte[] SecretKeyEncoding => Encoding.ASCII.GetBytes(SecretKey);
    }
}

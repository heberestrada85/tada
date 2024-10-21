namespace Tada.Application.Models
{

    public sealed class AccessToken
    {
        public string Token { get; }

        public int ExpiresIn { get; }

        public int Status { get; set; }

        public AccessToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
        }

    }
}

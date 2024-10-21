using System.Threading.Tasks;
using Tada.Application.Models;

namespace Tada.Application.Interface
{
    public interface IJwtAuthenticator : IBasicAuthenticator
    {
        Task<(Result result, UserApp user)> RefreshToken(string token);
    }
}

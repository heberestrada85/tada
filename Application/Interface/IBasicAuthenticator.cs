using System.Threading.Tasks;
using Tada.Application.Models;

namespace Tada.Application.Interface
{
    public interface IBasicAuthenticator
    {
        Task<(Result result, UserApp user)> AuthenticateUser(AuthenticateModel authenticateUser);
    }
}

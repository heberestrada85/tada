
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;

namespace Tada.Application.Interface
{
    public interface ITokenFactory
    {
        Task<(Result result, RefreshToken token)> GenerateToken(ApplicationUser user, CancellationToken cancellationToken = default, int size = 32);
        Task<RefreshToken> SearchToken(string Token, CancellationToken cancellationToken = default);
    }
}

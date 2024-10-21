using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;

namespace Tada.Application.Interface
{
    public interface IJwtFactory
    {
        Task<AccessToken> GenerateEncodedToken(ApplicationUser user);
    }
}

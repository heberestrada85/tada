using System.Collections.Generic;

namespace Tada.Application.Interface
{
    public interface ICurrentUserService
    {
        public string UserId { get; }

        public string Username { get; }
    }
}

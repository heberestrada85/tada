using Tada.Application.Interface;
using Tada.Domain.Entities;
using System;

namespace Tada.Application.Models
{
    public class RefreshAccessToken : IMapFrom<RefreshToken>
    {
        public string Token { get; set; }

        public long ExpireAt { get; set; }
    }
}


using System;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Tada.Application.Interface;

namespace Tada.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId  = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Sid);

            UserName  = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (UserId == null) return;

        }

        public string UserId { get; }
        public string UserName { get; }
    }
}

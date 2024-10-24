using System;
using MediatR;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Tada.Application.Models;
using System.Collections.Generic;
using Tada.Application.Interface;
using Microsoft.Extensions.Logging;

namespace Tada.Application.Commands
{
    public class CreateUserCommand : IRequest<(Result result, UserApp user)>
    {
        private readonly UserApp _user;

        public CreateUserCommand(UserApp user)
        {
            _user = user;
        }

        public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand,(Result result, UserApp user)>
        {
            private readonly IIdentityService _identityService;

            public CreateUserCommandHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }
            public Task<(Result result, UserApp user)> Handle(CreateUserCommand request, CancellationToken cancellationToken)
            {
                var temp = _identityService.CreateUserAsync(request._user);
                return temp;
            }
        }
    }

}

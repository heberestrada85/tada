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

namespace Tada.Application.Commands.Users
{
    public class UpdateUserCommand : IRequest<(Result, UserApp)>
    {
        private readonly UserApp _user;
        public UpdateUserCommand(UserApp user)
        {
            _user = user;
        }

        public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, (Result, UserApp)>
        {
            private readonly IIdentityService _identityService;

            public UpdateUserCommandHandler(IIdentityService identityService)
            {
                _identityService = identityService;
            }

            public Task<(Result, UserApp)> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
            {
                Task<(Result, UserApp)> temp = _identityService.EditUserAsync(request._user);

                return temp;
            }
        }
    }

}


using System;
using MediatR;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Tada.Application.Models;
using System.Collections.Generic;
using Tada.Application.Interface;
using System.Text.Json;

namespace Tada.Application.Commands.Users
{
    public class AuthenticateUserCommand : IRequest<(Result result, UserApp user)>
    {

        private readonly AuthenticateModel _authenticateModel;

        public AuthenticateUserCommand(AuthenticateModel authenticateModel)
        {
            _authenticateModel = authenticateModel;
        }

        public class AuthenticateUserCommandHandler : IRequestHandler<AuthenticateUserCommand, (Result result, UserApp user)>
        {
            private readonly IIdentityService _identityService;
            private readonly IJwtFactory _jwtFactory;
            private readonly IMediator _mediator;
            private readonly ITokenFactory _tokenFactory;

            public AuthenticateUserCommandHandler(IIdentityService identityService, IJwtFactory jwtFactory, IMediator mediator, ITokenFactory tokenFactory)
            {
                _identityService = identityService;
                _jwtFactory = jwtFactory;
                _mediator = mediator;
                _tokenFactory = tokenFactory;
            }

            public async Task<(Result result, UserApp user)> Handle(AuthenticateUserCommand request, CancellationToken cancellationToken)
            {
                Console.WriteLine(JsonSerializer.Serialize(request._authenticateModel));
                (Result result, ApplicationUser user, IList<string> roles, IList<Claim> claims) temp = await _identityService.AuthenticateUser(request._authenticateModel);
                Console.WriteLine(JsonSerializer.Serialize(temp));
                if (temp.result.Succeeded == false) {
                    return  (temp.result, new UserApp());
                }
                (Result res, RefreshToken refreshToken) = await _tokenFactory.GenerateToken(temp.user);

                AccessToken token = await _jwtFactory.GenerateEncodedToken(temp.user);

                var userApp = new UserApp()
                {
                    AccessToken = token,
                    Firstname = temp.user.Firstname,
                    Surname = temp.user.Surname,
                    UserId = temp.user.Id,
                    UserName = temp.user.UserName,
                    IdDepartment = temp.user.IdDepartment,
                    ClaimsAssign = temp.claims,
                    RefreshToken = new RefreshAccessToken()
                    {
                        ExpireAt = new DateTimeOffset(refreshToken.ExpireAt).ToUnixTimeMilliseconds(),
                        Token = refreshToken.Token
                    }
                };

                return  (temp.result, userApp);
            }
        }
    }
}

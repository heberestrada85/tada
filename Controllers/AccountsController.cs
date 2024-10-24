using System.Threading.Tasks;
using Tada.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Tada.Application.Commands;
using Microsoft.AspNetCore.Authorization;

namespace Tada.Controllers
{
    public class AccountsController : ApiController
    {

        [HttpPost ("signup")]
        public async Task<IActionResult> Create([FromBody]  UserApp user)
        {
            (Result result, UserApp user) result = await Mediator.Send(new CreateUserCommand(user));

            if (result.result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpPut("recover")]
        public async Task<IActionResult> Update([FromBody]  UserApp user)
        {
            (Result result, UserApp user) result = await Mediator.Send(new UpdateUserCommand(user));
            if (result.result.Succeeded)
                return Ok(result);

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpGet("Token")]
        public async Task<IActionResult> Token([FromBody] AuthenticateModel model)
        {
            (Result result, UserApp user) result = await Mediator.Send(new AuthenticateUserCommand(model));

            if (result.result.Succeeded)
                return Ok(result.user.AccessToken);

            return Unauthorized(result);
        }

        [HttpGet("logout")]
        public IActionResult Register()
        {
            return Unauthorized();
        }
    }
}

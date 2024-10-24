using System.Threading.Tasks;
using Tada.Application.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Tada.Application.Commands.Users;
using Microsoft.AspNetCore.Authorization;

namespace Tada.Controllers
{
    public class HomeController : BaseController
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

        [AllowAnonymous]
        [HttpGet("logout")]
        public IActionResult Register()
        {
            return Unauthorized();
        }

        // [HttpPost]
        // public async Task<IActionResult> Register(string email, string password)
        // {
        //     UserApp user = new UserApp { Email = email, Password = password };
        //     (Result result, UserApp user) result = await Mediator.Send(new CreateUserCommand(user));

        //     var user = new IdentityUser { UserName = email, Email = email };
        //     var result = await _userManager.CreateAsync(user, password);
        //     if (result.Succeeded)
        //     {
        //         await _signInManager.SignInAsync(user, isPersistent: false);
        //         return RedirectToAction("Index", "Home");
        //     }
        //     foreach (var error in result.Errors)
        //     {
        //         ModelState.AddModelError("", error.Description);
        //     }
        //     return View();
        // }
    }
}

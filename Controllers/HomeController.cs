using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tada.Application.Models;
using Tada.Application.Users.Commands;

namespace Tada.Controllers
{
    public class HomeController : BaseController
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Token(string email, string password)
        {
            AuthenticateModel model = new AuthenticateModel { UserName = email, Password = password };
            (Result result, UserApp user) result = await Mediator.Send(new AuthenticateUserCommand(model));

            if (result.result.Succeeded)
                return Ok(result.user.AccessToken);

            ModelState.AddModelError("", "Invalid login attempt");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
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

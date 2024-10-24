using FluentValidation;
using Tada.Application.Models;

namespace Tada.Application.Validators
{
    public class UserApplicationValidator : AbstractValidator<UserApp>
    {
        public UserApplicationValidator()
        {
            RuleFor(e => e.Firstname).NotEmpty();
            RuleFor(e => e.Surname).NotEmpty();
            RuleFor(e => e.Password).NotEmpty();
            RuleFor(e => e.UserName).NotEmpty();
            RuleFor(e => e.Email).NotEmpty();
        }
    }
}

using System;
using AutoMapper;
using System.Linq;
using System.Transactions;
using Tada.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Tada.Application.Models;
using Tada.Application.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Tada.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Tada.Application.Validators;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tada.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTimeService;

        public IdentityService(UserManager<ApplicationUser> userManager, IMapper mapper,
                ICurrentUserService currentUserService, IDateTime dateTimeService
            )
        {
            _userManager = userManager;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _dateTimeService = dateTimeService;
        }

        public async Task<string> GetUserNameAsync(string userId)
        {
            var user = await _userManager.Users.FirstAsync(u => u.Id == userId);

            return user.NormalizedEmail;
        }

        public async Task<(Result Result, UserApp UserId)> CreateUserAsync(UserApp user, bool validateRole = true)
        {
            List<string> errors = new List<string>();

            // UserApplicationValidator validator = new UserApplicationValidator();
            // ValidationResult results = validator.Validate(user);

            // if (!results.IsValid)
            // {
            //     errors.AddRange(results.Errors.Select(error => error.ErrorMessage));

            //     return (Result.Failure(errors), user);
            // }

            try
            {
                ApplicationUser applicationUser = _mapper.Map<ApplicationUser>(user);
                IdentityResult result = await _userManager.CreateAsync(applicationUser, user.Password);
                if (!result.Succeeded)
                {
                    IEnumerable<string> userError = result.Errors.Select(t => t.Description);
                    return (Result.Failure(userError), null);
                }

                return (Result.Success(), user);
            }
            catch (Exception exe)
            {
                return (Result.Failure(new[] {exe.Message}), null);
            }
        }

        public async Task<(Result, UserApp)> EditUserAsync(UserApp user)
        {
            try
            {
                ApplicationUser userTemp = await _userManager.Users
                    .FirstOrDefaultAsync(t => t.Id.Equals(user.UserId));

                if (userTemp == null) return (Result.Failure(new[] {"Error al actualizar el usuario"}), user);
                {
                    try
                    {
                        userTemp.Firstname = user.Firstname;
                        userTemp.Surname = user.Surname;
                        userTemp.SecondSurname = user.SecondSurname;

                        if (!string.IsNullOrEmpty(user.Password))
                        {
                            await _userManager.RemovePasswordAsync(userTemp);
                            await _userManager.AddPasswordAsync(userTemp, user.Password);
                        }

                        IdentityResult result = await _userManager.UpdateAsync(userTemp);
                        if (result.Succeeded)
                        {
                            return (Result.Success(), user);
                        }
                    }
                    catch (Exception exe)
                    {
                        return (Result.Failure(new[] {"Error al actualizar el usuario", exe.Message}), user);
                    }
                }
                return (Result.Failure(new[] {"Error al actualizar el usuario"}), user);
            }
            catch(Exception ex){
                return (Result.Failure(new[] {$"Error al actualizar el usuario {ex.Message}"}), user);
            }
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            ApplicationUser user = _userManager.Users.SingleOrDefault(u => u.Id == userId);

            if (user != null)
            {
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    return Result.Success();
                }
            }

            return Result.Failure(new[] {"No se puede eliminar el usuario solicitado"});
        }

        public async Task<Result> DeleteUserAsync(List<string> userIds)
        {
            foreach (string userId in userIds)
            {
                ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(t => t.Id.Equals(userId));

                IdentityResult result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    return Result.Failure(new[] {"No se puede eliminar alguno de los usuarios indicados"});
                }
            }

            return Result.Success();
        }

        public IQueryable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>> predicate)
        {
            IQueryable<ApplicationUser> applicationUsers = _userManager.Users.Where(predicate)
                .Select(r =>
                    new ApplicationUser
                    {
                        Id = r.Id,
                        Email = r.Email,
                        Firstname = r.Firstname,
                        Surname = r.Surname,
                        SecondSurname = r.SecondSurname,
                        NormalizedEmail = r.NormalizedEmail,
                    }
                );

            return applicationUsers;
        }

        public async Task<(ApplicationUser, IList<string>)> GetById(string userId)
        {
            ApplicationUser applicationUsers = _userManager.Users.Where(o =>  o.Id == userId)
                .Select(r =>
                    new ApplicationUser
                    {
                        Id = r.Id,
                        Email = r.Email,
                        Firstname = r.Firstname,
                        Surname = r.Surname,
                        SecondSurname = r.SecondSurname,
                        NormalizedEmail = r.NormalizedEmail
                    }
                ).FirstOrDefault();

            IList<string> role = await _userManager.GetRolesAsync(applicationUsers);
            return (applicationUsers, role);
        }

        public async Task<(ApplicationUser, IList<string>)> GetUserWithRoles(string userId)
        {
            ApplicationUser applicationUsers = _userManager.Users.Where(o =>  o.Id == userId)
                .Select(r =>
                    new ApplicationUser
                    {
                        Id = r.Id,
                        Firstname = r.Firstname,
                        Surname = r.Surname,
                        SecondSurname = r.SecondSurname,
                        Email = r.Email,
                        NormalizedEmail = r.NormalizedEmail
                    }
                ).FirstOrDefault();

            IList<string> roleNames = await _userManager.GetRolesAsync(applicationUsers);

            return (applicationUsers, roleNames);
        }

        public async Task<Result> DeleteUserAsync(ApplicationUser user)
        {
            var result = await _userManager.DeleteAsync(user);

            return result.ToApplicationResult();
        }

        public async Task<Result> AssingEmployee(string userId)
        {
            ApplicationUser user = await _userManager.Users
               .FirstOrDefaultAsync(t => t.Id.Equals(userId));

            if (user is null)
            {
                return Result.Failure(new[] { "El usuario no se encontro" });
            }

            try
            {
                await _userManager.UpdateAsync(user);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                return Result.Failure( new[] { ex.Message });
            }

            return Result.Success();
        }

        public async Task<(Result result, ApplicationUser user, IList<string> roles, IList<Claim> claims)> AuthenticateUser(AuthenticateModel authenticateUser)
        {
            ApplicationUser user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.Equals(authenticateUser.Email));

            if (user == null)
            {
                return (Result.Failure(new List<string>() { "Usuario no encontrado" }), null, null, null);
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            IList<Claim> claims = await _userManager.GetClaimsAsync(user);

            if (user != null)
            {
                bool result = await _userManager.CheckPasswordAsync(user, authenticateUser.Password);
                    if (!result)
                    {
                        return (Result.Failure(new List<string>() { "Usuario no encontrado" }), null, null, null);
                    }
            }
            else
            {
                return (Result.Failure(new List<string>() { "Usuario no encontrado" }), null, null, null);
            }

            if (user == null)
                return (Result.Failure(new List<string>() { "Usuario no encontrado" }), null, null, null);


            return (Result.Success(), user, roles, claims);
        }
    }
}

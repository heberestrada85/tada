/////////////////////////////////////////////////////////////////////////////////////////////////
//
// Centinela
//
// Copyright (c) 2022, Centinela. Todos los derechos reservados.
// Este archivo es confidencial de Centinela. No distribuir.
//
// Developers : Heber Estrada

using System;
using System.Linq;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Security.Claims;
using Tada.Application.Models;
using System.Collections.Generic;

namespace Tada.Application.Interface
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<(Result Result, UserApp UserId)> CreateUserAsync(UserApp user, bool validateRole = true);

        Task<(Result, UserApp)> EditUserAsync(UserApp user);

        Task<Result> DeleteUserAsync(string userId);

        Task<Result> DeleteUserAsync(List<string> userIds);

        IQueryable<ApplicationUser> GetAll(Expression<Func<ApplicationUser, bool>> predicate);

        Task<(ApplicationUser, IList<string>)> GetById(string userId);

        Task<(ApplicationUser, IList<string>)> GetUserWithRoles(string userId);

        Task<(Result result, ApplicationUser user, IList<string> roles, IList<Claim> claims)> AuthenticateUser(AuthenticateModel authenticateUser);
    }
}

using System;
using AutoMapper;
using System.Linq;
using System.Transactions;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Security.Claims;
using Tada.Application.Models;
using System.Linq.Expressions;
using Tada.Application.Helpers;
using Tada.Application.Interface;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tada.Infrastructure.Identity
{
    public class IdentityRoleService : IIdentityRoleService
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        private readonly ICurrentUserService _currentUserService;

        private readonly IDateTime _dateTime;

        private readonly IMapper _mapper;

        public IdentityRoleService(RoleManager<ApplicationRole> roleManager, ICurrentUserService currentUserService, IDateTime dateTime, IMapper mapper)
        {
            _roleManager = roleManager;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _mapper = mapper;
        }

        public async Task<(Result result, string RolId)> CreateRoleAsync(RoleApp roleApp)
        {
            try
            {
                var role = _mapper.Map<ApplicationRole>(roleApp);

                string roleId = null;

                IdentityResult result = await _roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    return (result.ToApplicationResult(), null);
                }

                foreach (Permissions permission in roleApp.Permissions)
                {
                    result = await _roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Authentication, permission.Name));
                    if (!result.Succeeded)
                    {
                        return (result.ToApplicationResult(), null);
                    }
                }

                roleId = role.Id;

                return (Result.Success(), roleId);
            }
            catch(Exception ex){
                return (Result.Failure(new[]{ex.Message}), null);
            }
        }

        public async Task<Result> DeleteRoleAsync(string rolId)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(rolId);

            if( role != null)
            {
                var result = await _roleManager.UpdateAsync(role);
                return result.ToApplicationResult();
            }

            return Result.Success();
        }

        public IQueryable<ApplicationRole> GetAllRoles(Expression<Func<ApplicationRole, bool>> predicate) => _roleManager.Roles.Where(predicate);

        public async Task<(Result, ICollection<Permissions>)> GetPermissions(string roleId)
        {
            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                return (Result.Failure(new List<string> { "Rol no encontrado" }), null);
            }


            ICollection<Permissions> result = new List<Permissions>();
            IList<Claim> claims = await _roleManager.GetClaimsAsync(role);

            foreach(Claim claim in claims)
            {
                Permissions permission = PermissionHelper.GetPermission(claim.Value);
                if (permission != null)
                {
                    result.Add(permission);
                }
            }

            return ( Result.Success(), result);
        }

        public async Task<ApplicationRole> GetRoleById(string RolId)
        {
            return await _roleManager.FindByIdAsync(RolId);
        }

        public async Task<Result> UpdateRoleAsync(string roleId, RoleApp roleApp)
        {

            ApplicationRole role = await _roleManager.FindByIdAsync(roleId);

            if(role == null)
            {
                return Result.Failure(new List<string> { "Rol no encontrado" });
            }

            IdentityResult result = await ProcessPermissions(role, roleApp.Permissions);

            role = _mapper.Map(roleApp, role);

            if (!result.Succeeded)
            {
                return result.ToApplicationResult();
            }

            result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
            {
                return result.ToApplicationResult();
            }

            return Result.Success();
        }

        private async Task<IdentityResult> ProcessPermissions(ApplicationRole role, ICollection<Permissions> permissions)
        {
            IList<Claim> claims = await _roleManager.GetClaimsAsync(role);
            IdentityResult result = null;

            foreach( Permissions permission in permissions)
            {
                Claim claim = claims
                    .Where(c => c.Value == permission.Name)
                    .FirstOrDefault();

                if(claim == null)
                {
                    result = await _roleManager.AddClaimAsync(role, new Claim(ClaimTypes.Authentication, permission.Name));

                    if(!result.Succeeded)
                    {
                        return result;
                    }

                    continue;
                }

                claims.Remove(claim);
            }

            foreach (Claim claim in claims)
            {
                result = await _roleManager.RemoveClaimAsync(role, claim);

                if (!result.Succeeded)
                {
                    return result;
                }
            }

            if(result == null)
            {
                return IdentityResult.Success;
            }

            return result;
        }
    }
}

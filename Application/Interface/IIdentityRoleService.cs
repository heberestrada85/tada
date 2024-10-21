using System;
using System.Linq;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Tada.Application.Models;
using System.Collections.Generic;

namespace Tada.Application.Interface
{
    public interface IIdentityRoleService
    {
        public Task<(Result result, string RolId)> CreateRoleAsync( RoleApp profile );

        public Task<Result> UpdateRoleAsync( string RolId, RoleApp profile);

        public Task<Result> DeleteRoleAsync( string RolId );

        public IQueryable<ApplicationRole> GetAllRoles(Expression<Func<ApplicationRole, bool>> predicate);

        public Task<ApplicationRole> GetRoleById(string RolId);

        public Task<(Result result, ICollection<Permissions> permissions)> GetPermissions(string RolId);
    }
}

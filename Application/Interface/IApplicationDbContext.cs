using System;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Tada.Application.Interface;
using Microsoft.EntityFrameworkCore;

namespace Tada.Application.Interface
{
    public interface IApplicationDbContext
    {
        DbSet<ApplicationUser> ApplicationUser { get; set; }
        DbSet<ApplicationRole> ApplicationRole { get; set; }
        DbSet<RefreshToken> RefreshToken { get; set; }
        DbSet<Invitations> Invitations { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}

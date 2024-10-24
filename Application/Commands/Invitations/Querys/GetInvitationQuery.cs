using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using AutoMapper.QueryableExtensions;
using Tada.Domain.Entities;
using Tada.Application.Models;
using Tada.Application.Interface;

namespace Tada.Application.Commands
{
    public class GetInvitationsQuery : IRequest<List<InvitationsModel>>
    {
        public class GetInvitationsQueryHandler : IRequestHandler<GetInvitationsQuery, List<InvitationsModel>>
        {
            private readonly IApplicationDbContext _context;
            private readonly IMapper _mapper;

            public GetInvitationsQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<List<InvitationsModel>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
            {
                IQueryable<Invitations> datas = _context.Invitations.Where(x => x.Id > 0);
                List<InvitationsModel> datasResult = await datas.ProjectTo<InvitationsModel>(_mapper.ConfigurationProvider).ToListAsync(cancellationToken);

                return datasResult;
            }
        }
    }
}

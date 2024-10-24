using MediatR;
using AutoMapper;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;
using Tada.Application.Interface;
using Microsoft.EntityFrameworkCore;

namespace Tada.Application.Commands
{
    public class GetInvitationsByIdQuery : IRequest<InvitationsModel>
    {
        /// <summary>
        /// The model
        /// </summary>
        private readonly int _id;

        public GetInvitationsByIdQuery(int id)
        {
            _id = id;
        }

        public class GetInvitationsByIdQueryHandler : IRequestHandler<GetInvitationsByIdQuery, InvitationsModel>
        {
            private readonly IMapper _mapper;

            private readonly IApplicationDbContext _context;

            public GetInvitationsByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<InvitationsModel> Handle(GetInvitationsByIdQuery request, CancellationToken cancellationToken)
            {
                Invitations datas = await _context.Invitations.FirstOrDefaultAsync( l => l.Id == request._id ,cancellationToken);
                InvitationsModel datasResult = _mapper.Map<InvitationsModel>(datas);

                return datasResult;
            }
        }
    }
}

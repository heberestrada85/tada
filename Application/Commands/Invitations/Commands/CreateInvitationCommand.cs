using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;
using Tada.Application.Interface;
using Microsoft.EntityFrameworkCore;

namespace Tada.Application.Commands
{
    public class CreateInvitationsCommand : IRequest<Result>
    {
        private readonly InvitationsModel _model;

        public CreateInvitationsCommand(InvitationsModel model)
        {
            _model = model;
        }

        public class CreateInvitationsQueryHandler : IRequestHandler<CreateInvitationsCommand, Result>
        {

            private readonly IApplicationDbContext _context;

            private readonly IMapper _mapper;

            public CreateInvitationsQueryHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result> Handle(CreateInvitationsCommand request, CancellationToken cancellationToken)
            {
                Invitations toInsert = _mapper.Map<Invitations>(request._model);
                try
                {
                    await _context.Invitations.AddAsync(toInsert);
                    await _context.SaveChangesAsync(cancellationToken);
                    return Result.Success();
                }
                catch (DbUpdateException ex)
                {
                    return Result.Failure(new[]{ ex.Message });
                }
            }
        }
    }
}

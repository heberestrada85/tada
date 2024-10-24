/////////////////////////////////////////////////////////////////////////////////////////////////
//
// Centinela
//
// Copyright (c) 2022, Centinela. Todos los derechos reservados.
// Este archivo es confidencial de Centinela. No distribuir.
//
// Developers : Heber estrada, Chris Marquez

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
    public class UpdateInvitationsCommand : IRequest<Result>
    {
        private readonly InvitationsModel _model;

        public UpdateInvitationsCommand(InvitationsModel model)
        {
            _model = model;
        }

        public class UpdateInvitationsCommandHandler : IRequestHandler<UpdateInvitationsCommand, Result>
        {
            private readonly IApplicationDbContext _context;

            private readonly IMapper _mapper;

            public UpdateInvitationsCommandHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result> Handle(UpdateInvitationsCommand request, CancellationToken cancellationToken)
            {

                Invitations entity = _mapper.Map<Invitations>(request._model);

                try
                {
                    _context.Invitations.Update(entity);
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

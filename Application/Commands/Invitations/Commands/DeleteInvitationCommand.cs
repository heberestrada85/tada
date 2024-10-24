/////////////////////////////////////////////////////////////////////////////////////////////////
//
// Centinela
//
// Copyright (c) 2022, Centinela. Todos los derechos reservados.
// Este archivo es confidencial de Centinela. No distribuir.
//
// Developers : Chris Marquez

using MediatR;
using AutoMapper;
using System.Linq;
using System.Threading;
using Tada.Domain.Entities;
using System.Threading.Tasks;
using Tada.Application.Models;
using System.Collections.Generic;
using Tada.Application.Interface;
using Microsoft.EntityFrameworkCore;

namespace Tada.Application.Commands
{
    public class DeleteInvitationsCommand : IRequest<Result>
    {
        private readonly int _id;

        public DeleteInvitationsCommand(int id)
        {
            _id = id;
        }

        public class DeleteInvitationsCommandHandler : IRequestHandler<DeleteInvitationsCommand, Result>
        {
            private readonly IApplicationDbContext _context;

            private readonly IMapper _mapper;

            public DeleteInvitationsCommandHandler(IApplicationDbContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result> Handle(DeleteInvitationsCommand request, CancellationToken cancellationToken)
            {

                IEnumerable<Invitations> items = _context.Invitations.Where(o => o.Id == request._id);

                try
                {
                    foreach(var item in items)
                            _context.Invitations.Remove(item);
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

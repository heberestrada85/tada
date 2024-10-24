using System;
using Tada.Domain.Entities;
using Tada.Application.Interface;
using System.ComponentModel.DataAnnotations;

namespace Tada.Application.Models
{
    public class InvitationsModel  : IMapFrom<Invitations>
    {
        public long Id { get; set; }

        [Required]
        public string InvitationName { get; set; }

        [Required]
        public DateTime InvitationEntranceDate { get; set; }

        [Required]
        public DateTime InvitationDueDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tada.Domain.Entities
{
    public class Invitations
    {
        public long Id { get; set; }

        public string InvitationName { get; set; }

        public DateTime InvitationEntranceDate { get; set; }

        public DateTime InvitationDueDate { get; set; }
    }
}

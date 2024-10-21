using System;
using Tada.Domain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tada.Domain.Entities
{
   public class RefreshToken
   {
      public string Token { get; set; }

      public string UserId { get; set; }

      public DateTime ExpireAt { get; set; }

      public virtual ApplicationUser User { get; set; }

      public int Status { get; set; }

      public bool Valid { get; set; }
   }
}

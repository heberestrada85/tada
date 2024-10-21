using System.ComponentModel.DataAnnotations;

namespace Tada.Domain.Enums
{
    public enum PermissionsTypes : short
    {
        NotSet = 0,

        [Display(
            GroupName = "Administrator",
            Name = "Administrator",
            Description = "Administrator"
        )]
        Administrator = 1,
    }
}

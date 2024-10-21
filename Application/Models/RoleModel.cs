using System.Collections.Generic;

namespace Tada.Application.Models
{
    public class RoleModel
    {
        public string Name;

        public string Description;

        public long IdCorporacion;

        public long[] Locations;

        public List<string> Permissions;
    }
}

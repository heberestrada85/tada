using System;

namespace Tada.Application.Models
{
    public class Permissions
    {

        public Permissions(string groupName, string name, string description)
        {
            GroupName = groupName;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public string GroupName { get; private set; }

        public string Name { get; private set; }

        public string Description { get; set; }
    }
}

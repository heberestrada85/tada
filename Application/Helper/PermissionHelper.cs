using System;
using System.Reflection;
using Tada.Domain.Enums;
using Tada.Domain.Entities;
using Tada.Application.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Tada.Application.Helpers
{
    public static class PermissionHelper
    {
        private static Dictionary<string, Permissions> _permissions = GeneratePermissions();

        public static ICollection<Permissions> GetPermissions ()
        {
            return _permissions.Values;
        }

        public static Permissions GetPermission(string name)
        {
            if (_permissions.ContainsKey(name))
            {
                return _permissions[name];
            }
            return null;
        }

        private static Dictionary<string, Permissions> GeneratePermissions()
        {
            Type enumType = typeof(PermissionsTypes);
            Dictionary<string, Permissions> permissions = new Dictionary<string, Permissions>();
            foreach (var permissionName in Enum.GetNames(enumType))
            {
                var member = enumType.GetMember(permissionName);
                var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                    continue;

                permissions.Add(displayAttribute.Name, new Permissions(displayAttribute.GroupName, displayAttribute.Name, displayAttribute.Description));
            }

            return permissions;
        }
    }
}

using System;
using AutoMapper;
using Tada.Domain.Entities;
using Tada.Application.Interface;
using System.Collections.Generic;

namespace Tada.Application.Models
{
    public class RoleApp : IMapFrom<ApplicationRole>
    {
        public RoleApp()
        {
            Permissions = new List<Permissions>();
        }

        public string? Id;

        public string Name;

        public string Description;

        public DateTime? CreatedAt;

        public DateTime? UpdatedAt;

        public ICollection<Permissions> Permissions;

        public void Mapping(Profile profile)
        {
            profile.CreateMap<ApplicationRole, RoleApp>()
                .ForMember(p => p.Permissions, opt => opt.Ignore());

            profile.CreateMap<RoleModel, RoleApp>()
                .ForMember(p => p.Permissions, opt => opt.Ignore())
                .ForMember(p => p.Id, opt => opt.Ignore());

            profile.CreateMap<RoleApp, ApplicationRole>()
                .ForMember(d => d.NormalizedName, opt=> opt.MapFrom( src => src.Name ))
                .ForMember(d => d.Id, opt => opt.Ignore());
        }
    }
}

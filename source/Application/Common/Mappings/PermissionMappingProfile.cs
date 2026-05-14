using FSP.Api.Application.Features.Permission.Queries.GetPermissions.DTOs;
using FSP.Api.Domain.Entities.Permission;
using AutoMapper;

namespace FSP.Api.Application.Common.Mappings
{
    public class PermissionMappingProfile : Profile
    {
        public PermissionMappingProfile()
        {
            CreateMap<Permissao, PermissionDto>();
        }
    }
}

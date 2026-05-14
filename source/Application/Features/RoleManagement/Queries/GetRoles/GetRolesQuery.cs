using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetRoles
{
    public class GetRolesQuery : Query<ResponseBase<GetRolesQueryResponse>>
    {
        public GetRolesQuery()
        {
        }
    }
}

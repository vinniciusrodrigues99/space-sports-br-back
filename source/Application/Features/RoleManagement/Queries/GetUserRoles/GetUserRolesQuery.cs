using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.RoleManagement.Queries.GetUserRoles
{
    public class GetUserRolesQuery(Guid userId) : Query<ResponseBase<GetUserRolesQueryResponse>>
    {
        public Guid UserId { get; set; } = userId;
    }
}

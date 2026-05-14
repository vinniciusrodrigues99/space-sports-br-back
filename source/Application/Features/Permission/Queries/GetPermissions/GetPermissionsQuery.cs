using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Permission.Queries.GetPermissions
{
    public class GetPermissionsQuery : Query<ResponseBase<GetPermissionsQueryResponse>>
    {
        public string? Category { get; set; }
        public bool? IsActive { get; set; }

        public GetPermissionsQuery(string? category = null, bool? isActive = null)
        {
            Category = category;
            IsActive = isActive;
        }
    }
}

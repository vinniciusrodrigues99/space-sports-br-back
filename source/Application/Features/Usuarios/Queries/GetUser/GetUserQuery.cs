

using FSP.Api.Domain.Notifications;
using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Queries.GetUser
{
    public class GetUserQuery : Query<ResponseBase<GetUserQueryResponse>>
    {
        public required Guid UserID { get; set; }
    }
}

using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Usuarios.Queries.GetUserProfile;

public record GetUserProfileQuery(Guid IdUsuario) : IRequest<ResponseBase<GetUserProfileQueryResponse>>;

using Microsoft.AspNetCore.Identity;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Entities.Usuario;
using FSP.Api.Domain.Notifications;
using FSP.Api.Application.Features.Usuarios.Queries.GetUser;

namespace FSP.Api.Application.Features.Usuarios.Queries.GetUser
{
    public class GetUserQueryHandler(UserManager<ApplicationUser> _userManager, IMediator _mediator) : IRequestHandler<GetUserQuery, ResponseBase<GetUserQueryResponse>>
    {
        public async Task<ResponseBase<GetUserQueryResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserID.ToString());
            if (user == null)
            {
                await _mediator.Publish(new DomainNotification("GetUser", "Usuário não localizado."), cancellationToken);
                return ResponseBase<GetUserQueryResponse>.Failure();
            }

            return ResponseBase<GetUserQueryResponse>.Success(new GetUserQueryResponse
            {
                IdUsuario = user.Id,
                NomeCompleto = user.NomeCompleto,
                Email = user.Email
            });
        }
    }
}

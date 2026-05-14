using FSP.Api.Application.Features.Usuarios.Commands.UpdateUser;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.UpdateUser
{
    public class UpdateUserCommand(Guid idUsuario, UpdateUserRequest updateUserRequest) : Command<UpdateUserCommandResponse>
    {
        public Guid IdUsuario { get; set; } = idUsuario;
        public UpdateUserRequest UpdateUserRequest { get; set; } = updateUserRequest;
    }
}

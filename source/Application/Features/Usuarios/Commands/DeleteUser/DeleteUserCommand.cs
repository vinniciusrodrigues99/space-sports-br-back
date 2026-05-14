

using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.DeleteUser
{
    public class DeleteUserCommand : Command<Unit>
    {
        public required Guid UserID { get; set; }
    }
}

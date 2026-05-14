using FSP.Api.Application.Features.Usuarios.Commands.CreateUser;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.Application.Features.Usuarios.Commands.CreateUser
{
    public class CreateUserCommand(CreateUserRequest createUserRequest) : Command<CreateUserCommandResponse>
    {
        public CreateUserRequest CreateUserRequest { get; set; } = createUserRequest;
    }
}

using FSP.Api.Application.Common.Helpers;
using FSP.Api.Application.Features.SpaceSports.DTOs;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Constants;
using FSP.Api.Domain.Entities.Usuario;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FSP.Api.Application.Features.SpaceSports.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler(
    UserManager<ApplicationUser> userManager)
    : IRequestHandler<UpdateProfileCommand, ResponseBase<UserDTO>>
{
    public async Task<ResponseBase<UserDTO>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return ResponseBase<UserDTO>.Failure("Usuário não encontrado.");

        user.NomeCompleto = request.Name.Trim();
        if (request.AvatarUrl is not null)
            user.TxFoto = request.AvatarUrl;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return ResponseBase<UserDTO>.Failure(result.Errors.First().Description);

        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? ProfileRoles.Reader;

        return ResponseBase<UserDTO>.Success(new UserDTO
        {
            Id = user.Id.ToString(),
            Name = user.NomeCompleto,
            Email = user.Email ?? "",
            AvatarUrl = user.TxFoto,
            Role = RoleMapper.ToFrontendRole(role),
            CanPublish = user.CanPublish,
            CreatedAt = user.CriadoEm,
        });
    }
}

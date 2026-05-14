using FSP.Api.Domain.Common;

namespace FSP.Api.Application.Features.Autenticacao.Commands.VerifyTwoFactorCode;

public record VerifyTwoFactorCodeCommand(VerifyTwoFactorCodeRequest Request) 
    : IRequest<ResponseBase<VerifyTwoFactorCodeCommandResponse>>;

using Microsoft.AspNetCore.Mvc;
using FSP.Api.Domain.Common;
using FSP.Api.Domain.Notifications;

namespace FSP.Api.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public abstract class BaseController(INotificationHandler<DomainNotification> notifications,
                            INotificationHandler<DomainSuccesNotification> succesNotifications,
                            IMediator mediatorHandler) : Controller
    {
        private readonly DomainNotificationHandler _notifications = (DomainNotificationHandler)notifications;
        private readonly DomainSuccesNotificationHandler _succesNotifications = (DomainSuccesNotificationHandler)succesNotifications;
        private readonly IMediator _mediatorHandler = mediatorHandler;

        protected Guid ClienteId;

        protected bool OperacaoValida()
        {
            return !_notifications.TemNotificacao();
        }

        protected IEnumerable<string> ObterMensagensErro()
        {
            return _notifications.ObterNotificacoes().Select(c => c.Value).ToList();
        }
        protected IEnumerable<string> ObterMensagensDeSucesso()
        {
            return _succesNotifications.ObterNotificacoes().Select(c => c.Value).ToList();
        }

        protected void NotificarErro(string codigo, string mensagem)
        {
            _mediatorHandler.Publish(new DomainNotification(codigo, mensagem));
        }

        protected new IActionResult Response(ResponseBase responseBase)
        {
            var erros = ObterMensagensErro().ToList();
            var sucessos = ObterMensagensDeSucesso().ToList();

            if (erros.Count != 0)
                return BadRequest(ResponseBase.Failure(erros));

            if (sucessos.Count != 0)
            {
                var warningsExistentes = responseBase.Warnings?.ToList() ?? [];
                warningsExistentes.AddRange(sucessos);
                responseBase.Warnings = warningsExistentes;
            }
            return Ok(responseBase);
        }
    }
}
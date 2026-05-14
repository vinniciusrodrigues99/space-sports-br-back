using FSP.Api.Domain.Common;
using MediatR;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace FSP.Api.Domain.Notifications
{
    public abstract class Command<T> : IRequest<ResponseBase<T>>
    {
        public DateTime Timestamp { get; private set; }
        public ValidationResult ValidationResult { get; set; }
        public string IdCorrelation { get; set; }

        public virtual bool EhValido()
        {
            throw new NotImplementedException();
        }
        protected Command()
        {
            ValidationResult = new ValidationResult();
            Timestamp = DateTime.Now;
            IdCorrelation = string.Empty;
        }
    }
}

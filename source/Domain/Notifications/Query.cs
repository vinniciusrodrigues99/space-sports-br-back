using MediatR;

namespace FSP.Api.Domain.Notifications
{
    public abstract class Query<T> : IRequest<T>
    {
        public DateTime Timestamp { get; private set; }
        public string IdCorrelation { get; set; }

        protected Query()
        {
            Timestamp = DateTime.Now;
            IdCorrelation = string.Empty;
        }
    }
}

using FSP.Api.Domain.Entities.BaseAuditable;

namespace FSP.Api.Domain.Entities.Palpite
{
    public class Palpite : BaseAuditableEntity
    {
        public int EventId { get; set; }
        public required string HomeName { get; set; }
        public required string AwayName { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public required string Nickname { get; set; }
        public Guid? UserId { get; set; }
        public string? Stage { get; set; }
    }
}

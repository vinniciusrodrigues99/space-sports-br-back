using System.ComponentModel.DataAnnotations.Schema;

namespace FSP.Api.Domain.Common;

public abstract class BaseEntity
{
    public long Id { get; set; }
}

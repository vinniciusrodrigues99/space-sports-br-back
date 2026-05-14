using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSP.Api.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        public string? UserId { get;  }
        public string? UserName { get; }
        public string? UserEmail { get; }
        bool IsAuthenticated { get; }
    }
}
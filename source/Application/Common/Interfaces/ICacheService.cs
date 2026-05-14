using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSP.Api.Application.Common.Interfaces
{
    public interface ICacheService
    {
        Task<bool> ExistsAsync(string key);
        Task<long> GetCounterAsync(string key);
        Task<long> IncrementCounterAsync(string key, TimeSpan? expiration = null);
        Task SetLockAsync(string key, TimeSpan expiration);
    }
}
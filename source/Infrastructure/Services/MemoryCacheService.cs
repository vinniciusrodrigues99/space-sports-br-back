using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSP.Api.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FSP.Api.Infrastructure.Services
{
    public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
    {
        private readonly IMemoryCache _memoryCache = memoryCache;
        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(_memoryCache.TryGetValue(key, out _));
        }

        public Task<long> GetCounterAsync(string key)
        {
            if (_memoryCache.TryGetValue(key, out long counter))
            {
                return Task.FromResult(counter);
            }
            return Task.FromResult(0L);
        }

        public Task<long> IncrementCounterAsync(string key, TimeSpan? expiration = null)
        {
            long counter = 0;
            if (_memoryCache.TryGetValue(key, out long existingCounter))
            {
                counter = existingCounter;
            }
            counter++;
            var cacheEntryOptions = new MemoryCacheEntryOptions();
            if (expiration.HasValue)
            {
                cacheEntryOptions.SetAbsoluteExpiration(expiration.Value);
            }
            _memoryCache.Set(key, counter, cacheEntryOptions);
            return Task.FromResult(counter);
        }

        public Task SetLockAsync(string key, TimeSpan expiration)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(expiration);
            _memoryCache.Set(key, true, cacheEntryOptions);
            return Task.CompletedTask;
        }
    }
}
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Service
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string CacheKey, object Response, TimeSpan ExpireTime)
        {
            if (Response is null) return;

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var serializedResponse = JsonSerializer.Serialize(Response, options);

            await _database.StringSetAsync(CacheKey, serializedResponse, ExpireTime);
        }

        public async Task<string?> GetCachedResponse(string CacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(CacheKey);

            if (cachedResponse.IsNullOrEmpty) return null;

            return cachedResponse;
        }
    }
}

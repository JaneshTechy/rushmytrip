using StackExchange.Redis;
using System;

namespace TravelWeb.Services
{
    public class RedisCacheService
    {
        private readonly IDatabase _db;

        public RedisCacheService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public string Get(string key)
        {
            return _db.StringGet(key);
        }

        public bool Set(string key, string value, int minutes = 10)
        {
            return _db.StringSet(key, value, TimeSpan.FromMinutes(minutes));
        }
    }
}

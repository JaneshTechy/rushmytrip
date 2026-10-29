using StackExchange.Redis;
using System;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
namespace TravelWeb.Services
{
    public static class RedisService
    {
        private static ConnectionMultiplexer _redis;
        private static readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        public static async Task<IDatabase> GetDatabaseAsync()
        {
            if (_redis == null || !_redis.IsConnected)
            {
                await _lock.WaitAsync();
                try
                {
                    if (_redis == null || !_redis.IsConnected)
                    {
                        var options = new ConfigurationOptions
                        {
                            EndPoints =
                            {
                                "redis-15610.c10.us-east-1-2.ec2.cloud.redislabs.com:15610"
                            },
                            User = "default",
                            Password = "rDQMKmAZrLI8tdMHaQn0fIYmKCA6ENrr",

                            Ssl = true,
                            SslProtocols = SslProtocols.Tls12,
                            SslHost = "redis-15610.c10.us-east-1-2.ec2.cloud.redislabs.com",

                            AbortOnConnectFail = false,
                            ConnectTimeout = 15000,
                            SyncTimeout = 15000,
                            ConnectRetry = 3,
                            KeepAlive = 60
                        };

                        _redis = await ConnectionMultiplexer.ConnectAsync(options);
                    }
                }
                finally
                {
                    _lock.Release();
                }
            }

            return _redis.GetDatabase();
        }
    }
}

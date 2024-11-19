using StackExchange.Redis;
using BackEnd.src.core.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace BackEnd.src.infrastructure.Services
{
    public class RedisServices: IRedisServices
    {
        private readonly ConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private IConfiguration Configuration{get;}

        public RedisServices(IConfiguration configuration){
            Configuration = configuration;

            var options = new ConfigurationOptions
            {
                EndPoints = { $"{configuration["RedisLocal:Host"]}:{configuration["RedisLocal:Port"]}" },
                AbortOnConnectFail = false,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                ConnectRetry = 3,
                KeepAlive = 60,
                ReconnectRetryPolicy = new LinearRetry(1000),
                AllowAdmin = true
            };

            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
        }
        public async Task<bool> Set<T>(string key, T value, TimeSpan? expiry = null){
            try {
                if (!_redis.IsConnected)
                {
                    await _redis.GetDatabase().PingAsync();
                }
                var serializedValue = JsonSerializer.Serialize(value);
                return await _db.StringSetAsync(key, serializedValue, expiry);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Redis error: {ex.Message}");
                throw;
            }
        }
        public async Task<T> Get<T>(string key){
            var value = await _db.StringGetAsync(key);
            return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>(value);
        }
        public async Task<bool> Delete(string key){
            return await _db.KeyDeleteAsync(key);
        }
        public async Task<bool> KeyExists(string key){
            return _db.KeyExists(key);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace CacheSharp.Redis
{
    public sealed class RedisAsyncCache : IAsyncCache<string>, IInitializable
    {
        private IDatabase db;
        
        public List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        public string ProviderName { get { return "Redis"; }}

        public async Task PutAsync(string key, string value, TimeSpan lifeSpan)
        {
            await db.StringSetAsync(key, value, lifeSpan);
        }

        public async Task<string> GetAsync(string key)
        {
            return await db.StringGetAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await db.KeyDeleteAsync(key);
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            string endpoint = parameters["Endpoint"];
            ConnectionMultiplexer redis = await ConnectionMultiplexer.ConnectAsync(endpoint);
            db = redis.GetDatabase();
        }
    }
}
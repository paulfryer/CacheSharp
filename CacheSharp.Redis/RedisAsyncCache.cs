using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheSharp.Redis
{
    public sealed class RedisAsyncCache : IAsyncCache, IInitializable
    {
        private IDatabase db;
        
        public List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        public string ProviderName { get { return "Redis"; }}


        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            var stringValue = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, stringValue, lifeSpan);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var json = await db.StringGetAsync(key);
            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
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
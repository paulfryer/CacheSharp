using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace CacheSharp.Caching
{
    public sealed class RedisAsyncCache : AsyncCache<string>
    {
        private IDatabase db;
       

        public override async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            var endpoint = parameters["Endpoint"];
            var redis = await ConnectionMultiplexer.ConnectAsync(endpoint);
            db = redis.GetDatabase();
        }

        public override List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        protected internal override async Task Put(string key, string value, TimeSpan lifeSpan)
        {
            await db.StringSetAsync(key, value, lifeSpan);
        }

        protected internal override async Task<string> Get(string key)
        {
            return await db.StringGetAsync(key);
        }

        protected internal override async Task Remove(string key)
        {
            await db.KeyDeleteAsync(key);
        }
    }
}
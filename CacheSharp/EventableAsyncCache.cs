using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheSharp
{
    public sealed class EventableAsyncCache : IAsyncCache, ICacheEventable
    {
        private readonly IAsyncCache targetCache;

        public EventableAsyncCache(IAsyncCache targetCache)
        {
            this.targetCache = targetCache;
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            await targetCache.InitializeAsync(parameters);
        }

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            if (PrePut != null)
                PrePut(this, new PutEventArgs(key, value, lifeSpan));
            await targetCache.PutAsync(key, value, lifeSpan);
            if (PostPut != null)
                PostPut(this, new PutEventArgs(key, value, lifeSpan));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            if (PreGet != null)
                PreGet(this, new CacheEventArgs(key));
            var value = await targetCache.GetAsync<T>(key);
            if (PostGet != null)
                PostGet(this, new GetEventArgs(key));
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            if (PreRemove != null)
                PreRemove(this, new CacheEventArgs(key));
            await targetCache.RemoveAsync(key);
            if (PostRemove != null)
                PostRemove(this, new CacheEventArgs(key));
        }

        public string ProviderName
        {
            get { return targetCache.ProviderName; }
        }

        public List<string> InitializationProperties
        {
            get { return targetCache.InitializationProperties; }
        }

        public event EventHandler<PutEventArgs> PrePut;
        public event EventHandler<PutEventArgs> PostPut;

        public event EventHandler<CacheEventArgs> PreGet;
        public event EventHandler<GetEventArgs> PostGet;

        public event EventHandler<CacheEventArgs> PreRemove;
        public event EventHandler<CacheEventArgs> PostRemove;
    }
}
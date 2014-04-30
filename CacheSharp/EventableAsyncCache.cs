using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheSharp
{
    public sealed class EventableAsyncCache<T> : IAsyncCache<T>, IInitializable, ICacheEventable<T>
    {
        private readonly IAsyncCache<T> targetCache;

        public EventableAsyncCache(IAsyncCache<T> targetCache)
        {
            this.targetCache = targetCache;
        }

        public async Task PutAsync(string key, T value, TimeSpan lifeSpan)
        {
            if (PrePut != null)
                PrePut(this, new PutEventArgs<T>(key, value, lifeSpan));
            await targetCache.PutAsync(key, value, lifeSpan);
            if (PostPut != null)
                PostPut(this, new PutEventArgs<T>(key, value, lifeSpan));
        }

        public async Task<T> GetAsync(string key)
        {
            if (PreGet != null)
                PreGet(this, new CacheEventArgs(key));
            T value = await targetCache.GetAsync(key);
            if (PostGet != null)
                PostGet(this, new GetEventArgs<T>(key));
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

        public event EventHandler<PutEventArgs<T>> PrePut;
        public event EventHandler<PutEventArgs<T>> PostPut;

        public event EventHandler<CacheEventArgs> PreGet;
        public event EventHandler<GetEventArgs<T>> PostGet;

        public event EventHandler<CacheEventArgs> PreRemove;
        public event EventHandler<CacheEventArgs> PostRemove;

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            if (targetCache is IInitializable)
                await (targetCache as IInitializable).InitializeAsync(parameters);
        }

        public string ProviderName
        {
            get
            {
                if (targetCache is IInitializable)
                    return (targetCache as IInitializable).ProviderName;
                return targetCache.GetType().Name;
            }
        }

        public List<string> InitializationProperties
        {
            get
            {
                if (targetCache is IInitializable)
                    return (targetCache as IInitializable).InitializationProperties;
                return new List<string>();
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheSharp.Events;

namespace CacheSharp.Caching
{
    public abstract class AsyncCache<T> : IAsyncCache<T>, IDisposable
    {
        public abstract Task InitializeAsync(Dictionary<string, string> parameters);

        public virtual string ProviderName
        {
            get { return GetType().Name.Replace("AsyncCache", string.Empty); }
        }

        public virtual string CacheName
        {
            get { return "Default"; }
        }

        public abstract List<string> InitializationProperties { get; }

        async Task IAsyncCache<T>.PutAsync(string key, T value, TimeSpan lifeSpan)
        {
            if (PrePut != null)
                PrePut(this, new PutEventArgs<T>(key, value, lifeSpan));
            await Put(key, value, lifeSpan);
            if (PostPut != null)
                PostPut(this, new PutEventArgs<T>(key, value, lifeSpan));
        }

        async Task<T> IAsyncCache<T>.GetAsync(string key)
        {
            if (PreGet != null)
                PreGet(this, new CacheEventArgs(key));
            T value = await Get(key);
            if (PostGet != null)
                PostGet(this, new GetEventArgs<T>(key));
            return value;
        }

        async Task IAsyncCache<T>.RemoveAsync(string key)
        {
            if (PreRemove != null)
                PreRemove(this, new CacheEventArgs(key));
            await Remove(key);
            if (PostRemove != null)
                PostRemove(this, new CacheEventArgs(key));
        }

        protected abstract Task Put(string key, T value, TimeSpan lifeSpan);
        protected abstract Task<T> Get(string key);
        protected abstract Task Remove(string key);

        public event EventHandler<PutEventArgs<T>> PrePut;
        public event EventHandler<PutEventArgs<T>> PostPut;

        public event EventHandler<CacheEventArgs> PreGet;
        public event EventHandler<GetEventArgs<T>> PostGet;

        public event EventHandler<CacheEventArgs> PreRemove;
        public event EventHandler<CacheEventArgs> PostRemove;

        public virtual void Dispose()
        {
            // optional method to implement.
        }
    }
}
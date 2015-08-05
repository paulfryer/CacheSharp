using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace CacheSharp.Local
{
    public class LocalCache : ISyncCache, IAsyncCache
    {
        private MemoryCache cache;

       

        public void Initialize(Dictionary<string, string> parameters)
        {
            var cacheName = parameters["CacheName"];
            cache = new MemoryCache(cacheName);
        }

        public void Put<T>(string key, T value, TimeSpan lifeSpan)
        {
            cache.Add(new CacheItem(key)
            {
                Value = value
            }, new CacheItemPolicy
            {
                SlidingExpiration = lifeSpan
            });
        }

        public T Get<T>(string key)
        {
            return (T) cache.Get(key);
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }


        public List<string> InitializationProperties
        {
            get
            {
                return new List<string>
                {
                    "CacheName"
                };
            }
        }

        public string ProviderName
        {
            get { return "Local"; }
        }

        public Task InitializeAsync(Dictionary<string, string> parameters)
        {
            return Task.Run(() => Initialize(parameters));
        }

        public Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            return Task.Run(() => Put(key, value, lifeSpan));
        }

        public Task<T> GetAsync<T>(string key)
        {
            return Task.Run(() => Get<T>(key));
        }

        public Task RemoveAsync(string key)
        {
            return Task.Run(() => Remove(key));
        }
    }
}
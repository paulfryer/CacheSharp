using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace CacheSharp.Local
{
    public class LocalCache : ISyncCache
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
            get { return this.GetType().Name; }
        }
    }
}
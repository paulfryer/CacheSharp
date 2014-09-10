using System;
using System.Collections;
using System.Collections.Generic;
using Tangosol.Net;
using Tangosol.Util.Filter;

namespace CacheSharp.Coherence
{
    // TODO: Need to test this still!
    public class CoherenceCache : ISyncCache
    {
        private INamedCache cache;

        public void Put<T>(string key, T value, TimeSpan lifeSpan)
        {
            cache.Add(key, value);
        }

        public T Get<T>(string key)
        {
            var values = cache.GetValues(new KeyFilter(new ArrayList {key}));
            return (T) values[0];
        }

        public void Remove(string key)
        {
            cache.Remove(key);
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            var cacheName = parameters["CacheName"];
            cache = CacheFactory.GetCache(cacheName);
            // TODO: figure out how this stuff works for real. This is just a guess.
            cache.CacheService.Configure(CacheFactory.DefaultCacheConfig);
            // follow directions here: http://docs.oracle.com/cd/E18686_01/coh.37/e18678/net_library.htm
        }

        public List<string> InitializationProperties
        {
            get { return new List<string> {"CacheName"}; }
        }

        public string ProviderName
        {
            get { return "Coherence"; }
        }
    }
}
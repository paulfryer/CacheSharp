﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tangosol.Net;
using Tangosol.Util.Filter;

namespace CacheSharp.Coherence
{
    // TODO: Need to test this still!
    public class CoherenceAsyncCache : IAsyncCache, IInitializable
    {
        private INamedCache cache;

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            await Task.Run(() => cache.Add(key, value));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            object[] values = await Task.Run(() => cache.GetValues(new KeyFilter(new ArrayList {key})));
            return (T)values[0];
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() => cache.Remove(key));
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            string cacheName = parameters["CacheName"];
            cache = CacheFactory.GetCache(cacheName);
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
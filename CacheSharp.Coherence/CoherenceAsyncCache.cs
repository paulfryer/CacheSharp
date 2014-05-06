using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tangosol.Net;
using Tangosol.Util.Filter;

namespace CacheSharp.Coherence
{
    // TODO: Need to test this still!
    public class CoherenceAsyncCache : IAsyncCache<string>, IInitializable
    {
        private INamedCache cache;

        public async Task PutAsync(string key, string value, TimeSpan lifeSpan)
        {
            await Task.Run(() => cache.Add(key, value));
        }

        public async Task<string> GetAsync(string key)
        {
            object[] values = await Task.Run(() => cache.GetValues(new KeyFilter(new ArrayList {key})));
            return values[0] as string;
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
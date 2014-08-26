using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Newtonsoft.Json;

namespace CacheSharp.Azure
{
    public class AzureAsyncCache : IAsyncCache, IInitializable
    {
        private DataCache cache;
        public string CacheRegion { get; set; }

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            await Task.Run(() => cache.Put(key, value, lifeSpan, CacheRegion));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run(() =>
            {
                var json = cache.Get(key, CacheRegion) as string;
                var value = JsonConvert.DeserializeObject<T>(json);
                return value;
            });
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() => cache.Remove(key, CacheRegion));
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            string cacheName = parameters["CacheName"];
            CacheRegion = parameters["CacheRegion"];


            cache = new DataCache(cacheName);
            cache.CreateRegion(CacheRegion);
        }

        public List<string> InitializationProperties
        {
            get { return new List<string> {"CacheName", "CacheRegion"}; }
        }

        public string ProviderName
        {
            get { return "Azure"; }
        }
    }
}
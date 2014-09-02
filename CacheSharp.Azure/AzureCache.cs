using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Newtonsoft.Json;

namespace CacheSharp.Azure
{
    public class AzureCache : IAsyncCache, IInitializable, ISyncCache
    {
        private DataCache cache;
        public string CacheRegion { get; set; }

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            await Task.Run(() => Put(key, value, lifeSpan));
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await Task.Run(() => Get<T>(key));
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() => Remove(key));
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            var cacheName = parameters["CacheName"];
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

        public void Put<T>(string key, T value, TimeSpan lifeSpan)
        {
            cache.Put(key, value, lifeSpan, CacheRegion);
        }

        public T Get<T>(string key)
        {
            var json = cache.Get(key, CacheRegion) as string;
            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public void Remove(string key)
        {
            cache.Remove(key, CacheRegion);
        }
    }
}
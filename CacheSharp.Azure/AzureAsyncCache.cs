using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationServer.Caching;
using Microsoft.ApplicationServer.Caching.AzureClientHelper;

namespace CacheSharp.Azure
{
    public class AzureAsyncCache : IAsyncCache<string>, IInitializable
    {




        public async Task PutAsync(string key, string value, TimeSpan lifeSpan)
        {
            await Task.Run(() => cache.Put(key, value, null, lifeSpan));
        }

        public async Task<string> GetAsync(string key)
        {
            return await Task.Run(() => cache.Get(key) as string);
        }

        public async Task RemoveAsync(string key)
        {
            await Task.Run(() => cache.Remove(key));
        }

        private DataCache cache;
        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            var cacheName = parameters["CacheName"];
            cache = new DataCache(cacheName, cacheName);
            return;

        }

        public List<string> InitializationProperties
        {
            get
            {
                return new List<string>{"CacheName"};
            }
        }

        public string ProviderName { get { return "Azure"; } }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MemcachedSharp;

namespace CacheSharp.Memcached
{
    public sealed class MemcachedAsyncCache : IAsyncCache<string>, IInitializable, IDisposable
    {
        private MemcachedClient client;

        public List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        public string ProviderName { get { return "Memcached"; } }

        public async Task PutAsync(string key, string value, TimeSpan lifeSpan)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);


            await client.Set(key, bytes, new MemcachedStorageOptions
            {
                ExpirationTime = lifeSpan
            });
        }

        public async Task<string> GetAsync(string key)
        {
            MemcachedItem result = await client.Get(key);
            if (result == null)
                return null;
            var sr = new StreamReader(result.Data);
            return await sr.ReadToEndAsync();
        }

        public async Task RemoveAsync(string key)
        {
            await client.Delete(key);
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            string endpoint = parameters["Endpoint"];
            client = new MemcachedClient(endpoint, new MemcachedOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                ReceiveTimeout = TimeSpan.FromSeconds(2),
                EnablePipelining = true,
                MaxConnections = 8,
                MaxConcurrentRequestPerConnection = 30
            });
            
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
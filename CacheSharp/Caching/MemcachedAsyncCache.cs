using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MemcachedSharp;

namespace CacheSharp.Caching
{
    public sealed class MemcachedAsyncCache : AsyncCache<string>
    {
        private MemcachedClient client;

        public override List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        public override async Task InitializeAsync(Dictionary<string, string> parameters)
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

        public override void Dispose()
        {
            client.Dispose();
        }

        protected override async Task Put(string key, string value, TimeSpan lifeSpan)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(value);


           
            await client.Set(key, bytes, new MemcachedStorageOptions
            {
                ExpirationTime = lifeSpan
            });
        }

        protected override async Task<string> Get(string key)
        {
            MemcachedItem result = await client.Get(key);
            if (result == null)
                return null;
            var sr = new StreamReader(result.Data);
            return await sr.ReadToEndAsync();
        }

        protected override async Task Remove(string key)
        {
            await client.Delete(key);
        }
    }
}
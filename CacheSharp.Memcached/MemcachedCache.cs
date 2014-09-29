using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using MemcachedSharp;
using Newtonsoft.Json;

namespace CacheSharp.Memcached
{
    public sealed class MemcachedCache : IAsyncCache, IDisposable
    {
        private MemcachedClient client;

        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            var json = JsonConvert.SerializeObject(value);
            var bytes = Encoding.UTF8.GetBytes(json);
            await client.Set(key, bytes, new MemcachedStorageOptions
            {
                ExpirationTime = lifeSpan
            });
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var result = await client.Get(key);
            if (result == null)
                return default(T);
            var sr = new StreamReader(result.Data);
            var json = await sr.ReadToEndAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task RemoveAsync(string key)
        {
            await client.Delete(key);
        }

        public void Dispose()
        {
            client.Dispose();
        }

        public List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint"}; }
        }

        public string ProviderName
        {
            get { return "Memcached"; }
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            var endpoint = parameters["Endpoint"];
            client = new MemcachedClient(endpoint, new MemcachedOptions
            {
                ConnectTimeout = TimeSpan.FromSeconds(2),
                ReceiveTimeout = TimeSpan.FromSeconds(2),
                EnablePipelining = true,
                MaxConnections = 8,
                MaxConcurrentRequestPerConnection = 30
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace CacheSharp.Redis
{
    public sealed class RedisCache : IAsyncCache, ISyncCache
    {
        private IDatabase db;


        public async Task PutAsync<T>(string key, T value, TimeSpan lifeSpan)
        {
            var stringValue = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, stringValue, lifeSpan);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var json = await db.StringGetAsync(key);
            if (string.IsNullOrEmpty(json))
                return default(T);
            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public async Task RemoveAsync(string key)
        {
            await db.KeyDeleteAsync(key);
        }

        public List<string> InitializationProperties
        {
            get { return new List<string> {"Endpoint", "Key", "UseSsl"}; }
        }

        public string ProviderName
        {
            get { return "Redis"; }
        }

        public async Task InitializeAsync(Dictionary<string, string> parameters)
        {
            var endpoint = parameters["Endpoint"];
            var conBuilder = new StringBuilder();
            conBuilder.Append(endpoint);
            if (parameters.ContainsKey("Key"))
            {
                 var key = parameters["Key"];
                conBuilder.Append(",password=" + key);
            }
            if (parameters.ContainsKey("UseSsl"))
            {
                var useSsl = bool.Parse(parameters["UseSsl"]);
                conBuilder.Append(",ssl=" + useSsl);
            }
            var connectionString = conBuilder.ToString();
            var redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
            db = redis.GetDatabase();
        }

        public void Initialize(Dictionary<string, string> parameters)
        {
            throw new NotImplementedException();
        }

        public void Put<T>(string key, T value, TimeSpan lifeSpan)
        {
            var stringValue = JsonConvert.SerializeObject(value);
            db.StringSet(key, stringValue);
        }

        public T Get<T>(string key)
        {
            var json = db.StringGet(key);
            if (string.IsNullOrEmpty(json))
                return default(T);
            var value = JsonConvert.DeserializeObject<T>(json);
            return value;
        }

        public void Remove(string key)
        {
            db.KeyDelete(key);
        }
    }
}
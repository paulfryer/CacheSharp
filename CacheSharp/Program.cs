using System;
using System.Configuration;
using CacheSharp.Caching;
using CacheSharp.Configuration;
using CacheSharp.Runners;

namespace CacheSharp
{
    internal class Program
    {
        private static void Main()
        {
            var configuration = new FileLoadConfiguration();
            switch (configuration.CacheProvider)
            {
                case "Redis":
                    (new LoadRunner<RedisAsyncCache>(new FileLoadConfiguration(), ConfigurationManager.AppSettings["Redis.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Memcached":
                    (new LoadRunner<MemcachedAsyncCache>(new FileLoadConfiguration(), ConfigurationManager.AppSettings["Memcached.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Sql":
                    (new LoadRunner<SqlAsyncCache>(new FileLoadConfiguration(), ConfigurationManager.AppSettings["Sql.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                default:
                    throw new Exception("Unsupported cache provider: " + configuration.CacheProvider);
            }
            Console.ReadKey();
        }
    }
}
using System;
using System.Configuration;
using CacheSharp.Azure;
using CacheSharp.Coherence;
using CacheSharp.Local;
using CacheSharp.Memcached;
using CacheSharp.Redis;
using CacheSharp.SQLServer;

namespace CacheSharp.Console
{
    internal class Program
    {
        private static void Main()
        {
            var configuration = new FileLoadConfiguration();
            switch (configuration.CacheProvider)
            {
                case "Redis":
                    (new LoadRunner<RedisCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Redis.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Memcached":
                    (new LoadRunner<MemcachedCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Memcached.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Sql":
                    (new LoadRunner<SqlCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Sql.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Local":
                    (new LoadRunner<LocalCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Local.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                    /*
                case "Azure":
                    (new LoadRunner<AzureCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Azure.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Coherence":
                    (new LoadRunner<CoherenceCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Coherence.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;*/
                default:
                    throw new Exception("Unsupported cache provider: " + configuration.CacheProvider);
            }
            System.Console.ReadKey();
        }
    }
}
using System;
using System.Configuration;
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
                    (new LoadRunner<RedisAsyncCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Redis.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Memcached":
                    (new LoadRunner<MemcachedAsyncCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Memcached.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                case "Sql":
                    (new LoadRunner<SqlAsyncCache>(new FileLoadConfiguration(),
                        ConfigurationManager.AppSettings["Sql.ThingSpeakWriteApiKey"])).Run().Wait();
                    break;
                default:
                    throw new Exception("Unsupported cache provider: " + configuration.CacheProvider);
            }
            System.Console.ReadKey();
        }
    }
}
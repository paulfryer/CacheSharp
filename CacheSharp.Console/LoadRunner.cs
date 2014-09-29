using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CacheSharp.Console
{
    public sealed class LoadRunner<TCache>
        where TCache : IAsyncCache, IInitializable, new()
    {
        private readonly ILoadConfiguration loadConfiguration;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<long, int>> metrics =
            new ConcurrentDictionary<string, ConcurrentDictionary<long, int>>();

        private readonly object metricLock = new object();
        private readonly string thingSpeakApiKey;
        
        private readonly Timer timer = new Timer(SampleWindowSize.TotalMilliseconds);

        public LoadRunner(ILoadConfiguration loadConfiguration, string thingSpeakApiKey)
        {
            this.loadConfiguration = loadConfiguration;
            this.thingSpeakApiKey = thingSpeakApiKey;

            if (!string.IsNullOrEmpty(thingSpeakApiKey))
            {
                timer.Elapsed += OnTimerElapsed;
                timer.Start();
            }
        }

        public static TimeSpan SampleWindowSize
        {
            get
            {
                var seconds = 15; // default
                if (ConfigurationManager.AppSettings.AllKeys.Contains("SampleWindowSizeSeconds"))
                    seconds = Convert.ToInt16(ConfigurationManager.AppSettings["SampleWindowSizeSeconds"]);
                return TimeSpan.FromSeconds(seconds);
            }
        }

        public IList<string> TestOperations
        {
            get
            {
                return ConfigurationManager.AppSettings["TestOperations"].Split(',').Select(op => op.Trim()).ToList();
            }
        }

        private int GetMetricAverage(string metric, TimeSpan timeSpan)
        {
            if (!metrics.ContainsKey(metric))
                return 0;
            var seconds = Convert.ToInt32(DateTime.UtcNow.Subtract(new DateTime(2000, 1, 1)).TotalSeconds);
            var value =
                Convert.ToInt32(metrics[metric].Where(d => d.Key >= seconds - timeSpan.TotalSeconds)
                    .Select(d => d.Value)
                    .Average());
            return value;
        }

        private void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            using (var wc = new WebClient())
            {
                const int seconds = 15;
                var puts = GetMetricAverage("Put", TimeSpan.FromSeconds(seconds));
                var gets = GetMetricAverage("Get", TimeSpan.FromSeconds(seconds));
                var removes = GetMetricAverage("Remove", TimeSpan.FromSeconds(seconds));
                var total = puts + gets + removes;
                var url =
                    string.Format(
                        "http://api.thingspeak.com/update?key={0}&field1={1}&field2={2}&field3={3}&field4={4}",
                        thingSpeakApiKey, total, puts, gets, removes);
                wc.DownloadString(new Uri(url));
            }
        }


        public async Task Run()
        {
            var actions = new List<Task>();
            for (var i = 0; i < loadConfiguration.ParallelInstances; i++)
            {
                var targetCache = new TCache();
                var initializationProperties = targetCache.InitializationProperties.ToDictionary(
                    initializationProperty => initializationProperty,
                    initializationProperty =>
                        ConfigurationManager.AppSettings[targetCache.ProviderName + "." + initializationProperty]);
                await targetCache.InitializeAsync(initializationProperties);
                var eventableCache = new EventableAsyncCache(targetCache);
                eventableCache.PrePut += (sender, args) => UpdateMetric("Put");
                eventableCache.PreGet += (sender, args) => UpdateMetric("Get");
                eventableCache.PostRemove += (sender, args) => UpdateMetric("Remove");

                Action action = async () =>
                {
                    for (var x = 0; x < loadConfiguration.IterationsPerInstance; x++)
                    {
                        var characters = loadConfiguration.CharactersPerMessage;
                        var sb = new StringBuilder();
                        for (var c = 0; c < characters; c++)
                            sb.Append(c);
                        var key = "Key" + x;
                        var value = sb.ToString();
                        if (TestOperations.Contains("Put"))
                            await eventableCache.PutAsync(key, value,
                                TimeSpan.FromMinutes(5));
                        if (TestOperations.Contains("Get"))
                        {
                            var valueFromCache = await eventableCache.GetAsync<string>(key);
                            //System.Console.WriteLine(valueFromCache);
                        }

                        if (TestOperations.Contains("Remove"))
                            await eventableCache.RemoveAsync(key);
                    }
                };
                actions.Add(Task.Run(action));
            }
            await Task.WhenAll(actions);
        }

        private async Task UpdateMetric(string metricKey)
        {
            if (!metrics.ContainsKey(metricKey))
                metrics.TryAdd(metricKey, new ConcurrentDictionary<long, int>());
            var seconds = Convert.ToInt32(DateTime.UtcNow.Subtract(new DateTime(2000, 1, 1)).TotalSeconds);
            if (!metrics[metricKey].ContainsKey(seconds))
            {
                metrics[metricKey].TryAdd(seconds, 1);
                if (metrics[metricKey].ContainsKey(seconds - 1))
                {
                    var countPerSecond = metrics[metricKey][seconds - 1];
                    var n = countPerSecond/100;
                    lock (metricLock)
                    {
                        System.Console.WriteLine();
                        foreach (var n1 in new object[n])
                            System.Console.Write(" ");
                        System.Console.Write(metricKey + ": " + countPerSecond);
                    }
                }
            }
            else metrics[metricKey][seconds]++;
        }
    }
}
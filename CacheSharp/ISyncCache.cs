using System;
using System.Collections.Generic;

namespace CacheSharp
{
    public interface ISyncCache : ICache
    {
        void Initialize(Dictionary<string, string> parameters);
        void Put<T>(string key, T value, TimeSpan lifeSpan);
        T Get<T>(string key);
        void Remove(string key);
    }
}
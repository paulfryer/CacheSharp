using System;

namespace CacheSharp
{
    public interface ISyncCache
    {
        void Put<T>(string key, T value, TimeSpan lifeSpan);
        T Get<T>(string key);
        void Remove(string key);
    }
}
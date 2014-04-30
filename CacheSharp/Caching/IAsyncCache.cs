using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheSharp.Caching
{
    public interface IAsyncCache<T>
    {
        Task Initialize(Dictionary<string, string> parameters);
        Task Put(string key, T value, TimeSpan lifeSpan);
        Task<T> Get(string key);
        Task Remove(string key);
    }
}
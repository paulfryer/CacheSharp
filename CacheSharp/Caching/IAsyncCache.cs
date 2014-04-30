using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CacheSharp.Caching
{
    public interface IAsyncCache<T>
    {
        Task InitializeAsync(Dictionary<string, string> parameters);
        Task PutAsync(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
using System;
using System.Threading.Tasks;

namespace CacheSharp
{

    public interface IAsyncCache
    {
        Task PutAsync<T>(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
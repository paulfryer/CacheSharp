using System;
using System.Threading.Tasks;

namespace CacheSharp
{
    public interface IAsyncCache<T>
    {
        Task PutAsync(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync(string key);
        Task RemoveAsync(string key);
    }
}
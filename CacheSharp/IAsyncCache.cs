using System;
using System.Threading.Tasks;

namespace CacheSharp
{

    /*
    public interface IAsyncCache<T>
    {
        Task PutAsync(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync(string key);
        Task RemoveAsync(string key);
    }*/

    public interface IAsyncCache
    {
        Task PutAsync<T>(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }

    public interface ISyncCache
    {
        void Put<T>(string key, T value, TimeSpan lifeSpan);
        T Get<T>(string key);
        void Remove(string key);
    }
}
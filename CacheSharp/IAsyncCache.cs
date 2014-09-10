using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CacheSharp
{
    public interface IAsyncCache : ICache
    {
        Task InitializeAsync(Dictionary<string, string> parameters);
        Task PutAsync<T>(string key, T value, TimeSpan lifeSpan);
        Task<T> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
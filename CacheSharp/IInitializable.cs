using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CacheSharp
{
    public interface IInitializable
    {
        Task InitializeAsync(Dictionary<string, string> parameters);
        List<string> InitializationProperties { get; }
        string ProviderName { get; }
    }
}
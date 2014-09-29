using System.Collections.Generic;

namespace CacheSharp
{
    public interface ICache
    {
        List<string> InitializationProperties { get; }
        string ProviderName { get; }
    }
}
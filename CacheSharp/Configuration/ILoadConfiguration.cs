using System.Security.Cryptography.X509Certificates;

namespace CacheSharp.Configuration
{
    public interface ILoadConfiguration
    {
        int ParallelInstances { get; }
        int IterationsPerInstance { get; }
        string CacheProvider { get; }
        int CharactersPerMessage { get; }
    }
}
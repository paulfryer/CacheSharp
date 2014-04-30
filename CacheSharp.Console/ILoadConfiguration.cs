namespace CacheSharp.Console
{
    public interface ILoadConfiguration
    {
        int ParallelInstances { get; }
        int IterationsPerInstance { get; }
        string CacheProvider { get; }
        int CharactersPerMessage { get; }
    }
}
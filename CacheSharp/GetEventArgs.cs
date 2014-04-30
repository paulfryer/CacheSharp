namespace CacheSharp
{
    public class GetEventArgs<T> : CacheEventArgs
    {
        public GetEventArgs(string key) : base(key)
        {
        }

        public T Value { get; set; }
        public bool CacheHit { get; set; }
        public bool CacheMiss { get; set; }
    }
}
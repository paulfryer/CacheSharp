namespace CacheSharp
{
    public class GetEventArgs : CacheEventArgs
    {
        public GetEventArgs(string key) : base(key)
        {
        }

        public object Value { get; set; }

        public bool CacheMiss
        {
            get { return Value == null; }
        }
    }
}
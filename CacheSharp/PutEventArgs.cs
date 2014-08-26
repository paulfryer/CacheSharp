using System;

namespace CacheSharp
{
    public class PutEventArgs : CacheEventArgs
    {
        public PutEventArgs(string key, object value, TimeSpan lifeSpan) : base(key)
        {
            Value = value;
            LifeSpan = lifeSpan;
        }

        public object Value { get; set; }
        public TimeSpan LifeSpan { get; set; }
    }
}
using System;

namespace CacheSharp.Events
{
    public class PutEventArgs<T> : CacheEventArgs
    {
        public PutEventArgs(string key, T value, TimeSpan lifeSpan) : base(key)
        {
            Value = value;
            LifeSpan = lifeSpan;
        }

        public T Value { get; set; }
        public TimeSpan LifeSpan { get; set; }
    }
}
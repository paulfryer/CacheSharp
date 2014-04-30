using System;

namespace CacheSharp
{
    public interface ICacheEventable<T>
    {
        event EventHandler<PutEventArgs<T>> PrePut;
        event EventHandler<PutEventArgs<T>> PostPut;

        event EventHandler<CacheEventArgs> PreGet;
        event EventHandler<GetEventArgs<T>> PostGet;

        event EventHandler<CacheEventArgs> PreRemove;
        event EventHandler<CacheEventArgs> PostRemove;
    }
}
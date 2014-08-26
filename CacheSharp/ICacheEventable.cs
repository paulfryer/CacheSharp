using System;

namespace CacheSharp
{
    public interface ICacheEventable
    {
        event EventHandler<PutEventArgs> PrePut;
        event EventHandler<PutEventArgs> PostPut;

        event EventHandler<CacheEventArgs> PreGet;
        event EventHandler<GetEventArgs> PostGet;

        event EventHandler<CacheEventArgs> PreRemove;
        event EventHandler<CacheEventArgs> PostRemove;
    }
}
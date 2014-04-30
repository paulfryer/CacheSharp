using System;

namespace CacheSharp.Events
{
    public class CacheEventArgs : EventArgs
    {
        public CacheEventArgs(string key)
        {
            Key = key;
        }

        public string Key { get; set; }
    }
}
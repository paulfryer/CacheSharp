using System;

namespace CacheSharp
{
    public class CacheEventArgs : EventArgs
    {
        public CacheEventArgs(string key)
        {
            Key = key;
        }

        /// <summary>
        /// The name of the cache provider.
        /// </summary>
        public string ProviderName { get; set; }

        public string Key { get; set; }
    }
}
using System;

namespace CacheSharp
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public string Topic { get; set; }
        public string Value { get; set; }
    }
}
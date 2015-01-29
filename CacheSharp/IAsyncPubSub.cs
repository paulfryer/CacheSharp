using System;
using System.Threading.Tasks;

namespace CacheSharp
{
    public interface IAsyncPubSub
    {
        Task PublishAsync(string topic, string value);
        Task SubscribeAsync(string topic);
        Task UnsubscribeAsync(string topic);
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
    }
}
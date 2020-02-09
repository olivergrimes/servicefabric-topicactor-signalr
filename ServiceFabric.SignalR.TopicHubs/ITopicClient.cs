using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.TopicHubs
{
    public interface ITopicClient<THub, TIHub, TSubscription, TMessage>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
    {
        Task Subscribe(TSubscription subscription, string connectionId);

        Task Unsubscribe(TSubscription subscription, string connectionId);

        Task UnsubscribeAll(string connectionId);
    }
}

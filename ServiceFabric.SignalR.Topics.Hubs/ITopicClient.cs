using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public interface ITopicClient<THub, TIHub, TSubscription, TMessage>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
        where TSubscription : ITopicId
    {
        Task Subscribe(TSubscription subscription, string connectionId);

        Task Unsubscribe(TSubscription subscription, string connectionId);

        Task UnsubscribeAll(string connectionId);
    }
}

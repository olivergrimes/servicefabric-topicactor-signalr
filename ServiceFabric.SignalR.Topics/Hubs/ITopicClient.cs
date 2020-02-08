using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public interface ITopicClient<TMessage, THub, TIHub>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
    {
        Task Subscribe(string connectionId, string topicId);

        Task Unsubscribe(string connectionId, string topicId);

        Task UnsubscribeAll(string connectionId);
    }
}

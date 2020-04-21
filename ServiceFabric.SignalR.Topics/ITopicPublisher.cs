using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicPublisher<TMessage, TSubscription>
        where TSubscription : ITopicId
    {
        Task Publish(TMessage message, TSubscription subscription);
    }
}

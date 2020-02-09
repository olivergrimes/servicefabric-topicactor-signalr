using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicPublisher<TMessage, TSubscription>
        where TSubscription : ITopicId<TSubscription>
    {
        Task Publish(TMessage message, TSubscription subscription);
    }
}

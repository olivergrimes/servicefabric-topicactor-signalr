using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicSubscriber<TMessage, TSubscription>
        where TSubscription : ITopicId<TSubscription>
    {
        Task Subscribe(TSubscription subscription);

        Task Unsubscribe(TSubscription subscription);
    }
}

using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicSubscriber<TMessage>
    {
        Task Subscribe(string topicId);

        Task Unsubscribe(string topicId);
    }
}

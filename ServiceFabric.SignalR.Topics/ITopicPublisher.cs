using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicPublisher<TMessage>
    {
        Task Publish(string topicId, TMessage message);
    }
}

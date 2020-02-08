using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicMessageCallback<TMessage>
    {
        Task OnMessage(string topicId, TMessage message);
    }
}

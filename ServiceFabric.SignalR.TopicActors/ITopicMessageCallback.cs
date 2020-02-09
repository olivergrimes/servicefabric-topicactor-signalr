using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicMessageCallback<TMessage, TSubscription>
    {
        Task OnMessage(TSubscription subscription, TMessage message);
    }
}

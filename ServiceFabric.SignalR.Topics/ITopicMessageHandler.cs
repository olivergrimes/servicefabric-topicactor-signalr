using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicMessageHandler<TMessage, TSubscription>
    {
        Task OnMessage(TSubscription subscription, TMessage message);
    }
}

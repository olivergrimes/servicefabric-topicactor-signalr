using System.Threading.Tasks;

namespace ServiceFabric.SignalR.TopicHubs
{
    public interface ITopicHub<in TMessage>
    {
        Task OnMessage(TMessage update);
    }
}

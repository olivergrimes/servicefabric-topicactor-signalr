using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public interface ITopicHub<in TMessage>
    {
        Task OnMessage(TMessage update);
    }
}

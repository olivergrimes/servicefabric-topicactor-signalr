using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public interface ITopicActor : IActor, IActorEventPublisher<ITopicActorEvents>
    {
        Task PublishMessage(TopicActorMessage message);
    }
}

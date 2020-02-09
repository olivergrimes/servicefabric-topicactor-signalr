using Microsoft.ServiceFabric.Actors;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.TopicActors
{
    public interface ITopicActor : IActor, IActorEventPublisher<ITopicActorEvents>
    {
        Task PublishMessage(TopicActorMessage message);
    }
}

using Microsoft.ServiceFabric.Actors;

namespace ServiceFabric.SignalR.TopicActors
{
    public interface ITopicActorEvents : IActorEvents
    {
        void OnMessage(TopicActorMessage message);
    }
}

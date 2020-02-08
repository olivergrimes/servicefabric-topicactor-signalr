using Microsoft.ServiceFabric.Actors;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public interface ITopicActorEvents : IActorEvents
    {
        void OnMessage(TopicActorMessage message);
    }
}

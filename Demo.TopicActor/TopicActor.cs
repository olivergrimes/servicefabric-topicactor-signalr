using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using ServiceFabric.SignalR.Topics.Actors;

namespace Demo.TopicActor
{
    [StatePersistence(StatePersistence.None)]
    internal class TopicActor : Actor, ITopicActor
    {
        public TopicActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        { }

        public Task PublishMessage(TopicActorMessage message)
        {
            var @event = GetEvent<ITopicActorEvents>();
            @event.OnMessage(message);

            return Task.CompletedTask;
        }
    }
}

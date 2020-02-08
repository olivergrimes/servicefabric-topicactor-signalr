using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Generator;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public class TopicActorPublisher<TMessage> : ITopicPublisher<TMessage>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public TopicActorPublisher(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Publish(string topicId, TMessage message)
        {
            var uri = ActorNameFormat.GetFabricServiceUri(typeof(ITopicActor));
            var actor = _actorProxyFactory.CreateActorProxy<ITopicActor>(uri, new ActorId(topicId));
            var body = JsonConvert.SerializeObject(message);

            await actor.PublishMessage(new TopicActorMessage
            {
                Id = topicId,
                Body = body
            });
        }
    }
}

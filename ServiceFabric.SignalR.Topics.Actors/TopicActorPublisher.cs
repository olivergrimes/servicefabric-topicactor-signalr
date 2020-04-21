using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Generator;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public class TopicActorPublisher<TMessage, TSubscription> : ITopicPublisher<TMessage, TSubscription>
        where TSubscription : ITopicId
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public TopicActorPublisher(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public async Task Publish(TMessage message, TSubscription subscription)
        {
            var uri = ActorNameFormat.GetFabricServiceUri(typeof(ITopicActor));
            var topicId = subscription.GetTopicId();
            var actor = _actorProxyFactory.CreateActorProxy<ITopicActor>(uri, new ActorId(topicId));

            var serialisedMessage = JsonConvert.SerializeObject(message);
            var serialisedSubscription = JsonConvert.SerializeObject(subscription);

            await actor.PublishMessage(new TopicActorMessage
            {
                Subscription = serialisedSubscription,
                Message = serialisedMessage
            });
        }
    }
}

using Microsoft.ServiceFabric.Actors.Client;
using System;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public class TopicActorSubscriberFactory<TMessage> : ITopicSubscriberFactory<TMessage>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public TopicActorSubscriberFactory(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public ITopicSubscriber<TMessage> Create(ITopicMessageCallback<TMessage> callback)
        {
            return new TopicActorSubscriber<TMessage>(_actorProxyFactory, callback);
        }
    }
}

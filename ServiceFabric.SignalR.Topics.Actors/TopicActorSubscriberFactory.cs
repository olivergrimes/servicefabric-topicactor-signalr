using Microsoft.ServiceFabric.Actors.Client;
using System;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public class TopicActorSubscriberFactory<TMessage, TSubscription> : ITopicSubscriberFactory<TMessage, TSubscription>
        where TSubscription : ITopicId<TSubscription>
    {
        private readonly IActorProxyFactory _actorProxyFactory;

        public TopicActorSubscriberFactory(IActorProxyFactory actorProxyFactory)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
        }

        public ITopicSubscriber<TMessage, TSubscription> Create(ITopicMessageCallback<TMessage, TSubscription> callback)
        {
            return new TopicActorSubscriber<TMessage, TSubscription>(_actorProxyFactory, callback);
        }
    }
}

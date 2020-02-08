using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Generator;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Actors
{
    public class TopicActorSubscriber<TMessage, TSubscription> : ITopicActorEvents, ITopicSubscriber<TMessage, TSubscription>
        where TSubscription : ITopicId<TSubscription>
    {
        private readonly IActorProxyFactory _actorProxyFactory;
        private readonly ITopicMessageCallback<TMessage, TSubscription> _callback;
        private readonly Dictionary<string, ITopicActor> _proxies = new Dictionary<string, ITopicActor>();
        private readonly SemaphoreSlim _proxyLock = new SemaphoreSlim(1, 1);

        public TopicActorSubscriber(IActorProxyFactory actorProxyFactory, ITopicMessageCallback<TMessage, TSubscription> callback)
        {
            _actorProxyFactory = actorProxyFactory ?? throw new ArgumentNullException(nameof(actorProxyFactory));
            _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        }

        void ITopicActorEvents.OnMessage(TopicActorMessage message)
        {
            var deserialisedMessage = JsonConvert.DeserializeObject<TMessage>(message.Message);
            var deserialisedSubscription = JsonConvert.DeserializeObject<TSubscription>(message.Subscription);

            _callback.OnMessage(deserialisedSubscription, deserialisedMessage);
        }

        public Task Subscribe(TSubscription subscription)
        {
            return WithLock(async () =>
            {
                var topicId = subscription.GetTopicId();
                if (!_proxies.ContainsKey(topicId))
                {
                    var uri = ActorNameFormat.GetFabricServiceUri(typeof(ITopicActor));
                    var actor = _actorProxyFactory.CreateActorProxy<ITopicActor>(uri, new ActorId(topicId));
                    await actor.SubscribeAsync(this);

                    _proxies.Add(topicId, actor);
                }
            });
        }

        public Task Unsubscribe(TSubscription subscription)
        {
            return WithLock(async () =>
            {
                var topicId = subscription.GetTopicId();
                if (_proxies.TryGetValue(topicId, out var actor))
                {
                    await actor.UnsubscribeAsync(this);
                    _proxies.Remove(topicId);
                }
            });
        }

        private async Task WithLock(Func<Task> action)
        {
            try
            {
                await _proxyLock.WaitAsync();
                await action();
            }
            finally
            {
                _proxyLock.Release();
            }
        }
    }
}

using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public class TopicClient<THub, TIHub, TSubscription, TMessage> :
        ITopicClient<THub, TIHub, TSubscription, TMessage>,
        ITopicMessageCallback<TMessage, TSubscription>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
        where TSubscription : ITopicId
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<string, TopicSubscription<TSubscription>> _topicConnections = 
            new ConcurrentDictionary<string, TopicSubscription<TSubscription>>();
        
        private readonly ITopicSubscriber<TMessage, TSubscription> _topicSubscriber;
        
        private readonly IHubContext<THub, TIHub> _hubContext;

        public TopicClient(
            ITopicSubscriberFactory<TMessage, TSubscription> topicSubscriberFactory,
            IHubContext<THub, TIHub> hubContext)
        {
            if (topicSubscriberFactory == null)
            {
                throw new ArgumentNullException(nameof(topicSubscriberFactory));
            }

            _topicSubscriber = topicSubscriberFactory.Create(this);
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public Task Subscribe(TSubscription subscription, string connectionId)
        {
            return WithLock(async () =>
            {
                await _topicSubscriber.Subscribe(subscription);

                _topicConnections.AddOrUpdate(subscription.GetTopicId(), 
                    new TopicSubscription<TSubscription>
                    {
                        Subscription = subscription,
                        ConnectionIds = new HashSet<string> { connectionId }
                    },
                    (_, topicSubscription) =>
                    {
                        topicSubscription.ConnectionIds.Add(connectionId);
                        return topicSubscription;
                    });
            });
        }

        public Task Unsubscribe(TSubscription subscription, string connectionId)
        {
            return WithLock(async () =>
            {
                var topicId = subscription.GetTopicId();
                if (!_topicConnections.TryRemove(topicId, out var  topicSubscription))
                {
                    return;
                }

                topicSubscription.ConnectionIds.Remove(connectionId);

                var topicHasConnections = false;

                if (topicSubscription.ConnectionIds.Any())
                {
                    topicHasConnections = _topicConnections.TryAdd(topicId, topicSubscription);
                }

                if (!topicHasConnections)
                {
                    //All connections removed, unsubscribe
                    await _topicSubscriber.Unsubscribe(subscription);
                }
            });
        }

        public async Task UnsubscribeAll(string connectionId)
        {
            var topicConnections = _topicConnections
                .Where(t => t.Value.ConnectionIds.Contains(connectionId))
                .ToList();

            foreach (var topicConnection in topicConnections)
            {
                await Unsubscribe(topicConnection.Value.Subscription, connectionId);
            }
        }

        public Task OnMessage(TSubscription subscription, TMessage message)
        {
            if (!_topicConnections.TryGetValue(subscription.GetTopicId(), out var topicSubscription))
            {
                return Task.CompletedTask;
            }

            var hubs = _hubContext.Clients.Clients(topicSubscription.ConnectionIds.ToList());
            hubs.OnMessage(message);

            return Task.CompletedTask;
        }

        private async Task WithLock(Func<Task> action)
        {
            try
            {
                await _lock.WaitAsync();
                await action();
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}

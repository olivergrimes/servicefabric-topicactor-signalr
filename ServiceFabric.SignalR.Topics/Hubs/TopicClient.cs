using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public class TopicClient<TMessage, THub, TIHub, TSubscription> :
        ITopicClient<TMessage, THub, TIHub, TSubscription>,
        ITopicMessageCallback<TMessage, TSubscription>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
        where TSubscription : ITopicId<TSubscription>
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);

        private readonly ConcurrentDictionary<TSubscription, HashSet<string>> _topicConnections = 
            new ConcurrentDictionary<TSubscription, HashSet<string>>(); //<topicId, HashSet<connectionId>>
        
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

                _topicConnections.AddOrUpdate(subscription, new HashSet<string> { connectionId },
                    (_, connectionIds) =>
                    {
                        connectionIds.Add(connectionId);
                        return connectionIds;
                    });
            });
        }

        public Task Unsubscribe(TSubscription subscription, string connectionId)
        {
            return WithLock(async () =>
            {
                if (!_topicConnections.TryRemove(subscription, out var connectionIds))
                {
                    return;
                }

                connectionIds.Remove(connectionId);

                var topicHasConnections = false;

                if (connectionIds.Any())
                {
                    topicHasConnections = _topicConnections.TryAdd(subscription, connectionIds);
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
                .Where(t => t.Value.Contains(connectionId))
                .ToList();

            foreach (var topicConnection in topicConnections)
            {
                await Unsubscribe(topicConnection.Key, connectionId);
            }
        }

        public Task OnMessage(TSubscription subscription, TMessage message)
        {
            if (!_topicConnections.TryGetValue(subscription, out var connectionIds))
            {
                return Task.CompletedTask;
            }

            var hubs = _hubContext.Clients.Clients(connectionIds.ToList());
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

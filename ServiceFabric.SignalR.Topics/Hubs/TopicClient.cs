using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public class TopicClient<TMessage, THub, TIHub> :
        ITopicClient<TMessage, THub, TIHub>,
        ITopicMessageCallback<TMessage>
        where THub : Hub<TIHub>
        where TIHub : class, ITopicHub<TMessage>
    {
        private readonly ITopicSubscriber<TMessage> _topicSubscriber;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
        private readonly ConcurrentDictionary<string, HashSet<string>> _topicConnections = new ConcurrentDictionary<string, HashSet<string>>(); //<topicId, HashSet<connectionId>>
        private readonly IHubContext<THub, TIHub> _hubContext;

        public TopicClient(
            ITopicSubscriberFactory<TMessage> topicSubscriberFactory,
            IHubContext<THub, TIHub> hubContext)
        {
            if (topicSubscriberFactory == null)
            {
                throw new ArgumentNullException(nameof(topicSubscriberFactory));
            }

            _topicSubscriber = topicSubscriberFactory.Create(this);
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        }

        public Task Subscribe(string connectionId, string topicId)
        {
            return WithLock(async () =>
            {
                await _topicSubscriber.Subscribe(topicId);

                _topicConnections.AddOrUpdate(topicId, new HashSet<string> { connectionId },
                    (_, connectionIds) =>
                    {
                        connectionIds.Add(connectionId);
                        return connectionIds;
                    });
            });
        }

        public Task Unsubscribe(string connectionId, string topicId)
        {
            return WithLock(async () =>
            {
                if (!_topicConnections.TryRemove(topicId, out var connectionIds))
                {
                    return;
                }

                connectionIds.Remove(connectionId);

                var topicHasConnections = false;

                if (connectionIds.Any())
                {
                    topicHasConnections = _topicConnections.TryAdd(topicId, connectionIds);
                }

                if (!topicHasConnections)
                {
                    //All connections removed, unsubscribe
                    await _topicSubscriber.Unsubscribe(topicId);
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
                await Unsubscribe(connectionId, topicConnection.Key);
            }
        }

        public Task OnMessage(string topicId, TMessage message)
        {
            if (!_topicConnections.TryGetValue(topicId, out var connectionIds))
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

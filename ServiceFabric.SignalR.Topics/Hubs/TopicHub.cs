using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public abstract class TopicHub<THub, TIHub, TSubscription, TMessage> : Hub<TIHub>
           where THub : Hub<TIHub>
           where TIHub : class, ITopicHub<TMessage>
    {
        private readonly ITopicClient<TMessage, THub, TIHub> _topicClient;
        private readonly Func<TSubscription, HubCallerContext, Task<bool>> _authoriseSubscription;
        private readonly Func<TSubscription, string> _topicIdGenerator;

        public TopicHub(
            ITopicClient<TMessage, THub, TIHub> topicClient,
            Func<TSubscription, HubCallerContext, Task<bool>> authoriseSubscription,
            Func<TSubscription, string> topicIdGenerator)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException(nameof(topicClient));
            _authoriseSubscription = authoriseSubscription ?? throw new ArgumentNullException(nameof(authoriseSubscription));
            _topicIdGenerator = topicIdGenerator ?? throw new ArgumentNullException(nameof(topicIdGenerator));
        }

        public async Task Subscribe(TSubscription subscription)
        {
            var topicId = _topicIdGenerator(subscription);

            if (await _authoriseSubscription(subscription, Context))
            {
                await _topicClient.Subscribe(Context.ConnectionId, topicId);
            }
        }

        public async Task Unsubscribe(TSubscription subscription)
        {
            var topicId = _topicIdGenerator(subscription);

            await _topicClient.Unsubscribe(Context.ConnectionId, topicId);
        }

        public async Task UnsubscribeAll()
        {
            await _topicClient.UnsubscribeAll(Context.ConnectionId);
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            await UnsubscribeAll();

            await base.OnDisconnectedAsync(exception);
        }
    }
}

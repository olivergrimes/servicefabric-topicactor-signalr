using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public abstract class TopicHub<THub, TIHub, TSubscription, TMessage> : Hub<TIHub>
           where THub : Hub<TIHub>
           where TIHub : class, ITopicHub<TMessage>
    {
        private readonly ITopicClient<TMessage, THub, TIHub, TSubscription> _topicClient;
        private readonly Func<TSubscription, HubCallerContext, Task<bool>> _authoriseSubscription;

        public TopicHub(
            ITopicClient<TMessage, THub, TIHub, TSubscription> topicClient,
            Func<TSubscription, HubCallerContext, Task<bool>> authoriseSubscription)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException(nameof(topicClient));
            _authoriseSubscription = authoriseSubscription ?? throw new ArgumentNullException(nameof(authoriseSubscription));
        }

        public async Task Subscribe(TSubscription subscription)
        {
            if (await _authoriseSubscription(subscription, Context))
            {
                await _topicClient.Subscribe(subscription, Context.ConnectionId);
            }
        }

        public async Task Unsubscribe(TSubscription subscription)
        {
            await _topicClient.Unsubscribe(subscription, Context.ConnectionId);
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

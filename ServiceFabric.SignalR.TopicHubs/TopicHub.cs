using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace ServiceFabric.SignalR.TopicHubs
{
    public abstract class TopicHub<THub, TIHub, TSubscription, TMessage> : Hub<TIHub>
           where THub : Hub<TIHub>
           where TIHub : class, ITopicHub<TMessage>
    {
        private readonly ITopicClient<THub, TIHub, TSubscription, TMessage> _topicClient;
        private readonly Func<TSubscription, HubCallerContext, Task<bool>> _authorise;

        public TopicHub(
            ITopicClient<THub, TIHub, TSubscription, TMessage> topicClient,
            Func<TSubscription, HubCallerContext, Task<bool>> authorise)
        {
            _topicClient = topicClient ?? throw new ArgumentNullException(nameof(topicClient));
            _authorise = authorise ?? throw new ArgumentNullException(nameof(authorise));
        }

        public async Task Subscribe(TSubscription subscription)
        {
            if (await _authorise(subscription, Context))
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

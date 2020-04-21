namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicSubscriberFactory<TMessage, TSubscription>
        where TSubscription : ITopicId
    {
        ITopicSubscriber<TMessage, TSubscription> Create(ITopicMessageCallback<TMessage, TSubscription> callback);
    }
}

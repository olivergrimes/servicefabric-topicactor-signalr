namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicSubscriberFactory<TMessage>
    {
        ITopicSubscriber<TMessage> Create(ITopicMessageCallback<TMessage> callback);
    }
}

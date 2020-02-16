# servicefabric-topicactor-signalr

This library provides a framework for scaling SignalR in Service Fabric applications using Actor Events.  This method avoids the overhead and cost of an external backplane.

## Usage

Create an Actor service in the application that implements the `ITopicActor` actor interface included in the package, for example:

```
[StatePersistence(StatePersistence.None)]
internal class TopicActor : Actor, ITopicActor
{
    public TopicActor(ActorService actorService, ActorId actorId)
        : base(actorService, actorId)
    { }

    public Task PublishMessage(TopicActorMessage message)
    {
        var @event = GetEvent<ITopicActorEvents>();
        @event.OnMessage(message);

        return Task.CompletedTask;
    }
}
```

Each actor instance in this service will represent a specific pub/sub topic.

Register the topics services your SignalR host service:

Register in **Startup.cs**:

```
public void ConfigureServices(IServiceCollection services)
{
    services.AddSignalR();
    services.RegisterTopics();
}
```

This maps the dependencies that are required by the topic Hubs.

Then create your topic Hub classes, ensuring they inherit the `TopicHub<THub, TIHub, TSubscription, TMessage>` base class, e.g.:

```
public class AuctionHub : 
    TopicHub<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate>
{
    public AuctionHub(
        ITopicClient<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate> topicClient)
        : base(topicClient, authorise: (subscription, context) =>
            {
                return Task.FromResult(true); //Used to check resource based authorisation for requested subscription
            })
    { }
}
```

The `TMessage` and `TSubscription` types should live in the `TopicActor` interfaces library as these will be shared across publisher and subscriber.

The `TSubscription` type must implement the `ITopicId` interface, this allows an `ActorId` to be generated from your subscription contract.

Use the `ITopicPublisher<TMessage, TSubscription>`/`TopicActorPublisher` types to publish messages from any service within your application, e.g.:

```
public EventPublisher(
  StatelessServiceContext context, 
  ITopicPublisher<AuctionUpdate, AuctionSubscription> topicPublisher) : base(context)
{
    _topicPublisher = topicPublisher ?? throw new ArgumentNullException(nameof(topicPublisher));
}

private Task Publish(AuctionUpdate update)
{
   return _topicPublisher.Publish(update);
}
```

Any SignalR client subscribed to that particular topic will receive the update.  If a service doesn't have any clients subscribed to that topic, it won't receive the update.

This repository contains the source for the nuget package and also a working demo example.


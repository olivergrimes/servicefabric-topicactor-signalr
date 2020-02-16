# servicefabric-topicactor-signalr

This library provides a framework for scaling SignalR in Service Fabric applications using Actor Events.  This method avoids the overhead and cost of an external backplane.

## Usage

### TopicActor

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

### SignalR Setup

Register the topics services within your SignalR host service:

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

> Note the `authorise` argument required by the `TopicHub` base class.  This allows resource-based authorisation against the requested subscription.

The `TMessage` and `TSubscription` types should be serializable and live in the `TopicActor` interfaces library, as they will be shared between publisher and subscriber services.

The `TSubscription` type must implement the `ITopicId` interface, this allows an `ActorId` to be generated from your subscription contract.


That's it!  You can now use `ITopicPublisher<TMessage, TSubscription>`/`TopicActorPublisher` to publish messages from any service within your application, e.g.:


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

Any SignalR connected client subscribed to that particular topic will receive the update.  If a service doesn't have any clients subscribed to that topic, it won't receive the update.

This repository contains the source for the nuget package and also a working demo example.  Open the browser console to see the published messages being received.


---


### Notes

I've omitted the SignalR boilerplate setup and js client as that all remains the same, regardless of this library being used.  In my implementations I have ensured the client re-subscribes if the websocket connection closes.  This is because the actor proxy stored by the SignalR host service will be lost in the event of a failover, e.g. during a deployment.
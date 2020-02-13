using Microsoft.ServiceFabric.Actors;

namespace ServiceFabric.SignalR.Topics.Actors
{
    interface IActorEventsReceiverFactory<TActorEvents>
        where TActorEvents: IActorEvents
    {
        TActorEvents Create();
    }
}

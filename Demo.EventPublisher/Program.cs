using System;
using System.Diagnostics;
using System.Threading;
using Demo.TopicActor.Interfaces;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.SignalR.Topics.Actors;

namespace Demo.EventPublisher
{
    internal static class Program
    {
        private static void Main()
        {
            try
            {
                ServiceRuntime.RegisterServiceAsync("Demo.EventPublisherType",
                    context => new EventPublisher(
                        context,
                        new TopicActorPublisher<AuctionUpdate, AuctionSubscription>(new ActorProxyFactory()))
                    ).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(EventPublisher).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }
    }
}

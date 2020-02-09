using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Demo.TopicActor.Interfaces;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using ServiceFabric.SignalR.Topics;

namespace Demo.EventPublisher
{
    internal sealed class EventPublisher : StatelessService
    {
        private readonly ITopicPublisher<AuctionUpdate, AuctionSubscription> _topicPublisher;

        public EventPublisher(StatelessServiceContext context, ITopicPublisher<AuctionUpdate, AuctionSubscription> topicPublisher)
            : base(context)
        {
            _topicPublisher = topicPublisher ?? throw new ArgumentNullException(nameof(topicPublisher));
        }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[0];
        }

        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await _topicPublisher.Publish(new AuctionUpdate
                {
                    DateClosing = DateTime.Now,
                    Id = 1,
                    Name = "Test"
                },
                    new AuctionSubscription
                    {
                        Category = "Test",
                    });

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}

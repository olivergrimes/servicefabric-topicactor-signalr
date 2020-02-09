using System.Threading.Tasks;
using Demo.TopicActor.Interfaces;
using ServiceFabric.SignalR.Topics.Hubs;

namespace Demo.AspNetCoreHost.Hubs.Auction
{
    public class AuctionHub : TopicHub<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate>
    {
        public AuctionHub(ITopicClient<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate> topicClient)
            : base(topicClient, authorise: (subscription, context) =>
                {
                    return Task.FromResult(true); //Used to check resource based authorisation for requested subscription
                })
        { }
    }
}

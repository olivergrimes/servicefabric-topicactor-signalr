using ServiceFabric.SignalR.Topics.Hubs;
using System.Threading.Tasks;
using Demo.TopicActor.Interfaces;

namespace Demo.AspNetCoreHost.Hubs.Auction
{
    public class AuctionHub : TopicHub<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate>
    {
        public AuctionHub(ITopicClient<AuctionUpdate, AuctionHub, IAuctionHub> topicClient)
            : base(topicClient,
                  authoriseSubscription: (subscription, context) =>
                  {
                      return Task.FromResult(true); //Handle subscription-level auth
                  },
                  topicIdGenerator: subscription => $"NewAuctions-{subscription.Category}") //TODO: move topicId generator to shared dep for publisher
        { }
    }
}

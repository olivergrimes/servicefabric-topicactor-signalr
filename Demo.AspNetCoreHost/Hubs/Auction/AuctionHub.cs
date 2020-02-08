using ServiceFabric.SignalR.Topics.Hubs;
using System.Threading.Tasks;
using Demo.TopicActor.Interfaces;

namespace Demo.AspNetCoreHost.Hubs.Auction
{
    public class AuctionHub : TopicHub<AuctionHub, IAuctionHub, AuctionSubscription, AuctionUpdate>
    {
        public AuctionHub(ITopicClient<AuctionUpdate, AuctionHub, IAuctionHub, AuctionSubscription> topicClient)
            : base(topicClient,
                  authoriseSubscription: (subscription, context) =>
                  {
                      return Task.FromResult(true); //Handle subscription-level auth
                  })
        { }
    }
}

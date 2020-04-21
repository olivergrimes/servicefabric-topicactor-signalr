using ServiceFabric.SignalR.Topics;
using ServiceFabric.SignalR.Topics.Hubs;
using System.Threading.Tasks;

namespace Demo.AspNetCoreHost.Hubs.Bid
{
    public interface IBidHub : ITopicHub<BidUpdate>
    {

    }

    public class BidSubscription : ITopicId
    {
        public int AuctionId { get; set; }

        public string GetTopicId()
        {
            return $"NewBid-{AuctionId}";
        }
    }

    public class BidUpdate
    {
        public int AuctionId { get; set; }

        public string Bidder { get; set; }

        public decimal Amount { get; set; }
    }

    public class BidHub : TopicHub<BidHub, IBidHub, BidSubscription, BidUpdate>
    {
        public BidHub(ITopicClient<BidHub, IBidHub, BidSubscription, BidUpdate> topicClient)
             : base(topicClient, authorise: (subscription, context) =>
                 {
                     return Task.FromResult(true); //Used to check resource based authorisation for requested subscription
                 })
        { }
    }
}

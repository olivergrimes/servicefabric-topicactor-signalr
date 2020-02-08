using Demo.AspNetCoreHost.Hubs.Auction;
using Microsoft.AspNetCore.SignalR;
using ServiceFabric.SignalR.Topics.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.AspNetCoreHost.Hubs.Bid
{
    public interface IBidHub : ITopicHub<BidUpdate>
    {

    }

    public class BidSubscription
    {
        public int AuctionId { get; set; }
    }

    public class BidUpdate
    {
        public int AuctionId { get; set; }

        public string Bidder { get; set; }

        public decimal Amount { get; set; }
    }

    public class BidHub : TopicHub<BidHub, IBidHub, BidSubscription, BidUpdate>
    {
        public BidHub(ITopicClient<BidUpdate, BidHub, IBidHub> topicClient)
             : base(topicClient,
                  authoriseSubscription: (subscription, context) =>
                  {
                      return Task.FromResult(true); //Handle subscription-level auth
                  },
                  topicIdGenerator: subscription => $"NewBids-{subscription.AuctionId}")
        { }
    }
}

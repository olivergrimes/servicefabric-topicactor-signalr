using Demo.TopicActor.Interfaces;
using ServiceFabric.SignalR.Topics.Hubs;

namespace Demo.AspNetCoreHost.Hubs.Auction
{
    public interface IAuctionHub : ITopicHub<AuctionUpdate>
    { }
}

using ServiceFabric.SignalR.Topics;
using System;

namespace Demo.TopicActor.Interfaces
{
    public class AuctionSubscription : ITopicId
    {
        public string Category { get; set; }

        public string GetTopicId()
        {
            if (string.IsNullOrWhiteSpace(Category))
            {
                throw new ArgumentNullException(Category);
            }

            return $"NewAuction-{Category}";
        }
    }
}

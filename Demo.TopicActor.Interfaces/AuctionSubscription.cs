using ServiceFabric.SignalR.Topics;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Demo.TopicActor.Interfaces
{
    public class AuctionSubscription : ITopicId<AuctionSubscription>
    {
        public string Category { get; set; }

        public bool Equals([AllowNull] AuctionSubscription x, [AllowNull] AuctionSubscription y)
        {
            return x.Category.Equals(y.Category, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode([DisallowNull] AuctionSubscription obj)
        {
            return Category.GetHashCode();
        }

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

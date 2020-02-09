using System.Collections.Generic;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    internal class TopicSubscription<TSubscription>
    {
        public TSubscription Subscription { get; set; }

        public HashSet<string> ConnectionIds { get; set; }
    }
}

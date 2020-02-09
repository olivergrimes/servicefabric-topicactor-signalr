using System.Collections.Generic;

namespace ServiceFabric.SignalR.TopicHubs
{
    internal class TopicSubscription<TSubscription>
    {
        public TSubscription Subscription { get; set; }

        public HashSet<string> ConnectionIds { get; set; }
    }
}

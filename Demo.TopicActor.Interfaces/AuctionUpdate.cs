using System;

namespace Demo.TopicActor.Interfaces
{
    public class AuctionUpdate
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime DateClosing { get; set; }
    }
}

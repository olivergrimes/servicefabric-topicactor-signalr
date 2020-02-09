using System;
using System.Collections.Generic;

namespace ServiceFabric.SignalR.Topics
{
    public interface ITopicId<T> : IEqualityComparer<T>
    {
        string GetTopicId();
    }
}

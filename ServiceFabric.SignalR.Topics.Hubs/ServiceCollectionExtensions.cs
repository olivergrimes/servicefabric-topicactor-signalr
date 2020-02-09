using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceFabric.SignalR.Topics.Actors;

namespace ServiceFabric.SignalR.Topics.Hubs
{
    public static class ServiceCollectionExtensions
    {
        public static void RegisterTopics(this IServiceCollection services)
        {
            services.AddScoped<IActorProxyFactory, ActorProxyFactory>();
            services.AddSingleton(typeof(ITopicSubscriberFactory<,>), typeof(TopicActorSubscriberFactory<,>));
            services.AddSingleton(typeof(ITopicClient<,,,>), typeof(TopicClient<,,,>));
        }
    }
}

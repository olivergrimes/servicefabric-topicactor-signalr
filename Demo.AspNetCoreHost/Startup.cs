using Demo.AspNetCoreHost.Hubs.Auction;
using Demo.AspNetCoreHost.Hubs.Bid;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using ServiceFabric.SignalR.Topics;
using ServiceFabric.SignalR.Topics.Actors;
using ServiceFabric.SignalR.Topics.Hubs;

namespace Demo.AspNetCoreHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSignalR(o =>
            {
                o.EnableDetailedErrors = true;
            });

            services.AddScoped<IActorProxyFactory, ActorProxyFactory>();
            services.AddScoped(typeof(ITopicSubscriberFactory<,>), typeof(TopicActorSubscriberFactory<,>));
            services.AddSingleton(typeof(ITopicClient<,,,>), typeof(TopicClient<,,,>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<AuctionHub>("/hubs/auction");
                endpoints.MapHub<BidHub>("/hubs/bid");
            });
        }
    }
}

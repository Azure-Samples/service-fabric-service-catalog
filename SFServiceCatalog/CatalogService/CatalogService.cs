using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Collections.Generic;
using System.Fabric;
using System.IO;

namespace CatalogService
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class CatalogService : StatelessService
    {
        public CatalogService(StatelessServiceContext context)
            : base(context)
        { }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {                    
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new WebListenerCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");

                        return new WebHostBuilder().UseWebListener()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseApplicationInsights()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{

        //    var configurationPackage = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");

        //    var brokerSection = configurationPackage.Settings.Sections["DefaultBrokers"];
        //    if (brokerSection != null && brokerSection.Parameters != null)
        //    {
        //        foreach (var parm in brokerSection.Parameters)
        //        {
        //            var token = new CancellationToken();
        //            try
        //            {
        //                var catalogProxy = ActorProxy.Create<IBrokerCatalogActor>(new ActorId("default"), serviceName: "BrokerCatalogActor");
        //                await catalogProxy.AddBrokerAsync(new OSB.Broker { Name = parm.Name, Url = parm.Value, User = "haishi", Password = "P$ssword!" }, token);
        //            }
        //            catch (Exception exp)
        //            {
        //                ServiceEventSource.Current.ServiceMessage(Context, ">>>>>" + exp.Message + "---" + exp.GetType().ToString());
        //            }
        //        }
        //    }

        //}
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using BrokeredServiceActor.Interfaces;
using OSB;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BrokeredServiceActor
{
    [StatePersistence(StatePersistence.Persisted)]
    internal class BrokeredServiceActor : Actor, IBrokeredServiceActor
    {

        private IActorTimer mUpdateTimer;

        public BrokeredServiceActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override async Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            var configurationPackage = this.ActorService.Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            var configurationSection = configurationPackage.Settings.Sections["BrokerConfiguration"];
            var serviceType = configurationSection.Parameters["ServiceType"].Value;
            var servicePlan = configurationSection.Parameters["ServicePlan"].Value;
            var policy = configurationSection.Parameters["BrokerPolicy"] == null ? "ProvisionAndBind" : configurationSection.Parameters["BrokerPolicy"].Value;

            if (configurationSection.Parameters["UseCSService"] == null || bool.Parse(configurationSection.Parameters["UseCSService"].Value) == false)
            {
                var OSBEndpoint = configurationSection.Parameters["OSBEndpoint"].Value;
                var OSBUser = configurationSection.Parameters["OSBUser"].Value;
                var OSBPassword = configurationSection.Parameters["OSBPassword"].Value;
                await OSBBootstrap(OSBEndpoint, OSBUser, OSBPassword, serviceType, servicePlan);
            }
            else
            {
                var CSEndpoint = string.Format("{0}/api/entities", configurationSection.Parameters["CSEndpoint"].Value); 
                await CSBootstrap(CSEndpoint, serviceType, servicePlan, policy);
            }
        }
        private async Task CSBootstrap(string CSEndpoint, string serviceType, string servicePlan, string policy)
        {
            if (policy == "ProvisionAndBind")
            {
                var jsonString = CatalogServiceClient.AzureServiceParametersJsonBuilder.BuildStorageInstanceParameters(serviceType, servicePlan,
                                                        "osb-test-group", this.Id.GetStringId(), "eastus", "Standard_LRS");
                await CatalogServiceClient.CatalogServiceClient.CreateServiceInstanceAsync(CSEndpoint, jsonString, this.Id.GetStringId());
                mUpdateTimer = RegisterTimer(
                       CSCheckProvisionState,
                       new string[] { CSEndpoint, this.Id.GetStringId(), serviceType, servicePlan },
                       TimeSpan.FromMilliseconds(1),
                       TimeSpan.FromMilliseconds(5));
            }
            else
            {
                string bindingId = Guid.NewGuid().ToString("N").Substring(0, 8);
                var jsonString = CatalogServiceClient.AzureServiceParametersJsonBuilder.BuildStorageBindingParameters(serviceType,servicePlan, this.Id.GetStringId(), bindingId);
                await CatalogServiceClient.CatalogServiceClient.CreateBindingAsync(CSEndpoint, jsonString, bindingId);
            }
        }
        private async Task OSBBootstrap(string OSBEndpoint, string OSBUser, string OSBPassword, string serviceType, string servicePlan)
        {
            var catalog = await OSBClient.CatalogAsync(OSBEndpoint, OSBUser, OSBPassword);

            var brokeredService = catalog.FirstOrDefault(s => s.Name == serviceType || s.Id == serviceType);

            if (brokeredService != null)
            {
                var plan = brokeredService.Plans.FirstOrDefault(p => p.Name == servicePlan || p.Id == servicePlan);
                if (plan != null)
                {
                    var response = await OSBClient.ProvisionAsync(OSBEndpoint, OSBUser, OSBPassword, brokeredService.Id, plan.Id, this.Id.GetStringId());
                    mUpdateTimer = RegisterTimer(
                        OSBCheckProvisionState,
                        new string[] { OSBEndpoint, OSBUser, OSBPassword, this.Id.GetStringId(), brokeredService.Id, plan.Id },
                        TimeSpan.FromMilliseconds(1),
                        TimeSpan.FromMilliseconds(5));
                }
            }
        }
        private async Task CSCheckProvisionState(object state)
        {
            string[] parms = (string[])state;
            var response = await CatalogServiceClient.CatalogServiceClient.QueryServiceInstanceProvisionStatusAsync(parms[0], parms[1]);
            List<LastOperation> operations = null;
            try
            {
                operations = JsonConvert.DeserializeObject<List<LastOperation>>(response);
            }
            catch (Exception) //TODO: catch deserialization exception only
            {                
            }
            if (operations != null)
            {
                foreach (var operation in operations)
                {
                    if (operation.State == "succeeded")
                    {
                        string bindingId = Guid.NewGuid().ToString("N").Substring(0, 8);
                        var jsonString = CatalogServiceClient.AzureServiceParametersJsonBuilder.BuildStorageBindingParameters(parms[2], parms[3], parms[1], bindingId);
                        var bindResponse = await CatalogServiceClient.CatalogServiceClient.CreateBindingAsync(parms[0], jsonString, bindingId);
                        try
                        {
                            List<BindingwithResult> result = JsonConvert.DeserializeObject<List<BindingwithResult>>(bindResponse);
                            if (result != null && result.Count > 0)
                            {
                                await this.StateManager.SetStateAsync<BindingwithResult>("Binding", result[0]);
                                UnregisterTimer(mUpdateTimer);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
        }
        private async Task OSBCheckProvisionState(object state)
        {
            string[] parms = (string[])state;
            var response = await OSBClient.QueryServiceInstanceProvisionStatusAsync(parms[0], parms[1], parms[2], parms[3]);
            if (response.State == "succeeded")
            {
                string bindingId = Guid.NewGuid().ToString("N").Substring(0, 8);
                var bindResponse = await OSBClient.BindAsync(parms[0], parms[1], parms[2], parms[3], parms[4], parms[5], bindingId);
                await this.StateManager.SetStateAsync<BindingResult>("Binding", bindResponse);
                UnregisterTimer(mUpdateTimer);
            }
        }

        public async Task<List<Tuple<string, string>>> GetBindingCredential(CancellationToken cancellationToken)
        {
            List<Tuple<string, string>> ret = new List<Tuple<string, string>>();
            var containsState = await this.StateManager.ContainsStateAsync("Binding");
            if (containsState)
            {
                var state = await this.StateManager.GetStateAsync<BindingwithResult>("Binding", cancellationToken);
                if (state != null && state.Result !=null)
                {
                    if (!string.IsNullOrEmpty(state.Result.Credentials))
                    {
                        JObject credentialObject = JsonConvert.DeserializeObject(state.Result.Credentials) as JObject;
                        foreach (var property in credentialObject.Properties())
                        {
                            ret.Add(new Tuple<string, string>(property.Name, property.Value.ToString()));
                        }
                    }
                }
            }
            return ret;
        }


        protected override Task OnDeactivateAsync()
        {
            if (mUpdateTimer != null)
            {
                UnregisterTimer(mUpdateTimer);
            }
            return base.OnDeactivateAsync();
        }
    }
}

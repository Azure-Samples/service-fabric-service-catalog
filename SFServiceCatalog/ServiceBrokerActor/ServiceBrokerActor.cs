using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using ServiceBrokerActor.Interfaces;
using OSB;
using sfcat;
using sfcat.OSBCommands;

namespace ServiceBrokerActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class ServiceBrokerActor : Actor, IServiceBrokerActor
    {
        public ServiceBrokerActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");
            return this.StateManager.TryAddStateAsync("count", 0);
        }

        public async Task<bool> Connect(string serviceId, string planId, string instanceId, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync<string>("ServiceId", serviceId);
            await this.StateManager.SetStateAsync<string>("PlanId", planId);
            await this.StateManager.SetStateAsync<string>("InstanceId", instanceId);
            CreateServiceInstanceCommand command = new CreateServiceInstanceCommand(new Dictionary<string, string>
            {
                {"id", instanceId },
                {"f", string.Format(@"{
                        ""service_id"": ""{0}"",
                        ""plan_id"": ""{1}"",
                        ""parameters"": {
                            ""resourceGroup"": ""osb-test-group"",
                            ""storageAccountName"": ""cat01"",
                            ""location"": ""eastus"",
                            ""accountType"": ""Standard_LRS""
                        }
                        }", serviceId, planId)}
            }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
            command.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
            var conclusion = command.Run();
            return conclusion is SuccessConclusion;
        }

        public async Task<BindingwithResult> GetBinding(string bindingId, CancellationToken cancellationToken)
        {
            var instanceId = await this.StateManager.GetStateAsync<string>("InstanceId", cancellationToken);
            if (string.IsNullOrEmpty(bindingId))
                bindingId = Guid.NewGuid().ToString("N");
            var binding = await this.StateManager.TryGetStateAsync<BindingwithResult>(bindingId, cancellationToken);
            if (binding.HasValue)
            {
                return binding.Value;
            } else
            {
                CreateBindingCommand command = new CreateBindingCommand(new Dictionary<string, string>
                {
                    {"instance-id", instanceId},
                    {"id", bindingId },
                    {"f", string.Format(@"{
                            ""service_id"": ""{0}"",
                            ""plan_id"": ""{1}""
                            }", "A","B")
                    }
                }, new ServiceInstanceSchemaChecker("http://localhost:8088"));
                ListEntitiesCommand<BindingwithResult> listCommand = new ListEntitiesCommand<BindingwithResult>("binding");
                listCommand.InjectSwitch("CatalogServiceEndpoint", "http://localhost:8088");
                var conclusion = listCommand.Run();
                var bindings = ((MultiOutputConclusion)conclusion).GetObjectList<BindingwithResult>();
                if (bindings.Count > 0)
                {
                    await this.StateManager.SetStateAsync<BindingwithResult>(bindingId, bindings[0], cancellationToken);
                    return bindings[0];
                }
                else
                    return null;
            }
        }
    }
}

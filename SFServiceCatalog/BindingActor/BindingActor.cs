using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using BindingActor.Interfaces;
using OSB;

namespace BindingActor
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
    internal class BindingActor : Actor, IBindingActor
    {
        /// <summary>
        /// Initializes a new instance of BindingActor
        /// </summary>
        /// <param name="actorService">The Microsoft.ServiceFabric.Actors.Runtime.ActorService that will host this actor instance.</param>
        /// <param name="actorId">The Microsoft.ServiceFabric.Actors.ActorId for this actor instance.</param>
        public BindingActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        /// <summary>
        /// This method is called whenever an actor is activated.
        /// An actor is activated the first time any of its methods are invoked.
        /// </summary>
        protected override Task OnActivateAsync()
        {
            ActorEventSource.Current.ActorMessage(this, "Actor activated.");

            // The StateManager is this actor's private state store.
            // Data stored in the StateManager will be replicated for high-availability for actors that use volatile or persisted state storage.
            // Any serializable object can be saved in the StateManager.
            // For more information, see https://aka.ms/servicefabricactorsstateserialization

            return Task.FromResult<bool>(true);
        }

        public async Task<BindingwithResult> OSBBindAsync(string instanceId, string serviceId, string planId, string bindingId, CancellationToken cancellationToken)
        {
            var bindingResult = await OSBClient.BindAsync("http://104.45.224.242", "haishi", "P$ssword!", instanceId, serviceId, planId, bindingId);
            var bindingWithResult = new BindingwithResult
            {
                Binding = new Binding
                {
                    BindingId = bindingId,
                    InstanceId = instanceId,
                    ServiceId = serviceId,
                    PlanId = planId
                }
                    ,
                Result = bindingResult
            };
            await this.StateManager.SetStateAsync<BindingwithResult>("bindingResult", bindingWithResult, cancellationToken);
            return bindingWithResult;
        }
        public async Task OSBUnbindAsync(CancellationToken cancellationToken)
        {
            var state = await this.StateManager.GetStateAsync<BindingwithResult>("bindingResult", cancellationToken);
            await OSBClient.UnbindAsync("http://104.45.224.242", "haishi", "P$ssword!", state.Binding.InstanceId, state.Binding.ServiceId, state.Binding.PlanId, state.Binding.BindingId);
        }
    }
}

using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;
using SFServiceCatalog.SmartEntity.Interfaces;
using System.IO;

namespace SFServiceCatalog.SmartEntity
{
    public abstract class SmartEntityActor : Actor, ISmartEntity
    {
        private IEntityHandler mHandler;
        private const string SwitchesKey = "mySwitches";
        private const string IdKey = "myId";
        private const string MyState = "myState";
        private const string MyInstance = "myInstance";
        private const string HandlerAssembly = "HandlerAssembly";
        private const string HandlerType = "HandlerType";
        

        private IActorTimer updateTimer;

        protected override Task OnActivateAsync()
        {
            return base.OnActivateAsync();
        }
        public SmartEntityActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {           
        }

        private async Task WatchStatusAsync(object state)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>(SwitchesKey);
            ensureHandler(switches);
            if (mHandler == null)
                UnregisterTimer(updateTimer);
            else
            {
                bool hasState = await this.StateManager.ContainsStateAsync(IdKey);
                if (hasState)
                {
                    var id = await this.StateManager.GetStateAsync<string>(IdKey);
                    var status = await mHandler.OnWatch(id, switches, state);
                    if (status.Succeeded && !string.IsNullOrEmpty(status.UpdatedState))
                        await this.StateManager.SetStateAsync<string>(MyState, status.UpdatedState);
                }
            }
        }

        private void ensureHandler(Dictionary<string,string> switches)
        {
            if (mHandler == null)
            {
                if (switches != null && switches.ContainsKey(HandlerAssembly))
                {
                    var handlerPackage = this.ActorService.Context.CodePackageActivationContext.GetDataPackageObject("Handlers");
                    var handlerPath = Path.Combine(handlerPackage.Path, switches[HandlerAssembly]);
                    var assembly = Assembly.LoadFrom(handlerPath);
                    mHandler = (IEntityHandler)assembly.CreateInstance(switches[HandlerType]);
                }
            }
        }

        public async Task SetSwitchesAsync(Dictionary<string, string> switches, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync<Dictionary<string, string>>(SwitchesKey, switches, cancellationToken);
        }
        public virtual async Task<string> OnCreated(string id, string json, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync<string>(MyInstance, json, cancellationToken);
            await this.StateManager.SetStateAsync<string>(IdKey, id, cancellationToken);
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>(SwitchesKey);
            ensureHandler(switches);
            var extendedState = await mHandler.OnCreated(id, json, switches);
            if (extendedState.Succeeded == true)
            {
                if (!string.IsNullOrEmpty(extendedState.UpdatedState))
                    await this.StateManager.SetStateAsync<string>(MyState, extendedState.UpdatedState, cancellationToken);
                if (!string.IsNullOrEmpty(extendedState.UpdatedEntity))
                    await this.StateManager.SetStateAsync<string>(MyInstance, extendedState.UpdatedEntity, cancellationToken);
                updateTimer = RegisterTimer(
                  WatchStatusAsync,               // Callback method
                  extendedState.UpdatedState,          // Parameter to pass to the callback method
                  TimeSpan.FromSeconds(3),        // Amount of time to delay before the callback is invoked
                  TimeSpan.FromSeconds(3));       // Time interval between invocations of the callback method
            }
            return extendedState.UpdatedEntity;

        }

        public virtual async Task<string> OnUpdated(string id, string json, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync<string>(MyInstance, json, cancellationToken);
            await this.StateManager.SetStateAsync<string>(IdKey, id, cancellationToken);
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>(SwitchesKey);
            ensureHandler(switches);
            var extendedState = await mHandler.OnUpdated(id, json, switches);
            if (extendedState.Succeeded == true)
            {
                if (!string.IsNullOrEmpty(extendedState.UpdatedState))
                    await this.StateManager.SetStateAsync<string>(MyState, extendedState.UpdatedState, cancellationToken);
                if (!string.IsNullOrEmpty(extendedState.UpdatedEntity))
                    await this.StateManager.SetStateAsync<string>(MyInstance, extendedState.UpdatedEntity, cancellationToken);              
            }
            return extendedState.UpdatedEntity;
        }

        public async Task<string> GetStatusAsync(CancellationToken cancellationToken)
        {
            bool hasState = await this.StateManager.ContainsStateAsync(MyState, cancellationToken);
            if (hasState)
                return await this.StateManager.GetStateAsync<string>(MyState);
            else
                return "OK";
        }
        
        protected override Task OnDeactivateAsync()
        {
            if (updateTimer != null)
            {
                UnregisterTimer(updateTimer);
            }

            return base.OnDeactivateAsync();
        }

        public async Task<string> OnDeleted(string id, CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>(SwitchesKey, cancellationToken);
            ensureHandler(switches);
            if (mHandler != null)
            {
                var entity = await this.StateManager.GetStateAsync<string>(MyInstance, cancellationToken);
                await mHandler.OnDeleted(id, entity, switches);
            }
            await this.StateManager.RemoveStateAsync(MyInstance, cancellationToken);
            if (updateTimer != null)
            {
                UnregisterTimer(updateTimer);
            }
            return null;
        }
    }
}

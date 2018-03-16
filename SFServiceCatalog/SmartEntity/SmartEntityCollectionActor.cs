using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.ServiceFabric.Actors.Client;
using SFServiceCatalog.SmartEntity.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace SFServiceCatalog.SmartEntity
{
    public abstract class SmartEntityCollectionActor: Actor, ISmartEntityCollection
    {
        public SmartEntityCollectionActor(ActorService actorService, ActorId actorId):base(actorService, actorId)
        {
            
        }

        public async Task SetSwitchesAsync(Dictionary<string, string> switches, CancellationToken cancellationToken)
        {
            await this.StateManager.SetStateAsync<Dictionary<string, string>>("mySwitches", switches, cancellationToken);
        }
        public async Task AddEntityAsync(string id, string entity, string idField, CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>("mySwitches", cancellationToken);
            if (switches.ContainsKey("EntityServiceType") && !string.IsNullOrEmpty(switches["EntityServiceType"]))
            {
                await this.StateManager.AddOrUpdateStateAsync<string>(id, entity, (key, value) => entity, cancellationToken);
                var entityProxy = ActorProxy.Create<ISmartEntity>(new ActorId(this.Id.ToString() + "-" + id), serviceName: switches["EntityServiceType"]);
                await entityProxy.SetSwitchesAsync(switches, cancellationToken);
                string updatedEntity = await entityProxy.OnCreated(id, entity, cancellationToken);
                if (!string.IsNullOrEmpty(updatedEntity)) //This is a special case - when the entity is updated by the handler
                    await this.StateManager.AddOrUpdateStateAsync<string>(id, updatedEntity, (key, value) => updatedEntity, cancellationToken);
            }
            else
            {
                var list = JArray.Parse(entity);
                foreach (var item in list.Where(t=>t[idField] != null))
                {
                    await this.StateManager.SetStateAsync<string>(item[idField].ToString(), JsonConvert.SerializeObject(item), cancellationToken);
                }
            }
        }

        public async Task UpdateEntityAsync(string id, string entity, string idField, CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>("mySwitches", cancellationToken);
            if (switches.ContainsKey("EntityServiceType") && !string.IsNullOrEmpty(switches["EntityServiceType"]))
            {
                await this.StateManager.AddOrUpdateStateAsync<string>(id, entity, (key, value) => entity, cancellationToken);
                var entityProxy = ActorProxy.Create<ISmartEntity>(new ActorId(this.Id.ToString() + "-" + id), serviceName: switches["EntityServiceType"]);
                await entityProxy.SetSwitchesAsync(switches, cancellationToken);
                string updatedEntity = await entityProxy.OnUpdated(id, entity, cancellationToken);
                if (!string.IsNullOrEmpty(updatedEntity)) //This is a special case - when the entity is updated by the handler
                    await this.StateManager.AddOrUpdateStateAsync<string>(id, updatedEntity, (key, value) => updatedEntity, cancellationToken);
            }
            else
            {
                var list = JArray.Parse(entity);
                foreach (var item in list.Where(t => t[idField] != null))
                {
                    await this.StateManager.SetStateAsync<string>(item[idField].ToString(), JsonConvert.SerializeObject(item), cancellationToken);
                }
            }
        }
        public async Task<List<string>> ListEntitiesAsync(CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>("mySwitches", cancellationToken);

            var ret = new List<string>();
            var entities = await this.StateManager.GetStateNamesAsync();
            foreach (var entity in entities)
            {
                if (entity != "mySwitches")
                {
                    var item = await StateManager.GetStateAsync<string>(entity);
                        ret.Add(item);
                }
            }
            return ret;
        }
        public async Task<string> GetEntityAsync(string id, CancellationToken cancellationToken)
        {
            bool hasKey = await this.StateManager.ContainsStateAsync(id, cancellationToken);
            if (hasKey)
                return await this.StateManager.GetStateAsync<string>(id, cancellationToken);
            else
                return null;
        }

        public async Task<string> GetEntityStatusAsync(string id, CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>("mySwitches", cancellationToken);

            if (switches.ContainsKey("EntityServiceType") && !string.IsNullOrEmpty(switches["EntityServiceType"]))
            {
                var entityProxy = ActorProxy.Create<ISmartEntity>(new ActorId(this.Id.ToString() + "-" + id), serviceName: switches["EntityServiceType"]);
                await entityProxy.SetSwitchesAsync(switches, cancellationToken);
                return await entityProxy.GetStatusAsync(cancellationToken);
            }
            else
                return "OK";
        }

        public async Task DeleteEntityAsync(string id, CancellationToken cancellationToken)
        {
            var switches = await this.StateManager.GetStateAsync<Dictionary<string, string>>("mySwitches", cancellationToken);
            if (switches.ContainsKey("EntityServiceType") && !string.IsNullOrEmpty(switches["EntityServiceType"]))
            {
                var entityProxy = ActorProxy.Create<ISmartEntity>(new ActorId(this.Id.ToString() + "-" + id), serviceName: switches["EntityServiceType"]);
                await entityProxy.SetSwitchesAsync(switches, cancellationToken);
                await entityProxy.OnDeleted(id, cancellationToken);                
            }
            await this.StateManager.RemoveStateAsync(id, cancellationToken);
        }
    }
}

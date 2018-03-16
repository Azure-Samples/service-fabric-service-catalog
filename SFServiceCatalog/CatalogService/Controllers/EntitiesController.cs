using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFServiceCatalog.SmartEntity.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CatalogService.Controllers
{
    [Route("api/[controller]")]
    public class EntitiesController : Controller
    {
        private readonly IOptions<CSSettings> myConfig;

        public EntitiesController(IOptions<CSSettings> config)
        {
            myConfig = config;
        }
        
        // GET api/values/5
        [HttpGet("{entity_type}/{entity_id?}")]
        public async Task<IActionResult> Get(string entity_type, string entity_id = "", bool watch = false)
        {
            var token = default(CancellationToken);
            if (entity_type == "sfcatconfig")
            {
                return Ok(myConfig.Value.ClientSettings);
            }
            if (myConfig.Value.SupportedTypes.ContainsKey(entity_type))
            {
                var typeSettings = myConfig.Value.SupportedTypes[entity_type];
                var catalogProxy = ActorProxy.Create<ISmartEntityCollection>(new ActorId("default-" + entity_type), serviceName: typeSettings.CatalogServiceType);
                var switches = myConfig.Value.ActorSwitches[typeSettings.CatalogServiceType];
                fillSwitches(switches, typeSettings);
                await catalogProxy.SetSwitchesAsync(switches, token);
                if (string.IsNullOrEmpty(entity_id))
                    return Ok(await catalogProxy.ListEntitiesAsync(token));
                else
                {
                    if (!watch)
                    {
                        string entity = await catalogProxy.GetEntityAsync(entity_id, token);
                        if (!string.IsNullOrEmpty(entity))
                            return Ok(await catalogProxy.GetEntityAsync(entity_id, token));
                        else
                            return NotFound(entity_id);
                    }
                    else
                        return Ok(await catalogProxy.GetEntityStatusAsync(entity_id, token));
                }
            }
            else
                return BadRequest();
        }

        // POST api/values
        [HttpPut("{entity_type}/{entity_id?}")]
        public async Task<IActionResult> Put(string entity_type, string entity_id, [FromBody]object entity, string idField="")
        {
            var token = default(CancellationToken);

            if (myConfig.Value.SupportedTypes.ContainsKey(entity_type))
            {
                var typeSettings = myConfig.Value.SupportedTypes[entity_type];

                string serializedEntity = JsonConvert.SerializeObject(entity);
                if (!string.IsNullOrEmpty(typeSettings.SerializedProperties))
                {
                    var tempObject = JObject.Parse(serializedEntity);
                    var parms = typeSettings.SerializedProperties.Split(';');
                    foreach (var parm in parms)
                    {
                        tempObject[parm] = JsonConvert.SerializeObject(tempObject[parm]);
                    }
                    serializedEntity = JsonConvert.SerializeObject(tempObject);
                }

                var catalogProxy = ActorProxy.Create<ISmartEntityCollection>(new ActorId("default-" + entity_type), serviceName: typeSettings.CatalogServiceType);
                var switches = myConfig.Value.ActorSwitches[typeSettings.CatalogServiceType];
                fillSwitches(switches, typeSettings);
                await catalogProxy.SetSwitchesAsync(switches, token);
                await catalogProxy.AddEntityAsync(entity_id, serializedEntity, idField, token);
                return Created("/entities/" + entity_type + "/" + entity_id, serializedEntity);
            }
            else
                return BadRequest();
        }

        private void fillSwitches(Dictionary<string,string> switches, ActorSettings settings)
        {
            if (!switches.ContainsKey("EntityServiceType"))
                switches.Add("EntityServiceType", settings.EntityServiceType);
            else
                switches["EntityServiceType"] = settings.EntityServiceType;
            if (!switches.ContainsKey("HandlerAssembly"))
                switches.Add("HandlerAssembly", settings.HandlerAssembly);
            else
                switches["HandlerAssembly"] = settings.HandlerAssembly;
            if (!switches.ContainsKey("HandlerType"))
                switches.Add("HandlerType", settings.HandlerType);
            else
                switches["HandlerType"] = settings.HandlerType;
            if (!switches.ContainsKey("SerializedProperties"))
                switches.Add("SerializedProperties", settings.SerializedProperties);
            else
                switches["SerializedProperties"] = settings.SerializedProperties;
            if (!switches.ContainsKey("CSEndpoint"))
                switches.Add("CSEndpoint", "http://localhost:8088");
        }
        // PUT api/values/5
        [HttpPost("{entity_type}/{entity_id}")]
        public async Task<IActionResult> Post(string entity_type, string entity_id, [FromBody]object entity, string idField = "")
        {
            var token = default(CancellationToken);

            if (myConfig.Value.SupportedTypes.ContainsKey(entity_type))
            {
                var typeSettings = myConfig.Value.SupportedTypes[entity_type];

                string serializedEntity = JsonConvert.SerializeObject(entity);
                if (!string.IsNullOrEmpty(typeSettings.SerializedProperties))
                {
                    var tempObject = JObject.Parse(serializedEntity);
                    var parms = typeSettings.SerializedProperties.Split(';');
                    foreach (var parm in parms)
                    {
                        tempObject[parm] = JsonConvert.SerializeObject(tempObject[parm]);
                    }
                    serializedEntity = JsonConvert.SerializeObject(tempObject);
                }

                var catalogProxy = ActorProxy.Create<ISmartEntityCollection>(new ActorId("default-" + entity_type), serviceName: typeSettings.CatalogServiceType);
                var switches = myConfig.Value.ActorSwitches[typeSettings.CatalogServiceType];
                fillSwitches(switches, typeSettings);
                await catalogProxy.SetSwitchesAsync(switches, token);
                await catalogProxy.UpdateEntityAsync(entity_id, serializedEntity, idField, token);
                return Ok();
            }
            else
                return BadRequest();
        }

        // DELETE api/values/5
        [HttpDelete("{entity_type}/{entity_id}")]
        public async Task<IActionResult> Delete(string entity_type, string entity_id)
        {           
            var token = default(CancellationToken);

            if (myConfig.Value.SupportedTypes.ContainsKey(entity_type))
            {
                var typeSettings = myConfig.Value.SupportedTypes[entity_type];

                var catalogProxy = ActorProxy.Create<ISmartEntityCollection>(new ActorId("default-" + entity_type), serviceName: typeSettings.CatalogServiceType);
                var switches = myConfig.Value.ActorSwitches[typeSettings.CatalogServiceType];
                fillSwitches(switches, typeSettings);
                await catalogProxy.SetSwitchesAsync(switches, token);
                await catalogProxy.DeleteEntityAsync(entity_id, token);
                return NoContent();
            }
            else
                return BadRequest();

        }
    }
}

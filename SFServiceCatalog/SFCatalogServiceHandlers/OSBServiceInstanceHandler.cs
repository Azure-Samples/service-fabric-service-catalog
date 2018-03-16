using Microsoft.ServiceFabric.Actors.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSB;
using SFServiceCatalog.SmartEntity.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFCatalogServiceHandlers
{
    public class OSBServiceInstanceHandler : IEntityHandler
    {
        public async Task<HandlerResult> OnCreated(string id, string entityJson, Dictionary<string, string> switches)
        {
            try
            {
                var instance = JsonConvert.DeserializeObject<ServiceInstance>(entityJson);
                instance.InstanceId = id;

                var services = await OSBClient.CatalogAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"]);
                var service = services.FirstOrDefault(s => s.Id == instance.ServiceId);
                if (service == null)
                    return new HandlerResult
                    {
                        Succeeded = false,
                        ErrorMessage = string.Format("Service Id {0} is not found.", instance.ServiceId)
                    };
                var plan = service.Plans.FirstOrDefault(p => p.Id == instance.PlanId);
                if (plan == null)
                {
                    return new HandlerResult
                    {
                        Succeeded = false,
                        ErrorMessage = string.Format("Plan Id {0} is not found.", instance.PlanId)
                    };
                }                
                
                var response = await OSBClient.ProvisionAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"],
                        instance.ServiceId, instance.PlanId, id, instance.Parameters);
                if (response.StatusCode == System.Net.HttpStatusCode.OK
                    || response.StatusCode == System.Net.HttpStatusCode.Created
                    || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return new HandlerResult
                    {
                        Succeeded = true,
                        UpdatedEntity = JsonConvert.SerializeObject(instance), 
                        UpdatedState = await response.Content.ReadAsStringAsync(),
                        HTTPStatus = response.StatusCode,
                        ErrorMessage = null
                    };
                }
                else
                {
                    return new HandlerResult
                    {
                        Succeeded = false,
                        HTTPStatus = response.StatusCode,
                        //UpdatedEntity = await response.Content.ReadAsStringAsync(),
                        ErrorMessage = response.ReasonPhrase
                    };
                }
            }
            catch (Exception exp)
            {
                return new HandlerResult
                {
                    Succeeded = false,
                    ErrorMessage = exp.Message,
                    HTTPStatus = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<HandlerResult> OnDeleted(string id, string entityJosn, Dictionary<string, string> switches)
        {
            try
            {
                var state = JsonConvert.DeserializeObject<ServiceInstance>(entityJosn);
                var response = await OSBClient.Deprovisioning(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"], state.InstanceId, state.ServiceId, state.PlanId);
                return new HandlerResult
                {
                    Succeeded = response.IsSuccessStatusCode,
                    ErrorMessage = response.ReasonPhrase,
                    HTTPStatus = response.StatusCode,
                    UpdatedEntity = null
                };
            }
            catch (Exception exp)
            {
                return new HandlerResult
                {
                    Succeeded = false,
                    ErrorMessage = exp.Message,
                    HTTPStatus = System.Net.HttpStatusCode.InternalServerError,
                    UpdatedEntity = null
                };
            }
        }

        public async Task<HandlerResult> OnUpdated(string id, string newEntityJson, Dictionary<string, string> switches)
        {
            try
            {
                var instance = JsonConvert.DeserializeObject<ServiceInstance>(newEntityJson);
                instance.InstanceId = id;

                var services = await OSBClient.CatalogAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"]);
                var service = services.FirstOrDefault(s => s.Id == instance.ServiceId);
                if (service == null)
                    return new HandlerResult
                    {
                        Succeeded = false,
                        ErrorMessage = string.Format("Service Id {0} is not found.", instance.ServiceId)
                    };
                var plan = service.Plans.FirstOrDefault(p => p.Id == instance.PlanId);
                if (plan == null)
                {
                    return new HandlerResult
                    {
                        Succeeded = false,
                        ErrorMessage = string.Format("Plan Id {0} is not found.", instance.PlanId)
                    };
                }

                var response = await OSBClient.UpdateServiceInstanceAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"],
                        instance.ServiceId, instance.PlanId, id);
                if (response.StatusCode == System.Net.HttpStatusCode.OK
                    || response.StatusCode == System.Net.HttpStatusCode.Created
                    || response.StatusCode == System.Net.HttpStatusCode.Accepted)
                {
                    return new HandlerResult
                    {
                        Succeeded = true,
                        UpdatedEntity = JsonConvert.SerializeObject(instance),
                        UpdatedState = await response.Content.ReadAsStringAsync(),
                        HTTPStatus = response.StatusCode,
                        ErrorMessage = null
                    };
                }
                else
                {
                    return new HandlerResult
                    {
                        Succeeded = false,
                        HTTPStatus = response.StatusCode,
                        //UpdatedEntity = await response.Content.ReadAsStringAsync(),
                        ErrorMessage = response.ReasonPhrase
                    };
                }
            }
            catch (Exception exp)
            {
                return new HandlerResult
                {
                    Succeeded = false,
                    ErrorMessage = exp.Message,
                    HTTPStatus = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<HandlerResult> OnWatch(string id, Dictionary<string, string> switches, object context)
        {
            try
            {
                string operation = "";
                if (context != null)
                {
                    var jObject = (JObject)JsonConvert.DeserializeObject(context.ToString());
                    if (jObject.Property("operation") != null)
                        operation = jObject.Property("operation").Value.ToString();
                }
                var response = await OSBClient.QueryServiceInstanceProvisionStatusAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"], id, operation);
                return new HandlerResult
                {
                    Succeeded = response.IsSuccessStatusCode,
                    UpdatedState = await response.Content.ReadAsStringAsync(),
                    HTTPStatus = response.StatusCode,
                    ErrorMessage = response.ReasonPhrase
                };
            }
            catch (Exception exp)
            {
                return new HandlerResult
                {
                    Succeeded = false,
                    ErrorMessage = exp.Message,
                    HTTPStatus = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }
    }
}

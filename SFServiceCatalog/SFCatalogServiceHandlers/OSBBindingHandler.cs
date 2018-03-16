using Microsoft.ServiceFabric.Actors.Runtime;
using Newtonsoft.Json;
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
    public class OSBBindingHandler : IEntityHandler
    {
        public async Task<HandlerResult> OnCreated(string id, string entityJson, Dictionary<string, string> switches)
        {
            try
            {
                var binding = JsonConvert.DeserializeObject<Binding>(entityJson);
                var bindingResult = await OSBClient.BindAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"], binding.InstanceId, binding.ServiceId, binding.PlanId, id);
                var bindingWithResult = new BindingwithResult
                {
                    Binding = new Binding
                    {
                        BindingId = id,
                        InstanceId = binding.InstanceId,
                        ServiceId = binding.ServiceId,
                        PlanId = binding.PlanId
                    }
                    ,
                    Result = bindingResult
                };
                return new HandlerResult
                {
                    Succeeded = true,
                    ErrorMessage = null,
                    HTTPStatus = System.Net.HttpStatusCode.OK,
                    UpdatedEntity = JsonConvert.SerializeObject(bindingWithResult)
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

        public async Task<HandlerResult> OnDeleted(string id, string entityJosn, Dictionary<string, string> switches)
        {
            try
            {
                var state = JsonConvert.DeserializeObject<BindingwithResult>(entityJosn);
                var response = await OSBClient.UnbindAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"], state.Binding.InstanceId, state.Binding.ServiceId, state.Binding.PlanId, state.Binding.BindingId);
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

        public Task<HandlerResult> OnUpdated(string id, string newEntityJson, Dictionary<string, string> switches)
        {
            throw new NotImplementedException();
        }

        public Task<HandlerResult> OnWatch(string id, Dictionary<string, string> switches, object context)
        {
            return Task.FromResult<HandlerResult>(
                new HandlerResult
                {
                    Succeeded = true,
                    UpdatedEntity = null
                });
        }
    }
}

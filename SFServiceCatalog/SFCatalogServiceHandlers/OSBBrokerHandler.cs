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
    public class OSBBrokerHandler : IEntityHandler
    {

        public async Task<HandlerResult> OnCreated(string id, string entityJson, Dictionary<string, string> switches)
        {
            try
            {
                var services = await OSBClient.CatalogAsync(switches["OSBEndpoint"], switches["OSBUser"], switches["OSBPassword"]);
                var response = await CatalogServiceClient.CatalogServiceClient.PutEntityAsync(switches["CSEndpoint"], "service-class", id, JsonConvert.SerializeObject(services), "id");
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

        public Task<HandlerResult> OnDeleted(string id, string entityJosn, Dictionary<string, string> switches)
        {
            throw new NotImplementedException();
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class OSBClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task PostEntityAsync(string url, string entityType, object entity)
        {
            var content = new FormUrlEncodedContent(null);
            await httpClient.PostAsync(string.Format("{0}/{1}", url, entityType), content);
        }
        public static async Task<List<Service>> CatalogAsync(string url, string user, string password)
        {
            setAuthHeader(user, password);
            var response = await httpClient.GetAsync(string.Format("{0}/v2/catalog", url));
            response.EnsureSuccessStatusCode();
            var strResponse = await response.Content.ReadAsStringAsync();
            var ret = JObject.Parse(strResponse);            
            foreach(var item in ret["services"])
            {
                item["metadata"] = JsonConvert.SerializeObject(item["metadata"]);
                foreach(var plan in item["plans"])
                {
                    plan["metadata"] = JsonConvert.SerializeObject(plan["metadata"]);
                }
            }
            var str = ret["services"].ToString();
            return JsonConvert.DeserializeObject<List<Service>>(str);
        }        
        public static async Task<HttpResponseMessage> QueryServiceInstanceProvisionStatusAsync(string url, string user, string password, string id, string operation)
        {
            setAuthHeader(user, password);
            return await httpClient.GetAsync(string.Format("{0}/v2/service_instances/{1}/last_operation{2}", url, id, 
                string.IsNullOrEmpty(operation)? "": "?operation=" + operation));
            //if (response.StatusCode == System.Net.HttpStatusCode.OK)
            //{
            //    string body = await response.Content.ReadAsStringAsync();
            //    return JsonConvert.DeserializeObject<LastOperation>(body);
            //}
            //else
            //{
            //    return new LastOperation { State = response.StatusCode.ToString(), Description = response.ReasonPhrase };
            //}
        }
        public static async Task<HttpResponseMessage> ProvisionAsync(string url, string user, string password, string service_id, string plan_id, string instance_id, string parametersJson)
        {
            setAuthHeader(user, password);
            var content = new StringContent(string.Format(@"{{
                                                    ""service_id"":""{0}"",
                                                    ""plan_id"":""{1}"",
                                                    ""organization_guid"":""00000000-0000-0000-0000-000000000000"",
                                                    ""space_guid"":""00000000-0000-0000-0000-000000000000"",
                                                    ""parameters"": {2}
                                              }}", service_id, plan_id, parametersJson), Encoding.UTF8, "application/json");
            return await httpClient.PutAsync(string.Format("{0}/v2/service_instances/" + instance_id + "?accepts_incomplete=true", url), content);
        }
        public static async Task<HttpResponseMessage> UpdateServiceInstanceAsync(string url, string user, string password, string service_id, string plan_id, string instance_id)
        {
            setAuthHeader(user, password);
            var content = new StringContent(string.Format(@"{{
                                                    ""service_id"":""{0}"",
                                                    ""plan_id"":""{1}""
                                              }}", service_id, plan_id), Encoding.UTF8, "application/json");
            var method = new HttpMethod("PATCH");
            var request = new HttpRequestMessage(method, string.Format("{0}/v2/service_instances/" + instance_id + "?accepts_incomplete=true", url)){
                Content = content
            };
            return await httpClient.SendAsync(request);
        }
        public static async Task<BindingResult> BindAsync(string url, string user, string password, string instanceId, string serviceId, string planId, string bindingId)
        {
            setAuthHeader(user, password);
            var content = new StringContent(string.Format(@"{{
                                                    ""service_id"":""{0}"",
                                                    ""plan_id"":""{1}""
                                                    }}", serviceId, planId), Encoding.UTF8, "application/json");
            var result = await httpClient.PutAsync(string.Format("{0}/v2/service_instances/" + instanceId + "/service_bindings/" + bindingId + "?accepts_incomplete=true", url), content);
            string strBody = await result.Content.ReadAsStringAsync();
            var ret = JObject.Parse(strBody);
            if (ret["credentials"]!=null)
                ret["credentials"] = JsonConvert.SerializeObject(ret["credentials"]);
            if (ret["volumeMounts"] != null)
                ret["volumeMounts"] = JsonConvert.SerializeObject(ret["volumeMounts"]);
            return JsonConvert.DeserializeObject<BindingResult>(ret.ToString());
        }
        public static async Task<HttpResponseMessage> UnbindAsync(string url, string user, string password, string instanceId, string serviceId, string planId, string bindingId)
        {
            return await httpClient.DeleteAsync(string.Format("{0}/v2/service_instances/{1}/service_bindings/{2}?service_id={3}&plan_id={4}", url, instanceId, bindingId, serviceId, planId));
        }
        public static async Task<HttpResponseMessage> Deprovisioning(string url, string user, string password, string instanceId, string serviceId, string planId)
        {
            setAuthHeader(user, password);
            return await httpClient.DeleteAsync(string.Format("{0}/v2/service_instances/{1}?service_id={2}&plan_id={3}&accepts_incomplete=true", url, instanceId, serviceId, planId));
        }
        private static void setAuthHeader(string user,string password)
        {
            var byteArray = Encoding.ASCII.GetBytes(user + ":" + password);
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Broker-API-Version", "2.13");
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CatalogServiceClient
{
    public static class CatalogServiceClient
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async Task<ClientSettings> GetClientSettings(string url)
        {
            var httpResponse = await httpClient.GetAsync(string.Format("{0}/api/entities/sfcatconfig", url));
            if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;
            var strResponse = await httpResponse.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(strResponse))
                return null;
            else
                return JsonConvert.DeserializeObject<ClientSettings>(strResponse);
        }
        public static async Task<HttpResponseMessage> PutEntityAsync(string url, string entityType, string id, string json, string idField="")
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await httpClient.PutAsync(string.Format("{0}/api/entities/{1}/{2}?idField={3}", url, entityType, id, idField), content);
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException(exp.Message, exp);
            }
        }
        public static async Task<HttpResponseMessage> PostEntityAsync(string url, string entityType, string id, string json, string idField = "")
        {
            try
            {
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                return await httpClient.PostAsync(string.Format("{0}/api/entities/{1}/{2}?idField={3}", url, entityType, id, idField), content);
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException(exp.Message, exp);
            }
        }
        public static async Task<List<T>> GetEntitiesAsync<T>(string url, string entityType, string entityKey = "")
        {
            try
            {
                var httpResponse = await httpClient.GetAsync(string.Format("{0}/api/entities/{1}{2}", url, entityType, string.IsNullOrEmpty(entityKey) ? "" : ("/" + entityKey)));
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return null;
                var strResponse = await httpResponse.Content.ReadAsStringAsync();
                var ret = new List<T>();
                if (string.IsNullOrEmpty(entityKey))
                {
                    var stringArray = JsonConvert.DeserializeObject<List<string>>(strResponse);
                    if (stringArray != null)
                        foreach (var item in stringArray)
                            ret.Add(JsonConvert.DeserializeObject<T>(item));
                    else
                        return null;
                }
                else
                {
                    var item = JsonConvert.DeserializeObject<T>(strResponse);
                    ret.Add(item);
                }
                return ret;
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException(exp.Message, exp);
            }
        }
        public static async Task DeleteEntityAsync(string url, string entityType, string entityId)
        {
            try
            {
                await httpClient.DeleteAsync(string.Format("{0}/api/entities/{1}/{2}", url, entityType, entityId));
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException(exp.Message, exp);
            }
        }
        public static async Task<string> QueryServiceInstanceProvisionStatusAsync(string url, string id)
        { 
            try
            {
                var response = await httpClient.GetAsync(string.Format("{0}/api/entities/service-instance/{1}?watch=true", url, id));
                return response.Content.ReadAsStringAsync().Result;
            }
            catch (Exception exp)
            {
                throw new InvalidOperationException(exp.Message, exp);
            }
        }
    }
}

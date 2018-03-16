using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogServiceClient
{
    public class AzureServiceParametersJsonBuilder
    {
        public static string BuildStorageInstanceParameters(string serviceId, string planId, string resourceGroup, string storageAccountName, string location, string accountType)
        {
            return string.Format(@"{{""service_id"":""{0}"",""plan_id"":""{1}"",""parameters"":{{""resourceGroup"":""{2}"", ""storageAccountName"":""{3}"", ""location"": ""{4}"", ""accountType"": ""{5}""}}}}",
                                    serviceId, planId, resourceGroup, storageAccountName, location, accountType);
        }
        public static string BuildStorageBindingParameters(string serviceId, string planId, string instanceId, string bindingId)
        {
            return string.Format(@"{{""service_id"":""{0}"",""plan_id"":""{1}"",""instance_id"":""{2}"",""binding_id"":""{3}""}}",
                                      serviceId, planId, instanceId, bindingId);
        }
    }
}

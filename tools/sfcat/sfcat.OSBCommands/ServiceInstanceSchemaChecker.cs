using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using OSB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class ServiceInstanceSchemaChecker : OSBSchemaChecker
    {
        public ServiceInstanceSchemaChecker(string serviceEndpoint):base(serviceEndpoint)
        {
        }
        public override bool IsValid(string payload)
        {
            var instance = JObject.Parse(payload);
            if (Services == null)
                throw new ApplicationException("Service catalog is empty.");
            var service = Services.FirstOrDefault(s => s.Id == instance["service_id"].ToString());
            if (service == null)
                throw new ApplicationException("Service Id is not found in service catalog.");
            var plan = service.Plans.FirstOrDefault(p => p.Id == instance["plan_id"].ToString());
            if (plan == null)
                throw new ApplicationException("Plan Id is not found in service catalog.");
            if (plan.Schemas == null || plan.Schemas.ServiceInstanceSchemaSet == null 
                || plan.Schemas.ServiceInstanceSchemaSet.CreationSchema == null
                || plan.Schemas.ServiceInstanceSchemaSet.CreationSchema.ParametersSchema == null)
                return true;
            var schema = JSchema.Parse(plan.Schemas.ServiceInstanceSchemaSet.CreationSchema.ParametersSchema.ToString());
            return instance["parameters"].IsValid(schema);
        }
    }
}

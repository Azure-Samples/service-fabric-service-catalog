using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class ServiceInstance
    {
        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        [JsonProperty("plan_id")]
        public string PlanId { get; set; }
        [JsonProperty("parameters")]
        public string Parameters { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class Binding
    {
        [JsonProperty("instance_id")]
        public string InstanceId { get; set; }
        [JsonProperty("binding_id")]
        public string BindingId { get; set; }
        [JsonProperty("service_id")]
        public string ServiceId { get; set; }
        [JsonProperty("plan_id")]
        public string PlanId { get; set; }
    }
}

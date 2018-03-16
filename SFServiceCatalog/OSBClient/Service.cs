using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class Service
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("tags")]
        public List<string> Tags { get; set; }
        [JsonProperty("requires")]
        public List<string> Requires { get; set; }
        [JsonProperty("bindable")]
        public bool Bindable { get; set; }
        [JsonProperty("metadata")]
        public string Metadata { get; set; }
        [JsonProperty("dashboard_client")]
        public DashboardClient DashboardClient { get; set; }
        [JsonProperty("plan_updateable")]
        public bool PlanUpdateable { get; set; }
        [JsonProperty("plans")]
        public List<Plan> Plans { get; set; }
        public Service()
        {
        }
    }
}

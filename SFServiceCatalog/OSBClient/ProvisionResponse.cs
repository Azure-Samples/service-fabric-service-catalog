using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class ProvisionResponse
    {
        [JsonProperty("dashboard_url")]
        public string DashboardUrl { get; set; }
        [JsonProperty("operation")]
        public string Operation { get; set; }        
    }
}

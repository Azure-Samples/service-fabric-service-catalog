using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class DashboardClient
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("secret")]
        public string Secret { get; set; }
        [JsonProperty("redirect_uri")]
        public string RedirectUri { get; set; }
    }
}

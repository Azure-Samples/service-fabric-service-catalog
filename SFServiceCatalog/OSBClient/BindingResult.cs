using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class BindingResult
    {
        [JsonProperty("credentials")]
        public string Credentials { get; set; }
        [JsonProperty("volumeMounts")]
        public string VolumeMounts { get; set; }        
    }
}

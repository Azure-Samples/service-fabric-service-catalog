using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class LastOperation
    {
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
    }
}

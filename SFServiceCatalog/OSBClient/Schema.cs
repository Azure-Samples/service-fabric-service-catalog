using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class Schema
    {
        [JsonProperty("parameters")]
        public object ParametersSchema { get; set; }
    }
}

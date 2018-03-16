using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class SchemaSet
    {
        [JsonProperty("create")]
        public Schema CreationSchema {get;set;}
        [JsonProperty("update")]
        public Schema UpdateSchema { get; set; }
    }
}

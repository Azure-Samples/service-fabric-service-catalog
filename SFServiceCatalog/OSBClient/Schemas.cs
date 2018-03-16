using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class Schemas
    {
        [JsonProperty("service_instance")]
        public SchemaSet ServiceInstanceSchemaSet { get; set; }
        [JsonProperty("service_binding")]
        public SchemaSet ServiceBindingSchemaSet { get; set; }
    }
}

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class Plan
    {
        [JsonProperty("id")]
        public string Id { get; set;  }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("metadata")]
        public string Metadata { get; set; }
        [JsonProperty("free")]
        public bool Free { get; set; }
        [JsonProperty("bindable")]
        public bool Bindable { get; set; }
        [JsonProperty("schemas")]
        public Schemas Schemas { get; set; }
    }
}

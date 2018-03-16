using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class BindingwithResult
    {
        [JsonProperty("binding")]
        public Binding Binding { get; set; }
        [JsonProperty("result")]
        public BindingResult Result { get; set; }
    }
}

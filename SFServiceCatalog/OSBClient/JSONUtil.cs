using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSB
{
    public class JSONUtil
    {
        public void Test()
        {
            JObject o2 = null;
            using (StreamReader file = File.OpenText(@"json1.json"))
            {
                using (JsonTextReader reader = new JsonTextReader(file))
                {
                    o2 = (JObject)JToken.ReadFrom(reader);
                }
            }
            var aak = o2.SelectToken("$.services[?(@.id == '" + "acb56d7c-XXXX-XXXX-XXXX-feb140a59a66" + "')].plans[?(@.id == '" + "d3031751-XXXX-XXXX-XXXX-a42377d3320e" + "')].schemas.service_instance.create.parameters");
            var attempt = JToken.Parse(@"{
                    'billing-account':122
                }");
            if (aak != null)
            {
                Console.WriteLine(aak.ToString());
                JSchema schema = JSchema.Parse(aak.ToString());
                IList<string> errorMessages;
                bool valid = attempt.IsValid(schema, out errorMessages);
                Console.WriteLine(valid);
            }
        }
    }
}

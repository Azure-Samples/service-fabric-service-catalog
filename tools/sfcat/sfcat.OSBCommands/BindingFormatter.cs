using Newtonsoft.Json.Linq;
using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class BindingFormatter : IEntityFormatter
    {
        public IEnumerable<string> GenerateOutputStrings<T>(List<T> entities)
        {
            List<string> output = new List<string>();
            output.Add(string.Format("#w{0,-40}  CREDENTIALS", "ID"));
            
            if (entities != null && entities.Count > 0)
            {
                var instances = entities.Cast<BindingwithResult>();
                foreach (var instance in instances)
                {
                    if (instance.Result == null)
                        output.Add(string.Format("#a{0,-40}  {1}", instance.Binding.BindingId, "#rBinding not found!"));
                    else if (instance.Result.Credentials == "null")
                        output.Add(string.Format("#a{0,-40}  {1}", instance.Binding.BindingId, "#rCredentials not found!"));
                    else
                    {
                        var credential = JObject.Parse(instance.Result.Credentials);
                        bool first = true;
                        foreach (var property in credential.Properties())
                        {
                            if (first)
                            {
                                output.Add(string.Format("#a{0,-40}  {1}: {2}", instance.Binding.BindingId, property.Name, property.Value));
                                first = false;
                            }
                            else
                                output.Add(string.Format("#a{0,-40}  {1}: {2}", "", property.Name, property.Value));
                        }
                    }
                }
            }
            return output;
        }
    }
}
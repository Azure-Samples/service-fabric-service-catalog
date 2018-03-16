using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class ServiceFormatter : IEntityFormatter
    {
        public IEnumerable<string> GenerateOutputStrings<T>(List<T> entities)
        {
            List<string> output = new List<string>();
            output.Add(string.Format("#w{0,-40}  NAME", "ID"));
            
            if (entities != null && entities.Count > 0)
            {
                var services = entities.Cast<Service>();
                foreach (var service in services)
                {
                    output.Add(string.Format("#a{0,-40}  {1}", service.Id, service.Name));
                }
            }
            return output;
        }
    }
}

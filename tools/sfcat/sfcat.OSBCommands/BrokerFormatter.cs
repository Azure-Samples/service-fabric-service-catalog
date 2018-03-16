using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class BrokerFormatter : IEntityFormatter
    {
        public IEnumerable<string> GenerateOutputStrings<T>(List<T> entities)
        {
            List<string> output = new List<string>();
            output.Add(string.Format("#w{0,-40}  URL", "NAME"));
            if (entities != null && entities.Count > 0)
            {
                var brokers = entities.Cast<Broker>();
                foreach (var broker in brokers)
                {
                    output.Add(string.Format("#a{0,-40}  {1}", broker.Name, broker.Url));
                }
            }
            return output;
        }
    }
}

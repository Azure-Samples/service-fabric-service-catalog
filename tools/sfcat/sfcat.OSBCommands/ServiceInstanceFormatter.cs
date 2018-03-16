using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class ServiceInstanceFormatter : IEntityFormatter
    {
        public IEnumerable<string> GenerateOutputStrings<T>(List<T> entities)
        {
            List<string> output = new List<string>();
            output.Add(string.Format("#w{0,-40}  {1,-40}  PLAN ID", "ID", "SERVICE ID"));
            
            if (entities != null && entities.Count > 0)
            {
                var instances = entities.Cast<ServiceInstance>();
                foreach (var instance in instances)
                {
                    output.Add(string.Format("#a{0,-40}  {1,-40}  {2}", instance.InstanceId, instance.ServiceId, instance.PlanId));
                }
            }
            return output;
        }
    }
}

using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class LastOperationFormatter : IEntityFormatter
    {
        public IEnumerable<string> GenerateOutputStrings<T>(List<T> entities)
        {
            List<string> output = new List<string>();
            if (entities != null && entities.Count > 0)
            {
                var operations = entities.Cast<LastOperation>();
                foreach (var operation in operations)
                {
                    output.Add(string.Format("Status：  {0}", operation.State));
                    output.Add(string.Format("         {0}", operation.Description));
                }
            }            
            return output;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    internal class DeleteServiceInstanceCommand : DeleteEntityCommand
    {
        public DeleteServiceInstanceCommand(Dictionary<string, string> switches = null) : base("service-instance", switches)
        {
        }
    }
}

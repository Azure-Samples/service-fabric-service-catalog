using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    internal class CreateOSBEntityCommand : CreateEntityCommand
    {
        public CreateOSBEntityCommand(string entityType, Dictionary<string, string> switches = null) : base(entityType, switches)
        {
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    internal class DeleteBindingCommand : DeleteEntityCommand
    {
        public DeleteBindingCommand(Dictionary<string, string> switches = null) : base("binding", switches)
        {
        }
    }
}

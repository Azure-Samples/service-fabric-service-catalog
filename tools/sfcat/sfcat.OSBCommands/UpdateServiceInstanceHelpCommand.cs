using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    public class UpdateServiceInstanceHelpCommand: Command
    {
        protected override Intention RunCommand()
        {
            return new MultiOutputConclusion(
              new string[]
            {
                "",
                "  #gupdate service-instance #c--file #a<filename> #c--id #a<instance id>",
                "",
                "  #c--file          #aService Instance definition JSON file",
                "  #c--id            #aService Instance id",
                "",
                "  #yExample:",
                "",
                "  #wTo create a new Binding:",
                "        #aupdate service-instnace --file /path/to/file --id my_unique_id"});
        }
    }
}

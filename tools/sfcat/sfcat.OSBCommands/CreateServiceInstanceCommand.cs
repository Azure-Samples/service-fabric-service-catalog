using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OSB;
using sfcat.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public class CreateServiceInstanceCommand : CreateEntityCommand
    {
        public CreateServiceInstanceCommand(Dictionary<string, string> switches = null, ISchemaChecker checker = null) 
            : base("service-instance", switches, checker)
        {
        }
        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!mySwitches.ContainsKey("file") && !mySwitches.ContainsKey("f"))
                return new HelpConclusion { Commands = { new CreateServiceInstanceHelpCommand() } };
            return base.CheckSwitches(new HelpConclusion { Commands = { new CreateServiceInstanceHelpCommand() } });
        }
        public override string BuildPayload()
        {
            string fileName = mySwitches.ContainsKey("f") ? mySwitches["f"] : mySwitches["file"];
            if (File.Exists(fileName))
                return this.FillSwitchValues(File.ReadAllText(fileName));            
            else
                return this.FillSwitchValues(fileName);
        }
        public override Conclusion CheckPayload(string payload)
        {
            if (mChecker != null && !mChecker.IsValid(payload))
                return new FailedConclusion("Service instance parameters are not following the schema specified by service catalog.");
            
            return base.CheckPayload(payload);
        }
    }
}

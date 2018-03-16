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
    public class UpdateServiceInstanceCommand : UpdateEntityCommand
    {
        public UpdateServiceInstanceCommand(Dictionary<string, string> switches = null, ISchemaChecker checker = null) 
            : base("service-instance", switches, checker)
        {
        }
        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!mySwitches.ContainsKey("file") && !mySwitches.ContainsKey("f"))
                return new Intention { Commands = { new CreateServiceInstanceHelpCommand() } };
            return base.CheckSwitches(new Intention { Commands = { new CreateServiceInstanceHelpCommand() } });
        }
        public override string BuildPayload()
        {
            return this.FillSwitchValues(File.ReadAllText(mySwitches.ContainsKey("f") ? mySwitches["f"] : mySwitches["file"]));            
        }
        public override Conclusion CheckPayload(string payload)
        {
            if (mChecker != null && !mChecker.IsValid(payload))
                return new FailedConclusion("Service instance parameters are not following the schema specified by service catalog.");
            
            return base.CheckPayload(payload);
        }
    }
}

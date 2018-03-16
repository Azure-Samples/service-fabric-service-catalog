using Newtonsoft.Json;
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
    public class CreateBindingCommand: CreateEntityCommand
    {
        
        public CreateBindingCommand(Dictionary<string, string> switches = null, ISchemaChecker checker = null) 
            : base("binding", switches, checker)
        {        
        }

        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!mySwitches.ContainsKey("file") && !mySwitches.ContainsKey("f") 
                || (!mySwitches.ContainsKey("instance-id") || string.IsNullOrEmpty(mySwitches["instance-id"])))
                return new HelpConclusion { Commands = { new CreateBindingHelpCommand() } };
            return base.CheckSwitches(new Intention { Commands = { new CreateBindingHelpCommand() } });
        }
        public override string BuildPayload()
        {
            string fileName = mySwitches.ContainsKey("f") ? mySwitches["f"] : mySwitches["file"];
            Binding binding;
            if (File.Exists(fileName))
                binding = JsonConvert.DeserializeObject<Binding>(File.ReadAllText(fileName));
            else
                binding = JsonConvert.DeserializeObject<Binding>(fileName);
            binding.BindingId = mEntityKey;
            binding.InstanceId = mySwitches["instance-id"];
            return this.FillSwitchValues(JsonConvert.SerializeObject(binding));
        }        
    }
}

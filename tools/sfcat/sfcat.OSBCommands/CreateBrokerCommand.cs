using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat.OSBCommands
{
    internal class CreateBrokerCommand : CreateOSBEntityCommand
    {
        public CreateBrokerCommand(Dictionary<string, string> switches = null) : base("broker", switches)
        {
        }
        protected override Intention CheckSwitches(Intention intention = null)
        {
            if (!(mySwitches.ContainsKey("name") && mySwitches.ContainsKey("url") && mySwitches.ContainsKey("user") && mySwitches.ContainsKey("password") || mySwitches.ContainsKey("f") || mySwitches.ContainsKey("file")))
                return new HelpConclusion { Commands = { new CreateBrokerHelpCommand() } };
            if (!mySwitches.ContainsKey("id"))
                mySwitches.Add("id", mySwitches["name"]);
            else
                mySwitches["id"] = mySwitches["name"];
            return base.CheckSwitches();
        }
        public override string BuildPayload()
        {
            string content = null;
            if (mySwitches.ContainsKey("file"))
                content = this.FillSwitchValues(File.ReadAllText(mySwitches["file"]));
            else if (mySwitches.ContainsKey("f"))
                content = this.FillSwitchValues(File.ReadAllText(mySwitches["f"]));
            else
                content = string.Format(@"{{
                                                    ""name"":""{0}"",
                                                    ""url"":""{1}"",
                                                    ""user"": ""{2}"",
                                                    ""password"": ""{3}""
                                                }}", mySwitches["name"], mySwitches["url"], mySwitches["user"], mySwitches["password"]);
            return content;
        }
    }
}

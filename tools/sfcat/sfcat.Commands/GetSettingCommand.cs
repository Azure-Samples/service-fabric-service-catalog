using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class GetSettingCommand: Command
    {
        private string mKey;
        public GetSettingCommand(string key)
        {
            mKey = key;
        }
        protected override Intention RunCommand()
        {
            if (ConfigurationManager.AppSettings[mKey] == null)
                return new FailedConclusion(string.Format("#wInvalid setting key '#y{0}'.", mKey));
            else
                return new SingeOutputConclusion(string.Format("#w{0} = #a{1}", mKey, ConfigurationManager.AppSettings[mKey]));
        }
    }
}

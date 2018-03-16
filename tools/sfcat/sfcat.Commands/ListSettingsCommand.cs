using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class ListSettingsCommand: Command
    {        
        public ListSettingsCommand()
        {
        }
        protected override Intention RunCommand()
        {
            List<string> output = new List<string>();
            foreach(var key in ConfigurationManager.AppSettings.AllKeys)
                output.Add(string.Format("#w{0} = #a{1}", key, ConfigurationManager.AppSettings[key]));
            return new MultiOutputConclusion(output);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    internal class SetSettingCommand : Command
    {
        private string mKey;
        private string mValue;
        public SetSettingCommand(string key, string value)
        {
            mKey = key;
            mValue = value;
        }
        protected override Intention RunCommand()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            if (settings[mKey] == null)
                return new FailedConclusion(string.Format("#wInvalid setting key '#y{0}'.", mKey));
            else
            {
                settings[mKey].Value = mValue;
                configFile.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(configFile.AppSettings.SectionInformation.Name);
            }
            return new EmptyConclusion();
        }
    }
}

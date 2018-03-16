using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace sfcat
{
    public abstract class Command
    {
        public event EventHandler<ConclusionEventArgs> OnIntermediateConclusion;

        protected static readonly HttpClient httpClient = new HttpClient();
        protected Dictionary<string, string> mySwitches;
        public Intention Run()
        {
            var intention = CheckSwitches();
            if (intention != null)
                return intention;
            return RunCommand();
        }      
        protected virtual Intention RunCommand()
        {
            return null;
        }
        protected virtual Intention RunCommand<T>()
        {
            return null;
        }
        protected virtual Intention CheckSwitches(Intention intention=null)
        {
            return null;
        }
        public Command()
        {

        }
        public void InjectSwitch(string key, string value)
        {
            if (mySwitches == null)
                mySwitches = new Dictionary<string, string>();
            if (mySwitches.ContainsKey(key))
            {
                mySwitches[key] = value;
            }
            else
                mySwitches.Add(key, value);
        }
        public Command(Dictionary<string, string> switches)
        {
            mySwitches = switches;
        }
        protected string FillSwitchValues(string json)
        {
            JObject jObject = (JObject)JsonConvert.DeserializeObject(json);

            foreach (var property in jObject.Properties())
                fillSwitchValues(property);
            
            return jObject.ToString(Formatting.None);
        }
        private void fillSwitchValues(JProperty property)
        {
            if (property.Value.Type == JTokenType.String)
            {
                if (mySwitches != null && mySwitches.ContainsKey(property.Name))
                    property.Value = mySwitches[property.Name];
            }
            else if (property.Value.Type == JTokenType.Object)
            {
                JObject obj = (JObject)property.Value;
                foreach (var prop in obj.Properties())
                    fillSwitchValues(prop);
            }
        }
        public void Notify(Conclusion conclusion)
        {
            OnIntermediateConclusion?.Invoke(this, new ConclusionEventArgs(conclusion));
        }
    }
}

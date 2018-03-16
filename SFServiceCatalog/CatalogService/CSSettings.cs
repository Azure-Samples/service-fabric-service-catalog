using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CatalogService
{
    public class CSSettings
    {
        public Dictionary<string, ActorSettings> SupportedTypes { get; set; }
        public Dictionary<string, Dictionary<string, string>> ActorSwitches { get; set; }
        public ClientSettings ClientSettings {get;set;}
    }
    public class ClientSettings
    {
        public Dictionary<string, EntitySettings> SupportedTypes { get; set; }
    }
    public class ActorSettings
    {
        public string CatalogServiceType { get; set; }
        public string EntityServiceType { get; set; }
        public string HandlerAssembly { get; set; }
        public string HandlerType { get; set; }
        public string SerializedProperties { get; set; }
    }
    public class EntitySettings
    {
        public string EntityType { get; set; }
        public string EntityAssembly { get; set; }
        public string FormatterType { get; set; }
        public string FormatterAssembly { get; set; }
        public string[] Aliases { get; set; }
    }
}

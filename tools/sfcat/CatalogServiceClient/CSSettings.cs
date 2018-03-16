using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatalogServiceClient
{
    public class ClientSettings
    {
        public Dictionary<string, EntitySettings> SupportedTypes { get; set; }
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

using Microsoft.ServiceFabric.Actors.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFServiceCatalog.SmartEntity.Interfaces
{
    public interface IEntityHandler
    {
        Task<HandlerResult> OnCreated(string id, string entityJson, Dictionary<string, string> switches);
        Task<HandlerResult> OnUpdated(string id, string newEntityJson, Dictionary<string, string> switches);
        Task<HandlerResult> OnDeleted(string id, string entityJosn, Dictionary<string, string> switches);
        Task<HandlerResult> OnWatch(string id, Dictionary<string, string> switches, object context);
    }
}

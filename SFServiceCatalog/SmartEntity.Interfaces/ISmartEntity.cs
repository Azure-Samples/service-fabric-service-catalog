using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFServiceCatalog.SmartEntity.Interfaces
{
    public interface ISmartEntity: IActor
    {
        Task SetSwitchesAsync(Dictionary<string,string> switches, CancellationToken cancellationToken);
        Task<string> OnCreated(string id, string json, CancellationToken cancellationToken);
        Task<string> OnUpdated(string id, string json, CancellationToken cancellationToken);
        Task<string> GetStatusAsync(CancellationToken cancellationToken);
        Task<string> OnDeleted(string id, CancellationToken cancellationToken);
    }
}

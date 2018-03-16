using Microsoft.ServiceFabric.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFServiceCatalog.SmartEntity.Interfaces
{
    public interface ISmartEntityCollection: IActor
    {
        Task DeleteEntityAsync(string id, CancellationToken cancellationToken);
        Task SetSwitchesAsync(Dictionary<string, string> switches, CancellationToken cancellationToken);
        Task AddEntityAsync(string id, string entity, string idField, CancellationToken cancellationToken);
        Task UpdateEntityAsync(string id, string entity, string idField, CancellationToken cancellationToken);

        Task<List<string>> ListEntitiesAsync(CancellationToken cancellationToken);
        Task<string> GetEntityAsync(string id, CancellationToken cancellationToken);
        Task<string> GetEntityStatusAsync(string id, CancellationToken cancellationToken);
    }
}

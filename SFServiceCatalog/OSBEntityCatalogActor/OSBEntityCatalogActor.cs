using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using SFServiceCatalog.SmartEntity;
using SFServiceCatalog.SmartEntity.Interfaces;

namespace OSBEntityCatalogActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    [ActorService(Name= "OSBEntityCatalogActor")]
    internal class OSBEntityCatalogActor : SmartEntityCollectionActor, ISmartEntityCollection
    {
        public OSBEntityCatalogActor(ActorService actorService, ActorId actorId) : base(actorService, actorId)
        {
        }
    }
}

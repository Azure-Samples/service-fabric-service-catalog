using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFServiceCatalog.SmartEntity;
using SFServiceCatalog.SmartEntity.Interfaces;

namespace OSBEntityActor
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
    [ActorService(Name = "OSBEntityActor")]
    internal class OSBEntityActor : SmartEntityActor, ISmartEntity
    {
        public OSBEntityActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }
    }
}

using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    internal class DeathSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription deathQuery = new QueryDescription().WithAll<Dead>();
        QueryDescription ownerQuery = new QueryDescription().WithAll<Owner>();
        QueryDescription removeQuery = new QueryDescription().WithAll<Remove>();
        public DeathSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in deathQuery, (Entity entity, ref Position position) =>
            {
                World.PhysicsWorld.RemoveEntity(entity.Reference(), position.Point);

                World.World.Query(in ownerQuery, (Entity ownedEntity, ref Owner owner) =>
                {
                    if(owner.OwnerReference == entity.Reference())
                    {
                        ownedEntity.Add(new Remove());
                    }
                });
            });

            World.World.Destroy(in deathQuery);
            World.World.Destroy(in removeQuery);
        }
    }
}

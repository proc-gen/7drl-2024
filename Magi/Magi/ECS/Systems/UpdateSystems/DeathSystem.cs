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
        public DeathSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in deathQuery, (Entity entity, ref Position position) =>
            {
                World.PhysicsWorld.RemoveEntity(entity.Reference(), position.Point);
            });

            World.World.Destroy(in deathQuery);
        }
    }
}

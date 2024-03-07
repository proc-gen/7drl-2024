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
    public class CountdownSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription imbuementQuery = new QueryDescription().WithAll<Imbuement>();

        public CountdownSystem(GameWorld world) : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in imbuementQuery, (Entity entity, ref Owner owner, ref Imbuement imbuement) =>
            {
                if ((owner.OwnerReference.Entity.Has<Player>() && World.CurrentState == Constants.GameState.PlayerTurn)
                    || (!owner.OwnerReference.Entity.Has<Player>() && World.CurrentState == Constants.GameState.MonsterTurn))
                {
                    imbuement.TurnsLeft--;

                    if (imbuement.TurnsLeft == 0)
                    {
                        entity.Remove<Imbuement>();
                    }
                }
            });
        }
    }
}

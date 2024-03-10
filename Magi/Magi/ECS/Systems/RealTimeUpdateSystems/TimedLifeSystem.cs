using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.ECS.Systems.UpdateSystems;
using Magi.Processors;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.RealTimeUpdateSystems
{
    public class TimedLifeSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription timedLifeQuery = new QueryDescription().WithAll<TimedLife>();

        public TimedLifeSystem(GameWorld world) 
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in timedLifeQuery, (Entity entity, ref TimedLife timedLife) =>
            {
                timedLife.TimeLeft -= (float)delta.TotalSeconds;
                if(timedLife.TimeLeft < 0)
                {
                    entity.Add(new Remove());
                }
            });

            DeathProcessor.RemoveMarkedEntities(World);
        }
    }
}

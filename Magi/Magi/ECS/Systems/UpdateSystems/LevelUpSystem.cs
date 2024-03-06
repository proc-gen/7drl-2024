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
    public class LevelUpSystem : ArchSystem, IUpdateSystem
    {
        public LevelUpSystem(GameWorld world) 
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            var stats = World.PlayerReference.Entity.Get<CombatStats>();
            if (stats.Experience >= stats.ExperienceForNextLevel)
            {
                ExperienceHelper.ProcessLevelUp(ref stats);
                World.PlayerReference.Entity.Set(stats);
            }
        }
    }
}

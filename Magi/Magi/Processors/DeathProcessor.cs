using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using SadConsole.EasingFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Processors
{
    public static class DeathProcessor
    {
        public static void CheckIfDead(GameWorld World, EntityReference Source, EntityReference Target, ref CombatStats sourceStats, ref CombatStats targetStats, Name sourceName, Name targetName)
        {
            if (targetStats.CurrentHealth == 0)
            {
                World.AddLogEntry(string.Concat(sourceName.EntityName, " killed ", targetName.EntityName, "!"));
                if (Source.Entity.Has<Player>())
                {
                    sourceStats.Experience += targetStats.Experience;
                    Source.Entity.Set(sourceStats);
                    Target.Entity.Add(new Dead());
                }
                else if (Target.Entity.Has<Player>())
                {
                    World.CurrentState = Constants.GameState.PlayerDeath;
                }
                else if (Target.Entity.Has<Boss>())
                {
                    World.CurrentState = Constants.GameState.SkillAcquired;
                }
            }
        }
    }
}

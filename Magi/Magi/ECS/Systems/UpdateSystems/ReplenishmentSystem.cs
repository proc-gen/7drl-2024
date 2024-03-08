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
    public class ReplenishmentSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription combatStatsQuery = new QueryDescription().WithAll<CombatStats>();

        public ReplenishmentSystem(GameWorld world) 
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in combatStatsQuery, (Entity entity, ref CombatStats combatStats) =>
            {
                if ((entity.Has<Player>() && World.CurrentState == Constants.GameState.PlayerTurn)
                    || (!entity.Has<Player>() && World.CurrentState == Constants.GameState.MonsterTurn))
                {
                    combatStats.CurrentMana = Math.Min(combatStats.MaxMana, combatStats.CurrentMana + Math.Max(combatStats.Level, 1) + combatStats.CurrentIntelligence / 10);
                }
            });
        }
    }
}

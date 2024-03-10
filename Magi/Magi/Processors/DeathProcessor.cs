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
                    if (Target.Entity.Has<Boss>())
                    {
                        World.CurrentState = Constants.GameState.SkillAcquired;
                    }
                    else
                    {
                        Target.Entity.Add(new Dead());
                    }

                    if(World.ConfirmedKills.ContainsKey(targetName.EntityName))
                    {
                        World.ConfirmedKills[targetName.EntityName]++;
                    }
                    else
                    {
                        World.ConfirmedKills[targetName.EntityName] = 1;
                    }
                }
                else if (Target.Entity.Has<Player>())
                {
                    World.CurrentState = Constants.GameState.PlayerDeath;
                }
            }
        }

        public static void MarkEntityForRemoval(GameWorld World, EntityReference entityReference)
        {
            QueryDescription ownerQuery = new QueryDescription().WithAll<Owner>();

            if (entityReference.Entity.Has<Position>())
            {
                World.PhysicsWorld.RemoveEntity(entityReference, entityReference.Entity.Get<Position>().Point);
            }

            World.World.Query(in ownerQuery, (Entity ownedEntity, ref Owner owner) =>
            {
                if (owner.OwnerReference == entityReference)
                {
                    ownedEntity.Add(new Remove());
                }
            });
        }

        public static void RemoveMarkedEntities(GameWorld World)
        {
            QueryDescription removeQuery = new QueryDescription().WithAll<Remove>();
            World.World.Destroy(in removeQuery);
        }
    }
}

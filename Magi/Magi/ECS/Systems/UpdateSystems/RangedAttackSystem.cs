using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Processors;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class RangedAttackSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription rangedAttacksQuery = new QueryDescription().WithAll<RangedAttack>();
        QueryDescription reloadingQuery = new QueryDescription().WithAll<Reloading>();
        Random random = new Random();
        public RangedAttackSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in reloadingQuery, (Entity entity, ref Owner owner) =>
            {
                if ((owner.OwnerReference.Entity.Has<Player>() && World.CurrentState == Constants.GameState.PlayerTurn)
                    || (!owner.OwnerReference.Entity.Has<Player>() && World.CurrentState == Constants.GameState.MonsterTurn))
                {
                    entity.Remove<Reloading>();
                }
            });

            World.World.Query(in rangedAttacksQuery, (ref RangedAttack rangedAttack) =>
            {
                var sourceName = rangedAttack.Source.Entity.Get<Name>();
                var sourceStats = rangedAttack.Source.Entity.Get<CombatStats>();
                var sourceEquipment = rangedAttack.Source.Entity.Get<CombatEquipment>();
                var targetName = rangedAttack.Target.Entity.Get<Name>();
                var targetStats = rangedAttack.Target.Entity.Get<CombatStats>();
                var targetEquipment = rangedAttack.Target.Entity.Get<CombatEquipment>();

                World.World.Create(
                                new RealTime(),
                                new Renderable() { Glyph = sourceEquipment.MainHandReference.Entity.Get<Renderable>().Glyph, Color = Color.DarkGray },
                                new TimedLife() { TimeLeft = 1f },
                                new Position() { Point = rangedAttack.Target.Entity.Get<Position>().Point });

                var damage = CombatStatHelper.CalculatePhysicalDamage(random, Constants.AttackType.Ranged, sourceStats, sourceEquipment, targetStats, targetEquipment);
                if (damage > 0)
                {
                    targetStats.CurrentHealth = Math.Max(0, targetStats.CurrentHealth - damage);
                    World.AddLogEntry(string.Concat(sourceName.EntityName, " shoots ", targetName.EntityName, " for ", damage, "hp."));
                    DeathProcessor.CheckIfDead(World, rangedAttack.Source, rangedAttack.Target, ref sourceStats, ref targetStats, sourceName, targetName);
                    rangedAttack.Target.Entity.Set(targetStats);
                }
                else
                {
                    World.AddLogEntry(string.Concat(sourceName.EntityName, " is unable to hurt ", targetName.EntityName, "."));
                }

                sourceEquipment.MainHandReference.Entity.Add(new Reloading());
            });

            World.World.Destroy(in rangedAttacksQuery);
        }
    }
}

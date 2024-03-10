using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Processors;
using Magi.Utils;
using SadConsole.EasingFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class MeleeAttackSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription meleeAttacksQuery = new QueryDescription().WithAll<MeleeAttack>();
        Random random = new Random();
        public MeleeAttackSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in meleeAttacksQuery, (ref MeleeAttack meleeAttack) =>
            {
                var sourceName = meleeAttack.Source.Entity.Get<Name>();
                var sourceStats = meleeAttack.Source.Entity.Get<CombatStats>();
                var sourceEquipment = meleeAttack.Source.Entity.Get<CombatEquipment>();
                var targetName = meleeAttack.Target.Entity.Get<Name>();
                var targetStats = meleeAttack.Target.Entity.Get<CombatStats>();
                var targetEquipment = meleeAttack.Target.Entity.Get<CombatEquipment>();

                World.World.Create(
                                new RealTime(),
                                new Renderable() { Glyph = sourceEquipment.MainHandReference.Entity.Get<Renderable>().Glyph, Color = Color.DarkGray },
                                new TimedLife() { TimeLeft = 1f },
                                new Position() { Point = meleeAttack.Target.Entity.Get<Position>().Point });

                var damage = CombatStatHelper.CalculatePhysicalDamage(random, Constants.AttackType.Melee, sourceStats, sourceEquipment, targetStats, targetEquipment);
                if (damage > 0)
                {
                    targetStats.CurrentHealth = Math.Max(0, targetStats.CurrentHealth - damage);
                    World.AddLogEntry(string.Concat(sourceName.EntityName, " hits ", targetName.EntityName, " for ", damage, "hp."));
                    DeathProcessor.CheckIfDead(World, meleeAttack.Source, meleeAttack.Target, ref sourceStats, ref targetStats, sourceName, targetName);
                    meleeAttack.Target.Entity.Set(targetStats);
                }
                else
                {
                    World.AddLogEntry(string.Concat(sourceName.EntityName, " is unable to hurt ", targetName.EntityName, "."));
                }
            });

            World.World.Destroy(in meleeAttacksQuery);
        }
    }
}

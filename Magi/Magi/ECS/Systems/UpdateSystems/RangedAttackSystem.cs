using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Items.Processors;
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
        Random random = new Random();
        public RangedAttackSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in rangedAttacksQuery, (ref RangedAttack rangedAttack) =>
            {
                var sourceName = rangedAttack.Source.Entity.Get<Name>();
                var sourceStats = rangedAttack.Source.Entity.Get<CombatStats>();
                var sourceEquipment = rangedAttack.Source.Entity.Get<CombatEquipment>();
                var targetName = rangedAttack.Target.Entity.Get<Name>();
                var targetStats = rangedAttack.Target.Entity.Get<CombatStats>();
                var targetEquipment = rangedAttack.Target.Entity.Get<CombatEquipment>();

                var damage = CalculateDamage(sourceStats, targetStats, sourceEquipment, targetEquipment);
                if (damage > 0)
                {
                    targetStats.CurrentHealth = Math.Max(0, targetStats.CurrentHealth - damage);
                    World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " shoots ", targetName.EntityName, " for ", damage, "hp.")));
                    if (targetStats.CurrentHealth == 0)
                    {
                        World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " killed ", targetName.EntityName, "!")));
                        if (rangedAttack.Source.Entity.Has<Player>())
                        {
                            rangedAttack.Target.Entity.Add(new Dead());
                        }
                        else
                        {
                            World.CurrentState = Constants.GameState.PlayerDeath;
                        }
                    }
                    rangedAttack.Target.Entity.Set(targetStats);
                }
                else
                {
                    World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " is unable to hurt ", targetName.EntityName, ".")));
                }
            });

            World.World.Destroy(in rangedAttacksQuery);
        }

        private int CalculateDamage(CombatStats sourceStats, CombatStats targetStats, CombatEquipment sourceEquipment, CombatEquipment targetEquipment)
        {
            int damage = 0;
            int damageReduction = 0;// targetStats.CurrentArmor;

            var weaponDamage = WeaponProcessor.CalculateDamage(random, sourceEquipment.MainHandReference, false);
            damage += weaponDamage.Damage;

            return damage - damageReduction;
        }
    }
}

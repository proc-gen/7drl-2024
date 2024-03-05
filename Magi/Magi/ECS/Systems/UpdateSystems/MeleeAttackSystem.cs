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

                var damage = CalculateDamage(sourceStats, sourceEquipment, targetStats, targetEquipment);
                if (damage > 0)
                {
                    targetStats.CurrentHealth = Math.Max(0, targetStats.CurrentHealth - damage);
                    World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " hits ", targetName.EntityName, " for ", damage, "hp.")));
                    if (targetStats.CurrentHealth == 0)
                    {
                        World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " killed ", targetName.EntityName, "!")));
                        if (meleeAttack.Source.Entity.Has<Player>())
                        {
                            meleeAttack.Target.Entity.Add(new Dead());
                        }
                        else
                        {
                            World.CurrentState = Constants.GameState.PlayerDeath;
                        }
                    }
                    meleeAttack.Target.Entity.Set(targetStats);
                }
                else
                {
                    World.LogItems.Add(new LogItem(string.Concat(sourceName.EntityName, " is unable to hurt ", targetName.EntityName, ".")));
                }
            });

            World.World.Destroy(in meleeAttacksQuery);
        }

        private int CalculateDamage(CombatStats sourceStats, CombatEquipment sourceEquipment, CombatStats targetStats, CombatEquipment targetEquipment)
        {
            int damage = Math.Max(0, (int)((sourceStats.CurrentStrength - 10f) / 2f + 1f));
            int damageReduction = 0; //targetStats.CurrentArmor;

            var weaponDamage = WeaponProcessor.CalculateDamage(random, sourceEquipment.MainHandReference, true);
            damage += weaponDamage.Damage;

            return damage - damageReduction;
        }
    }
}

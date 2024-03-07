using Magi.Constants;
using Magi.ECS.Components;
using Magi.Items.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Utils
{
    public static class CombatStatHelper
    {
        public static void ProcessLevelUp(ref CombatStats stats)
        {
            stats.Level++;
            stats.Experience -= stats.ExperienceForNextLevel;
            stats.ExperienceForNextLevel = ExperienceRequiredForLevel(stats.Level);
        }

        private static int ExperienceRequiredForLevel(int level)
        {
            return (int)(15f + 2f * MathF.Pow(level - 1, 2f));
        }

        public static int CalculateMaxHealth(int level, int vitality)
        {
            return 2 * level + 2 * vitality;
        }

        public static int CalculateMaxMana(int level, int intelligence)
        {
            return 2 * level + 4 * intelligence;
        }

        public static int CalculateDamage(Random random, AttackType attackType, CombatStats sourceStats, CombatEquipment sourceEquipment, CombatStats targetStats, CombatEquipment targetEquipment)
        {
            int damage = GetInitialDamage(attackType, sourceStats); 
            bool melee = attackType == AttackType.Melee;
            int damageReduction = 0;

            if (attackType == AttackType.Magic)
            {

            }
            else
            {
                var weaponDamage = WeaponProcessor.CalculateDamage(random, sourceEquipment.MainHandReference, melee, damage);
                damage = weaponDamage.Damage;

                damageReduction = ArmorProcessor.CalculateDamageReduction(random, weaponDamage, targetEquipment.ArmorReference, targetEquipment.OffHandReference, melee);
                damage += (int)(weaponDamage.ImbuementDamage * (1f + sourceStats.CurrentIntelligence / 100f));
            }
                
            return damage - damageReduction;
        }

        private static int GetInitialDamage(AttackType attackType, CombatStats sourceStats)
        {
            if(attackType == AttackType.Melee)
            {
                return Math.Max(0, (int)((sourceStats.CurrentStrength - 10f) / 2f + 1f));
            }
            else if (attackType == AttackType.Magic)
            {
                return Math.Max(0, (int)((sourceStats.CurrentIntelligence - 10f) / 2f + 1f));
            }

            return 0;
        }
    }
}

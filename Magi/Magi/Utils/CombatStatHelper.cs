﻿using Magi.Constants;
using Magi.ECS.Components;
using Magi.Processors;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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

        public static int CalculatePhysicalDamage(Random random, AttackType attackType, CombatStats sourceStats, CombatEquipment sourceEquipment, CombatStats targetStats, CombatEquipment targetEquipment)
        {
            int damage = GetInitialDamage(attackType, sourceStats); 
            bool melee = attackType == AttackType.Melee;

            var weaponDamage = WeaponProcessor.CalculateDamage(random, sourceEquipment.MainHandReference, sourceStats.CurrentDexterity, melee, damage);
            damage = weaponDamage.Damage;

            int damageReduction = ArmorProcessor.CalculateDamageReduction(random, weaponDamage, targetEquipment.ArmorReference, targetEquipment.OffHandReference, targetStats.CurrentDexterity, melee);
            damage += (int)(weaponDamage.ImbuementDamage * (1f + sourceStats.CurrentIntelligence / 100f));
                
            return damage - damageReduction;
        }

        public static int CalculateMagicDamage(Random random, Skill skill, CombatStats sourceStats, CombatEquipment sourceEquipment, CombatStats targetStats, CombatEquipment targetEquipment)
        {
            int damage = GetInitialDamage(AttackType.Skill, sourceStats);

            var skillDamage = SkillDamageProcessor.CalculateDamage(random, skill, sourceStats.CurrentDexterity, damage);
            damage += skillDamage.Damage;

            int damageReduction = ArmorProcessor.CalculateDamageReduction(random, skillDamage, targetEquipment.ArmorReference, targetEquipment.OffHandReference, targetStats.CurrentDexterity, false);
            damageReduction /= 2;

            return damage - damageReduction;
        }

        private static int GetInitialDamage(AttackType attackType, CombatStats sourceStats)
        {
            if(attackType == AttackType.Melee)
            {
                return Math.Max(0, (int)((sourceStats.CurrentStrength - 10f) / 5f + 1f));
            }
            else if (attackType == AttackType.Skill)
            {
                return Math.Max(0, (int)((sourceStats.CurrentIntelligence - 10f) / 5f + 1f));
            }

            return 0;
        }
    }
}

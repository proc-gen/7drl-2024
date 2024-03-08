using Arch.Core;
using Magi.Constants;
using Magi.Containers;
using Magi.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Processors
{
    public static class SkillDamageProcessor
    {
        public static DamageCalculation CalculateDamage(Random random, Skill skill, int bonusDamage)
        {
            int damage = bonusDamage;
            DamageTypes damageType = DamageTypes.Bludgeoning;
            bool criticalHit = false;

            var damageEntries = skill.DamageRoll.Split('d');
            int numDice = int.Parse(damageEntries[0]);
            int numSides = int.Parse(damageEntries[1]);

            for (int i = 0; i < numDice; i++)
            {
                damage += random.Next(0, numSides) + 1;
            }

            criticalHit = random.Next(100) < (20 - skill.CriticalHitRoll + 1) * 5;
            damage *= criticalHit ? skill.CriticalHitMultiplier : 1;                

            return new DamageCalculation()
            {
                Damage = damage,
                DamageType = damageType,
                CriticalHit = criticalHit,
                ImbuementDamage = 0,
                ImbuementElement = skill.Element,
            };
        }
    }
}

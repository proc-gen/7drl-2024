using Magi.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Processors
{
    public static class ItemProcessor
    {
        public static bool CanEquip(Item item, CombatStats combatStats)
        {
            return item.StrengthRequirement <= combatStats.CurrentStrength
                && item.DexterityRequirement <= combatStats.CurrentDexterity
                && item.IntelligenceRequirement <= combatStats.CurrentIntelligence;
        }
    }
}

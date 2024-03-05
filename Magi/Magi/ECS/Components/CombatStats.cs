using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct CombatStats
    {
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int MaxMana { get; set; }
        public int CurrentMana { get; set; }
        public int BaseStrength { get; set; }
        public int CurrentStrength { get; set; }
        public int BaseIntelligence { get; set; }
        public int CurrentIntelligence { get; set; }
        public int BaseVitality { get; set; }
        public int CurrentVitality { get; set; }
        public int BaseDexterity { get; set; }
        public int CurrentDexterity { get; set; }
    }
}

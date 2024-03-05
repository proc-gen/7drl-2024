using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers
{
    public struct WeaponContainer : IContainer
    {
        public string Name { get; set; }
        public DamageTypes DamageType { get; set; }
        public WeaponTypes WeaponType { get; set; }
        public string DamageRoll { get; set; }
        public int Range { get; set; }
        public int CriticalHitRoll { get; set; }
        public int CriticalHitMultiplier { get; set; }
    }
}

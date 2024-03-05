using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers
{
    public struct DamageCalculation
    {
        public int Damage { get; set; }
        public DamageTypes DamageType { get; set; }
        public bool CriticalHit { get; set; }
    }
}

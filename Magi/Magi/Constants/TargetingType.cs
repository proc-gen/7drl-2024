using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Constants
{
    public enum TargetingType
    {
        SingleTargetDamage = 1,
        SingleTargetAreaDamage = 2,
        ChainTargetDamage = 3,
        AreaTarget = 4,
        Self = 5,
        Directional = 6,
        Imbuement = 7
    }
}

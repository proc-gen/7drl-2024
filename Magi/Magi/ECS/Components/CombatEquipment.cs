using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct CombatEquipment
    {
        public EntityReference MainHandReference { get; set; }
        public EntityReference OffHandReference { get; set; }
        public EntityReference ArmorReference { get; set; }
        public CombatEquipment()
        {
            MainHandReference = EntityReference.Null;
            OffHandReference = EntityReference.Null;
            ArmorReference = EntityReference.Null;
        }
    }
}

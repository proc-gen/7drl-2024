using Arch.Core;
using Magi.ECS.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct RangedAttack : IAttack
    {
        public EntityReference Source { get; set; }
        public EntityReference Target { get; set; }
    }
}

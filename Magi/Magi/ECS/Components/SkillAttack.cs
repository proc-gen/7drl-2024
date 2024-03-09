using Arch.Core;
using Magi.ECS.Components.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct SkillAttack : IAttack
    {
        public EntityReference Source { get; set; }
        public EntityReference SourceSkill { get; set; }
        public EntityReference Target { get; set; }
        public Point TargetLocation { get; set; }
        public int TurnsLeft { get; set; }
    }
}

using Arch.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct CombatSkills
    {
        public EntityReference Skill1 { get; set; }
        public EntityReference Skill2 { get; set; }
        public EntityReference Skill3 { get; set; }
        public EntityReference Skill4 { get; set; }

        public CombatSkills() 
        {
            Skill1 = EntityReference.Null;
            Skill2 = EntityReference.Null;
            Skill3 = EntityReference.Null;
            Skill4 = EntityReference.Null;
        }
    }
}

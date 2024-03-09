using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Enemy
    {
        public int MeleeChance { get; set; }
        public int RangedChance { get; set; }
        public int SkillChance { get; set; }
    }
}

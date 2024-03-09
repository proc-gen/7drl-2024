using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Item
    {
        public int StrengthRequirement {  get; set; }
        public int DexterityRequirement { get; set; }
        public int IntelligenceRequirement { get; set; }
    }
}

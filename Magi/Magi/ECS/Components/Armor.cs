using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Armor
    {
        public int ArmorBonus { get; set; }
        public ArmorClass ArmorClass { get; set; }
        public ArmorType ArmorType { get; set; }
        public int BlockChance { get; set; }
    }
}

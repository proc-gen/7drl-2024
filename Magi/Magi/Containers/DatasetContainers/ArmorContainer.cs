using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers.DatasetContainers
{
    public struct ArmorContainer : IContainer
    {
        public string Name { get; set; }
        public int Armor { get; set; }
        public ArmorClass ArmorClass { get; set; }
        public ArmorType ArmorType { get; set; }
        public int BlockChance { get; set; }
    }
}

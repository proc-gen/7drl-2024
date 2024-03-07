using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers.DatasetContainers
{
    public struct PlayerContainer : IContainer
    {
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Vitality { get; set; }
        public int Intelligence { get; set; }
        public int Dexterity { get; set; }
        public string Mainhand { get; set; }
        public string Offhand { get; set; }
        public string Armor { get; set; }
    }
}

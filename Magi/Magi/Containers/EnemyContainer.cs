using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers
{
    public struct EnemyContainer : IContainer
    {
        public string Name { get; set; }
        public int ViewDistance { get; set; }
        public char Glyph { get; set; }
        public int GlyphColorRed { get; set; }
        public int GlyphColorGreen { get; set; }
        public int GlyphColorBlue { get; set; }
        public int Health { get; set; }
        public int Mana { get; set; }
        public int Strength { get; set; }
        public int Intelligence { get; set; }
        public int Vitality { get; set; }
        public int Dexterity { get; set; }
    }
}

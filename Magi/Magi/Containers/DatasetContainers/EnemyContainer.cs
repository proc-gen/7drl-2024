using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers.DatasetContainers
{
    public struct EnemyContainer : IContainer
    {
        public string Name { get; set; }
        public Elements Element { get; set; }
        public int LevelRequirement { get; set; }
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
        public string Mainhand { get; set; }
        public string Offhand { get; set; }
        public string Armor { get; set; }
        public int Experience { get; set; }
        public bool Boss { get; set; }
        public string Skill1 { get; set; }
        public string Skill2 { get; set; }
        public string Skill3 { get; set; }
        public string Skill4 { get; set; }
    }
}

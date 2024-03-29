﻿using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers.DatasetContainers
{
    public struct ItemContainer : IContainer
    {
        public string Name { get; set; }
        public int LevelRequirement { get; set; }
        public int StrengthRequirement { get; set; }
        public int DexterityRequirement { get; set; }
        public int IntelligenceRequirement { get; set; }
        public int Glyph { get; set; }
        public int GlyphColorRed { get; set; }
        public int GlyphColorGreen { get; set; }
        public int GlyphColorBlue { get; set; }
        public ItemTypes ItemType { get; set; }
    }
}

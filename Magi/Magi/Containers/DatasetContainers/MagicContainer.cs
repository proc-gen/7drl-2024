﻿using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Containers.DatasetContainers
{
    public struct MagicContainer : IContainer
    {
        public string Name { get; set; }
        public Elements Element { get; set; }
        public TargetingType TargetingType { get; set; }
        public TargetSpace TargetSpace { get; set; }
        public int ManaCost { get; set; }
        public string DamageRoll { get; set; }
        public int TargetRange { get; set; }
        public int EffectRange { get; set; }
        public int LifetimeTurns { get; set; }
        public int CriticalHitRoll { get; set; }
        public int CriticalHitMultiplier { get; set; }
    }
}

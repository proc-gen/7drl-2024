using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Imbuement
    {
        public Elements Element { get; set; }
        public int TurnsLeft { get; set; }
    }
}

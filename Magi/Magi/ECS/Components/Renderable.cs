using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Components
{
    public struct Renderable
    {
        public char Glyph { get; set; }
        public Color Color { get; set; }
        public bool ShowOutsidePlayerFov { get; set; }
    }
}

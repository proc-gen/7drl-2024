using Magi.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps
{
    public struct Tile
    {
        public TileTypes BaseTileType { get; set; }
        public Color BackgroundColor { get; set; }
        public char Glyph { get; set; }
        public Color GlyphColor { get; set; }
        public bool Explored { get; set; }

        public Tile()
        {
            BaseTileType = TileTypes.Wall;
            BackgroundColor = Color.Gray;
            Glyph = (char)0;
            GlyphColor = Color.Gray;
            Explored = false;
        }
    }
}

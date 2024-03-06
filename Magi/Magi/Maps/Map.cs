using Arch.Core;
using Magi.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps
{
    public class Map
    {
        [JsonProperty]
        protected Tile[] MapGrid { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            MapGrid = new Tile[Width * Height];
        }

        public Tile GetTile(int x, int y)
        {
            return MapGrid[y * Width + x];
        }

        public Tile GetTile(Point point)
        {
            return GetTile(point.X, point.Y);
        }

        public void SetTile(int x, int y, Tile tile)
        {
            MapGrid[y * Width + x] = tile;
        }

        public void SetTile(Point point, Tile tile)
        {
            SetTile(point.X, point.Y, tile);
        }

        public bool IsPathBlocked(Point start, Point end, int range)
        {
            bool retVal = false;
            if (start == end)
            {
                retVal = true;
            }
            else if (Point.EuclideanDistanceMagnitude(start, end) > (range * range))
            {
                retVal = true;
            }
            else
            {
                var pointsInLine = FieldOfView.GetPointsInLine(start, end);
                foreach (var point in pointsInLine)
                {
                    var tile = GetTile(point);
                    if (tile.BaseTileType == Constants.TileTypes.Wall)
                    {
                        retVal = true;
                        break;
                    }
                }
            }

            return retVal;
        }
    }
}

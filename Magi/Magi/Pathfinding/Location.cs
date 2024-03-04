using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Pathfinding
{
    public class Location : ILocation<Location>
    {
        public Point Point { get; private set; }

        public List<Location> AdjacentLocations { get; set; }

        public Location(Point point)
        {
            Point = point;
            AdjacentLocations = new List<Location>();
        }

        public bool Equals(ILocation<Location> other)
        {
            return Point == other.Point;
        }

        public override string ToString()
        {
            return Point.ToString();
        }
    }
}

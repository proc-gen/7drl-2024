using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Pathfinding
{
    public interface ILocation<T> : IEquatable<ILocation<T>>
    {
        Point Point { get; }
        List<T> AdjacentLocations { get; set; }
    }
}

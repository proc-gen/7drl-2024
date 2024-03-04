using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Pathfinding
{
    public interface IWeightedGraph<L>
        where L : ILocation<L>
    {
        float Cost(L a, L b);
        float Cost(Point a, L b);
        IEnumerable<L> GetNeighbors(L id, L end);
        IEnumerable<L> GetNeighbors(Point id, L end);
    }
}

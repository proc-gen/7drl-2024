using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Pathfinding
{
    public class AStarSearch<L>
        where L : ILocation<L>
    {
        IWeightedGraph<L> Graph { get; set; }
        public AStarSearch(IWeightedGraph<L> graph)
        {
            Graph = graph;
        }

        public Point RunSearch(L start, L end)
        {
            if (start == null)
            {
                return end.Point;
            }

            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();
            Dictionary<Point, float> costSoFar = new Dictionary<Point, float>();
            PriorityQueue<Point, float> frontier = new PriorityQueue<Point, float>();

            frontier.Enqueue(start.Point, 0);
            cameFrom[start.Point] = start.Point;
            costSoFar[start.Point] = 0;

            while (frontier.Count > 0 && frontier.Peek() != end.Point)
            {
                var current = frontier.Dequeue();

                foreach (var next in Graph.GetNeighbors(current, end))
                {
                    float newCost = costSoFar[current] + Graph.Cost(current, next);
                    if (!costSoFar.TryGetValue(next.Point, out float nextCost) || newCost < nextCost)
                    {
                        costSoFar[next.Point] = newCost;
                        float priority = newCost + Heuristic(next, end);
                        frontier.Enqueue(next.Point, priority);
                        cameFrom[next.Point] = current;
                    }
                }
            }

            var currentNode = end.Point;
            if (cameFrom.ContainsKey(end.Point))
            {
                var nextNode = cameFrom[end.Point];
                while (nextNode != start.Point)
                {
                    currentNode = nextNode;
                    nextNode = cameFrom[nextNode];
                }

                return currentNode;
            }
            return start.Point;
        }

        private static float Heuristic(L a, L b)
        {
            return (float)Point.EuclideanDistanceMagnitude(a.Point, b.Point);
        }
    }
}

using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Pathfinding
{
    public class SquareGrid : IWeightedGraph<Location>
    {
        public static readonly Location[] AdjacentLocations = new[]
        {
            new Location(new Point(Direction.Up.DeltaX, Direction.Up.DeltaY)),
            new Location(new Point(Direction.Down.DeltaX, Direction.Down.DeltaY)),
            new Location(new Point(Direction.Left.DeltaX, Direction.Left.DeltaY)),
            new Location(new Point(Direction.Right.DeltaX, Direction.Right.DeltaY))
        };

        public GameWorld World { get; set; }

        public SquareGrid(GameWorld world)
        {
            World = world;
        }

        public bool InBounds(Location id)
        {
            return 0 <= id.Point.X && id.Point.X < World.Map.Width
                && 0 <= id.Point.Y && id.Point.Y < World.Map.Height;
        }

        public bool Passable(Location id)
        {
            var tile = World.Map.GetTile(id.Point);
            if (tile.BaseTileType == Constants.TileTypes.Wall)
            {
                return false;
            }

            var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(id.Point);
            return entitiesAtLocation == null || !entitiesAtLocation.Any(a => a.Entity.Has<Blocker>());
        }

        public float Cost(Location a, Location b)
        {
            return 1;
        }

        public float Cost(Point a, Location b)
        {
            return Cost(new Location(a), b);
        }

        public IEnumerable<Location> GetNeighbors(Location id, Location end)
        {
            foreach (var direction in AdjacentLocations)
            {
                Location next = new Location(id.Point + direction.Point);
                if (InBounds(next) && (Passable(next) || next.Point == end.Point))
                {
                    yield return next;
                }
            }
        }

        public IEnumerable<Location> GetNeighbors(Point id, Location end)
        {
            return GetNeighbors(new Location(id), end);
        }
    }
}

using CommunityToolkit.HighPerformance.Buffers;
using Magi.Maps.Spawners;
using Magi.Pathfinding;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public class RandomGenerator : Generator
    {
        List<Rectangle> Rooms { get; set; }

        public RandomGenerator(int width, int height)
            : base(width, height)
        {
            Rooms = new List<Rectangle>();
        }

        public override void Generate()
        {
            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (Map.Width - 1) || j == (Map.Height - 1))
                    {
                        Map.SetTile(i, j, Wall);
                    }
                    else
                    {
                        Map.SetTile(i, j, i % 2 == j % 2 ? Floor : Floor);
                    }
                }
            }

            Rooms.Add(new Rectangle(0, 0, Map.Width, Map.Height));

            for (int i = 0; i < 400; i++)
            {
                int x = Random.Next(1, Map.Width - 1);
                int y = Random.Next(1, Map.Height - 1);
                if (x != Map.Width / 2 && y != Map.Height / 2)
                {
                    Map.SetTile(x, y, Wall);
                }
            }

        }

        public override Point GetPlayerStartingPosition()
        {
            return Rooms.First().Center;
        }

        public override void SpawnEntitiesForMap(GameWorld world, RandomTable<string> enemySpawnTable, RandomTable<string> itemSpawnTable)
        {

            EnemySpawner enemySpawner = new EnemySpawner(enemySpawnTable, Random);
            ItemSpawner itemSpawner = new ItemSpawner(itemSpawnTable, Random);

            List<Rectangle> rooms = new List<Rectangle>();
            int width = Map.Width / 10;
            int height = Map.Height / 10;

            for (int i = 0; i < Map.Width - width; i += width)
            {
                for (int j = 0; j < Map.Height - height; j += height)
                {
                    rooms.Add(new Rectangle(i, j, width, height));
                }
            }

            foreach (var room in rooms)
            {
                SpawnEntitiesForRoom(world, enemySpawner, itemSpawner, room);
            }
        }

        private void SpawnEntitiesForRoom(GameWorld world, EnemySpawner enemySpawner, ItemSpawner itemSpawner, Rectangle room)
        {
            int numSpawns = Random.Next(0, 4);
            HashSet<Point> enemyLocations = new HashSet<Point>();
            HashSet<Point> itemLocations = new HashSet<Point>();

            int tries = 0;

            while ((enemyLocations.Count + itemLocations.Count) < numSpawns && tries < 20)
            {
                var point = new Point(room.X + Random.Next(1, room.Width), room.Y + Random.Next(1, room.Height));
                if (Map.GetTile(point).BaseTileType != Constants.TileTypes.Wall)
                {
                    if (Random.Next(4) == 0)
                    {
                        itemLocations.Add(point);
                    }
                    else
                    {
                        enemyLocations.Add(point);
                    }
                }

                tries++;
            }

            enemySpawner.SpawnEntitiesForPoints(world, enemyLocations);
            itemSpawner.SpawnEntitiesForPoints(world, itemLocations);
        }

        public override Point GetExitPosition()
        {
            SquareGridMapOnly SquareGrid = new SquareGridMapOnly(Map);
            AStarSearch<Location> AStarSearch = new AStarSearch<Location>(SquareGrid);

            int distance = 0;
            var start = new Location(GetPlayerStartingPosition());
            Point exit = Point.Zero;

            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    var tile = Map.GetTile(i, j);
                    if (tile.BaseTileType == Constants.TileTypes.Floor)
                    {
                        Point end = new Point(i, j);
                        var newDistance = AStarSearch.DistanceToPoint(start, new Location(end));
                        if (newDistance > distance)
                        {
                            distance = newDistance;
                            exit = end;
                        }
                        else if (newDistance == -1 && start.Point == end)
                        {
                            Map.SetTile(i, j, Wall);
                        }
                    }
                }
            }

            return exit;
        }

        public override void SpawnExitForMap(GameWorld world)
        {
            SpawnExit(world, GetExitPosition());
        }
    }
}

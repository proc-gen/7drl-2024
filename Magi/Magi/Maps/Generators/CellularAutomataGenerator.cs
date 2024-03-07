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
    public class CellularAutomataGenerator : Generator
    {
        Map MapA, MapB;
        public CellularAutomataGenerator(int width, int height) 
            : base(width, height)
        {
            MapA = new Map(width, height);
            MapB = new Map(width, height);
        }

        public override void Generate()
        {
            SetInitialState();

            for (int i = 0; i < 15; i++)
            {
                RunIteration(i);
            }

            Map = MapA;

            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (MapA.Width - 1) || j == (MapA.Height - 1))
                    {
                        Map.SetTile(i, j, Wall);
                    }
                }
            }
        }

        private void SetInitialState()
        {
            for (int i = 0; i < MapA.Width; i++)
            {
                for (int j = 0; j < MapA.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (MapA.Width - 1) || j == (MapA.Height - 1))
                    {
                        MapA.SetTile(i, j, Wall);
                    }
                    else
                    {
                        if (Random.Next(100) > 55)
                        {
                            SetFloor(MapA, i, j);
                        }
                        else
                        {
                            MapA.SetTile(i, j, Wall);
                        }
                    }
                }
            }
        }

        private void RunIteration(int iterationNumber)
        {
            var currentMap = iterationNumber % 2 == 0 ? MapA : MapB;
            var nextMap = iterationNumber % 2 == 0 ? MapB : MapA;

            for (int i = 0; i < currentMap.Width; i++)
            {
                for (int j = 0; j < currentMap.Height; j++)
                {
                    int numNeighbors = GetNumNeighbors(currentMap, i, j);
                    if(numNeighbors > 4 || numNeighbors == 0)
                    {
                        nextMap.SetTile(i, j, Wall);
                    }
                    else
                    {
                        SetFloor(nextMap, i, j);
                    }
                }
            }
        }

        private int GetNumNeighbors(Map map, int i, int j)
        {
            int neighbors = 0;

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                    }
                    else if (i + x >= 0
                        && i + x < map.Width
                        && j + y >= 0
                        && j + y < map.Height
                    )
                    {
                        neighbors += (map.GetTile(i + x, j + y).BaseTileType == Constants.TileTypes.Wall) ? 1 : 0;
                    }
                    else
                    {
                        neighbors++;
                    }
                }
            }

            
            return neighbors;
        }

        private void SetFloor(Map map, int i, int j)
        {
            map.SetTile(i, j, i % 2 == j % 2 ? Floor : Floor);
        }

        public override Point GetPlayerStartingPosition()
        {
            var startingPoint = new Point(Map.Width / 2, Map.Height / 2);
            while(Map.GetTile(startingPoint).BaseTileType == Constants.TileTypes.Wall)
            {
                startingPoint = startingPoint.Translate(-1, 0);
            }

            return startingPoint;
        }

        public override void SpawnEntitiesForMap(GameWorld world, RandomTable<string> enemySpawnTable, RandomTable<string> itemSpawnTable)
        {

            EnemySpawner enemySpawner = new EnemySpawner(enemySpawnTable, Random);
            ItemSpawner itemSpawner = new ItemSpawner(itemSpawnTable, Random);

            List<Rectangle> rooms = new List<Rectangle>();
            int width = Map.Width / 10;
            int height = Map.Height / 10;

            for(int i = 0; i < Map.Width - width; i += width)
            {
                for(int j = 0; j < Map.Height - height; j += height)
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

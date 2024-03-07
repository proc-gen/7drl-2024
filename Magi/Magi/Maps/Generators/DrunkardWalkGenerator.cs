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
    public class DrunkardWalkGenerator : Generator
    {
        public DrunkardWalkGenerator(int width, int height) 
            : base(width, height)
        {
        }

        public override void Generate()
        {
            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    Map.SetTile(i, j, Wall);
                }
            }

            Point start = new Point(Map.Width / 2, Map.Height / 2);
            SetFloor(start.X, start.Y);

            int totalTiles = Map.Width * Map.Height,
                desiredTiles = Map.Width * Map.Height / 2,
                floorTiles = 1;

            while (floorTiles < desiredTiles)
            {
                int drunkX = start.X,
                    drunkY = start.Y,
                    drunkLife = 150;

                while(drunkLife > 0)
                {
                    var tile = Map.GetTile(drunkX, drunkY);
                    if(tile.BaseTileType == Constants.TileTypes.Wall)
                    {
                        floorTiles++;
                    }
                    SetFloor(drunkX, drunkY);

                    int direction = Random.Next(4);
                    switch (direction)
                    {
                        case 0:
                            if(drunkX > 2)
                            {
                                drunkX--;
                            }
                            break;
                        case 1:
                            if(drunkX < Map.Width - 2)
                            {
                                drunkX++;
                            }
                            break;
                        case 2:
                            if (drunkY > 2)
                            {
                                drunkY--;
                            }
                            break;
                        case 3:
                            if (drunkY < Map.Height - 2)
                            {
                                drunkY++;
                            }
                            break;
                    }

                    drunkLife--;
                }
            }
        }

        private void SetFloor(int i, int j)
        {
            Map.SetTile(i, j, i % 2 == j % 2 ? Floor : Floor);
        }

        public override Point GetPlayerStartingPosition()
        {
            var startingPoint = new Point(Map.Width / 2, Map.Height / 2);
            while (Map.GetTile(startingPoint).BaseTileType == Constants.TileTypes.Wall)
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

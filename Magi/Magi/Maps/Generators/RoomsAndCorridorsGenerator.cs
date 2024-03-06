using CommunityToolkit.HighPerformance.Buffers;
using Magi.Maps.Spawners;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public class RoomsAndCorridorsGenerator : Generator
    {
        List<Rectangle> Rooms { get; set; }

        public RoomsAndCorridorsGenerator(int width, int height)
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
                    Map.SetTile(i, j, Wall);
                }
            }

            int maxRooms = 30, minSize = 6, maxSize = 10;

            for (int i = 0; i < maxRooms; i++)
            {
                int roomWidth = Random.Next(minSize, maxSize);
                int roomHeight = Random.Next(minSize, maxSize);
                int x = Random.Next(1, Map.Width - roomWidth - 1) - 1;
                int y = Random.Next(1, Map.Height - roomHeight - 1) - 1;

                Rectangle room = new Rectangle(x, y, roomWidth, roomHeight);
                bool canAdd = true;
                if (Rooms.Any() && Rooms.Exists(a => a.Intersects(room)))
                {
                    canAdd = false;
                }
                if (canAdd)
                {
                    this.ApplyRoomToMap(room);
                    if (Rooms.Any())
                    {
                        Point newCenter = room.Center;
                        Point oldCenter = Rooms.Last().Center;

                        if (Random.Next(0, 2) == 1)
                        {
                            this.ApplyHorizontalTunnel(oldCenter.X, newCenter.X, oldCenter.Y);
                            this.ApplyVerticalTunnel(oldCenter.Y, newCenter.Y, newCenter.X);
                        }
                        else
                        {
                            this.ApplyVerticalTunnel(oldCenter.Y, newCenter.Y, oldCenter.X);
                            this.ApplyHorizontalTunnel(oldCenter.X, newCenter.X, newCenter.Y);
                        }
                    }
                    Rooms.Add(room);
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

            foreach (var room in Rooms)
            {
                SpawnEntitiesForRoom(world, enemySpawner, itemSpawner, room);
            }
        }

        private void SpawnEntitiesForRoom(GameWorld world, EnemySpawner enemySpawner, ItemSpawner itemSpawner, Rectangle room)
        {
            int numSpawns = Random.Next(0, 4);
            HashSet<Point> enemyLocations = new HashSet<Point>();
            HashSet<Point> itemLocations = new HashSet<Point>();

            while (enemyLocations.Count < numSpawns)
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
            }

            enemySpawner.SpawnEntitiesForPoints(world, enemyLocations);
            itemSpawner.SpawnEntitiesForPoints(world, itemLocations);
        }

        public override void SpawnExitForMap(GameWorld world)
        {
            SpawnExit(world, Rooms.Last().Center);
        }

        
    }
}

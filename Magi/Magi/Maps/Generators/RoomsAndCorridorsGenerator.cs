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
        static Tile Wall = new Tile()
        {
            BaseTileType = Constants.TileTypes.Wall,
            BackgroundColor = new Color(0f, 0f, .5f)
        };
        static Tile FloorA = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.LightGray
        };
        static Tile FloorB = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.WhiteSmoke
        };

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

            Random random = new Random();

            for (int i = 0; i < maxRooms; i++)
            {
                int roomWidth = random.Next(minSize, maxSize);
                int roomHeight = random.Next(minSize, maxSize);
                int x = random.Next(1, Map.Width - roomWidth - 1) - 1;
                int y = random.Next(1, Map.Height - roomHeight - 1) - 1;

                Rectangle room = new Rectangle(x, y, roomWidth, roomHeight);
                bool canAdd = true;
                if (Rooms.Any() && Rooms.Exists(a => a.Intersects(room)))
                {
                    canAdd = false;
                }
                if (canAdd)
                {
                    ApplyRoomToMap(room);
                    if (Rooms.Any())
                    {
                        Point newCenter = room.Center;
                        Point oldCenter = Rooms.Last().Center;

                        if (random.Next(0, 2) == 1)
                        {
                            ApplyHorizontalTunnel(oldCenter.X, newCenter.X, oldCenter.Y);
                            ApplyVerticalTunnel(oldCenter.Y, newCenter.Y, newCenter.X);
                        }
                        else
                        {
                            ApplyVerticalTunnel(oldCenter.Y, newCenter.Y, oldCenter.X);
                            ApplyHorizontalTunnel(oldCenter.X, newCenter.X, newCenter.Y);
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
            
        }

        private void ApplyRoomToMap(Rectangle room)
        {
            for (int i = room.X + 1; i <= room.MaxExtentX; i++)
            {
                for (int j = room.Y + 1; j <= room.MaxExtentY; j++)
                {
                    Map.SetTile(i, j, i % 2 == j % 2 ? FloorA : FloorB);
                }
            }
        }

        private void ApplyHorizontalTunnel(int x1, int x2, int y)
        {
            for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
            {
                Map.SetTile(i, y, i % 2 == y % 2 ? FloorA : FloorB);
            }
        }

        private void ApplyVerticalTunnel(int y1, int y2, int x)
        {
            for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
            {
                Map.SetTile(x, j, x % 2 == j % 2 ? FloorA : FloorB);
            }
        }
    }
}

using Magi.Maps.Spawners;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public class BspInteriorGenerator : Generator
    {
        const int MinRoomSize = 8;

        List<Rectangle> Rooms { get; set; }
        public BspInteriorGenerator(int width, int height) 
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

            var first = new Rectangle(1, 1, Map.Width - 1, Map.Height - 1);
            Rooms.Add(first);
            AddSubrectangles(first);

            for (int i = 0; i < Rooms.Count - 1; i++)
            {
                this.ApplyRoomToMap(Rooms[i]);
                this.ApplyCorridor(Rooms[i].Center.X, Rooms[i].Center.Y, Rooms[i + 1].Center.X, Rooms[i + 1].Center.Y);
            }

            this.ApplyRoomToMap(Rooms.Last());

            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (Map.Width - 1) || j == (Map.Height - 1))
                    {
                        Map.SetTile(i, j, Wall);
                    }
                }
            }
        }

        private void AddSubrectangles(Rectangle parentRectangle)
        {
            if(Rooms.Any()) 
            {
                Rooms.RemoveAt(Rooms.Count - 1);
            }

            int width = parentRectangle.Width,
                height = parentRectangle.Height,
                halfWidth = parentRectangle.Width / 2,
                halfHeight = parentRectangle.Height / 2,
                split = Random.Next(4);

            if(split < 2)
            {
                var h1 = new Rectangle(parentRectangle.X, parentRectangle.Y, halfWidth - 1, height);
                Rooms.Add(h1);
                if(halfWidth > MinRoomSize)
                {
                    AddSubrectangles(h1);
                }

                var h2 = new Rectangle(parentRectangle.X + halfWidth, parentRectangle.Y, halfWidth, height);
                Rooms.Add(h2);
                if (halfWidth > MinRoomSize)
                {
                    AddSubrectangles(h2);
                }
            }
            else
            {
                var v1 = new Rectangle(parentRectangle.X, parentRectangle.Y, width, halfHeight - 1);
                Rooms.Add(v1);
                if (halfHeight > MinRoomSize)
                {
                    AddSubrectangles(v1);
                }

                var v2 = new Rectangle(parentRectangle.X, parentRectangle.Y + halfHeight, width, halfHeight);
                Rooms.Add(v2);
                if (halfHeight > MinRoomSize)
                {
                    AddSubrectangles(v2);
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

            while ((enemyLocations.Count + itemLocations.Count) < numSpawns)
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

        public override Point GetExitPosition()
        {
            return Rooms.Last().Center;
        }

        public override void SpawnExitForMap(GameWorld world)
        {
            SpawnExit(world, GetExitPosition());
        }
    }
}

using Magi.Maps.Spawners;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public class BspRoomGenerator : Generator
    {
        List<Rectangle> Rooms { get; set; }
        List<Rectangle> SubSections { get; set; }
        public BspRoomGenerator(int width, int height) 
            : base(width, height)
        {
            Rooms = new List<Rectangle>();
            SubSections = new List<Rectangle>();
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

            AddSubSections(new Rectangle(2, 2, Map.Width - 5, Map.Height - 5));
            int roomCreateTries = 0;
            while(roomCreateTries < 240)
            {
                var subSection = GetRandomRectangle();
                var candidate = GetRandomSubRectangle(subSection);

                if(IsPossible(candidate))
                {
                    this.ApplyRoomToMap(candidate);
                    Rooms.Add(candidate);
                    AddSubSections(subSection);
                }

                roomCreateTries++;
            }

            for(int i = 0; i < Rooms.Count - 1; i++)
            {
                this.ApplyCorridor(Rooms[i].Center.X, Rooms[i].Center.Y, Rooms[i+1].Center.X, Rooms[i+1].Center.Y);
            }
        }

        private void AddSubSections(Rectangle parentSection)
        {
            int width = parentSection.Width;
            int height = parentSection.Height;
            int halfWidth = width / 2 + 1;
            int halfHeight = height / 2 + 1;

            SubSections.Add(new Rectangle(parentSection.X, parentSection.Y, halfWidth, halfHeight));
            SubSections.Add(new Rectangle(parentSection.X, parentSection.Y + halfHeight, halfWidth, halfHeight));
            SubSections.Add(new Rectangle(parentSection.X + halfWidth, parentSection.Y, halfWidth, halfHeight));
            SubSections.Add(new Rectangle(parentSection.X + halfWidth, parentSection.Y + halfHeight, halfWidth, halfHeight));
        }

        private Rectangle GetRandomRectangle()
        {
            if(SubSections.Count == 1)
            {
                return SubSections[0];
            }

            return SubSections[Random.Next(SubSections.Count)];
        }

        private Rectangle GetRandomSubRectangle(Rectangle parentSection)
        {
            int width = parentSection.Width;
            int height = parentSection.Height;

            int newWidth = Math.Max(5, Random.Next(Math.Min(15, width) - 1) + 1);
            int newHeight = Math.Max(5, Random.Next(Math.Min(15, height) - 1) + 1);

            var subRectangle = new Rectangle(parentSection.X + Random.Next(1, 6) - 1, parentSection.Y + Random.Next(1, 6) - 1, newWidth, newHeight);

            return subRectangle;
        }

        public bool IsPossible(Rectangle newRoom)
        {
            bool retVal = true;

            var testRoom = newRoom.Expand(2, 2);

            if(Rooms.Where(a => a.Intersects(testRoom)).Any())
            {
                retVal = false;
            }
            else if(testRoom.X < 1 
                    || testRoom.Y < 1
                    || testRoom.MaxExtentX > Map.Width - 2
                    || testRoom.MaxExtentY > Map.Height - 2) 
            {
                retVal = false;
            }
            
            return retVal;
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

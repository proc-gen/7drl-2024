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
        public BspRoomGenerator(int width, int height) 
            : base(width, height)
        {
            Rooms = new List<Rectangle>();
        }

        public override void Generate()
        {

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

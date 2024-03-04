using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Spawners
{
    public abstract class Spawner
    {
        protected RandomTable<string> SpawnTable;
        protected Random Random;

        public Spawner(RandomTable<string> spawnTable, Random random)
        {
            SpawnTable = spawnTable;
            Random = random;
        }

        public void SpawnEntitiesForPoints(GameWorld world, HashSet<Point> points)
        {
            foreach (var point in points)
            {
                SpawnEntityForPoint(world, point);
            }
        }

        public abstract void SpawnEntityForPoint(GameWorld world, Point point);
    }
}

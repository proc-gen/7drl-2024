using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public abstract class Generator
    {
        public Map Map { get; protected set; }
        protected Random Random;
        public Generator(int width, int height)
        {
            Map = new Map(width, height);
            Random = new Random();
        }

        public abstract void Generate();

        public abstract Point GetPlayerStartingPosition();
        public abstract void SpawnEntitiesForMap(GameWorld world, RandomTable<string> enemySpawnTable, RandomTable<string> itemSpawnTable);
        public abstract void SpawnExitForMap(GameWorld world);
    }
}

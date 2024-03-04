using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Spawners
{
    public class EnemySpawner : Spawner
    {
        public static Dictionary<string, EnemyContainer> EnemyContainers;

        static EnemySpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "enemies.json");
            EnemyContainers = JsonFileManager.LoadDataset<EnemyContainer>(path);
        }

        public EnemySpawner(RandomTable<string> spawnTable, Random random) 
            : base(spawnTable, random)
        {
        }

        public override void SpawnEntityForPoint(GameWorld world, Point point)
        {
            string key = SpawnTable.Roll(Random);
            var enemyContainer = EnemyContainers[key];

            var reference = world.World.Create(
                new Position() { Point = point },
                new Name() { EntityName = enemyContainer.Name },
                new ViewDistance() { Distance = enemyContainer.ViewDistance },
                new Renderable() { Glyph = enemyContainer.Glyph, Color = new Color(enemyContainer.GlyphColorRed, enemyContainer.GlyphColorGreen, enemyContainer.GlyphColorBlue)},
                new Input() { CanAct = true },
                new Blocker()
            ).Reference();

            world.PhysicsWorld.AddEntity(reference, point);
        }
    }
}

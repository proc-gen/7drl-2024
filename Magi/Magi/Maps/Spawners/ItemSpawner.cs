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
    public class ItemSpawner : Spawner
    {
        public static Dictionary<string, ItemContainer> ItemContainers;
        static ItemSpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "items.json");
            ItemContainers = JsonFileManager.LoadDataset<ItemContainer>(path);
        }

        public ItemSpawner(RandomTable<string> spawnTable, Random random)
            : base(spawnTable, random)
        {
        }

        public override void SpawnEntityForPoint(GameWorld world, Point point)
        {
            string key = SpawnTable.Roll(Random);
            var itemContainer = ItemContainers[key];

            var reference = world.World.Create(
                new Item(),
                new Position() { Point =  point },
                new Name() { EntityName = itemContainer.Name },
                new Renderable() { Glyph = (char)itemContainer.Glyph, Color = new Color(itemContainer.GlyphColorRed, itemContainer.GlyphColorGreen, itemContainer.GlyphColorBlue)}
            ).Reference();

            world.PhysicsWorld.AddEntity(reference, point);
        }
    }
}

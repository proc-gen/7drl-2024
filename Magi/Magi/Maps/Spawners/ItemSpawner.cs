using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.ECS.Helpers;
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
        public static Dictionary<string, ConsumableContainer> ConsumableContainers;
        public static Dictionary<string, WeaponContainer> WeaponContainers;
        static ItemSpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "items.json");
            ItemContainers = JsonFileManager.LoadDataset<ItemContainer>(path);

            path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "consumables.json");
            ConsumableContainers = JsonFileManager.LoadDataset<ConsumableContainer>(path);
            
            path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "weapons.json");
            WeaponContainers = JsonFileManager.LoadDataset<WeaponContainer>(path);
        }

        public ItemSpawner(RandomTable<string> spawnTable, Random random)
            : base(spawnTable, random)
        {
        }

        public override void SpawnEntityForPoint(GameWorld world, Point point)
        {
            string key = SpawnTable.Roll(Random);
            var itemContainer = ItemContainers[key];

            List<object> components = new List<object>()
            {
                new Item(),
                new Position() { Point =  point },
                new Name() { EntityName = itemContainer.Name },
                new Renderable() { Glyph = (char)itemContainer.Glyph, Color = new Color(itemContainer.GlyphColorRed, itemContainer.GlyphColorGreen, itemContainer.GlyphColorBlue)}
            };

            switch (itemContainer.ItemType)
            {
                case Constants.ItemTypes.Quest:
                    break;
                case Constants.ItemTypes.Consumable:
                    components.AddRange(GetConsumableComponents(key));
                    break;
                case Constants.ItemTypes.Weapon:
                    components.AddRange(GetWeaponComponents(key));
                    break;
            }

            var reference = world.World.CreateFromArray(components.ToArray()).Reference();

            world.PhysicsWorld.AddEntity(reference, point);
        }

        private List<object> GetConsumableComponents(string key)
        {
            var components = new List<object>();
            var consumableContainer = ConsumableContainers[key];
            components.Add(new Consumable()
            {
                ConsumableType = consumableContainer.ConsumableType,
                ConsumableAmount = consumableContainer.ConsumableAmount
            });
            return components;
        }

        private List<object> GetWeaponComponents(string key)
        {
            var components = new List<object>();
            var weaponContainer = WeaponContainers[key];
            components.Add(new Weapon()
            {
                DamageType = weaponContainer.DamageType,
                WeaponType = weaponContainer.WeaponType,
                DamageRoll = weaponContainer.DamageRoll,
                Range = weaponContainer.Range,
                CriticalHitRoll = weaponContainer.CriticalHitRoll,
                CriticalHitMultiplier = weaponContainer.CriticalHitMultiplier
            });

            return components;
        }
    }
}

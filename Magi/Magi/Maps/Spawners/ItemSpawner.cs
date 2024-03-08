using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers.DatasetContainers;
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
        static Dictionary<string, ItemContainer> ItemContainers;
        static Dictionary<string, ConsumableContainer> ConsumableContainers;
        static Dictionary<string, WeaponContainer> WeaponContainers;
        static Dictionary<string, ArmorContainer> ArmorContainers;

        static ItemSpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "items.json");
            ItemContainers = JsonFileManager.LoadDataset<ItemContainer>(path);

            path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "consumables.json");
            ConsumableContainers = JsonFileManager.LoadDataset<ConsumableContainer>(path);
            
            path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "weapons.json");
            WeaponContainers = JsonFileManager.LoadDataset<WeaponContainer>(path);

            path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "armor.json");
            ArmorContainers = JsonFileManager.LoadDataset<ArmorContainer>(path);
        }

        public ItemSpawner(RandomTable<string> spawnTable, Random random)
            : base(spawnTable, random)
        {
        }

        public override void SpawnAnEntityForPoint(GameWorld world, Point point)
        {
            string key = SpawnTable.Roll(Random);
            var itemContainer = ItemContainers[key];

            List<object> components = new List<object>()
            {
                new Item()
                {
                    StrengthRequirement = itemContainer.StrengthRequirement,
                    DexterityRequirement = itemContainer.DexterityRequirement,
                    IntelligenceRequirement = itemContainer.IntelligenceRequirement,
                },
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
                case Constants.ItemTypes.Armor:
                    components.AddRange(GetArmorComponents(key));
                    break;
            }

            var reference = world.World.CreateFromArray(components.ToArray()).Reference();

            world.PhysicsWorld.AddEntity(reference, point);
        }

        public static EntityReference SpawnEntityForOwner(GameWorld world, string key, EntityReference owner) 
        {
            var itemContainer = ItemContainers[key];

            List<object> components = new List<object>()
            {
                new Item(){
                    StrengthRequirement = itemContainer.StrengthRequirement,
                    DexterityRequirement = itemContainer.DexterityRequirement,
                    IntelligenceRequirement = itemContainer.IntelligenceRequirement,
                },
                new Owner() { OwnerReference =  owner },
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
                case Constants.ItemTypes.Armor:
                    components.AddRange(GetArmorComponents(key));
                    break;
            }

            return world.World.CreateFromArray(components.ToArray()).Reference();
        }

        public static List<string> GetItemsForLevel(int level)
        {
            var items = new List<string>();
            foreach (var item in ItemContainers)
            {
                if (item.Value.LevelRequirement >= level)
                {
                    items.Add(item.Key);
                }
            }

            return items;
        }

        public static List<string> GetWeaponsForLevel(int level)
        {
            var weapons = new List<string>();
            foreach (var item in ItemContainers)
            {
                if(item.Value.ItemType == Constants.ItemTypes.Weapon
                    && item.Value.LevelRequirement >= level)
                {
                    weapons.Add(item.Key);
                }
            }

            return weapons;
        }

        public static List<string> GetArmorsForLevel(int level)
        {
            var armors = new List<string>();
            foreach (var item in ItemContainers)
            {
                if (item.Value.ItemType == Constants.ItemTypes.Armor
                    && ArmorContainers[item.Key].ArmorType == Constants.ArmorType.Wearable
                    && item.Value.LevelRequirement >= level)
                {
                    armors.Add(item.Key);
                }
            }

            return armors;
        }

        public static List<string> GetShieldsForLevel(int level)
        {
            var shields = new List<string>();
            foreach (var item in ItemContainers)
            {
                if (item.Value.ItemType == Constants.ItemTypes.Armor
                    && ArmorContainers[item.Key].ArmorType == Constants.ArmorType.Shield
                    && item.Value.LevelRequirement >= level)
                {
                    shields.Add(item.Key);
                }
            }

            return shields;
        }

        private static List<object> GetConsumableComponents(string key)
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

        private static List<object> GetWeaponComponents(string key)
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

        private static List<object> GetArmorComponents(string key) 
        {
            var components = new List<object>();
            var armorContainer = ArmorContainers[key];
            components.Add(new Armor()
            {
                ArmorBonus = armorContainer.Armor,
                ArmorType = armorContainer.ArmorType,
                ArmorClass = armorContainer.ArmorClass,
                BlockChance = armorContainer.BlockChance,
            });
            return components;
        }
    }
}

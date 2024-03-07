﻿using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.UI.Helpers;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    internal class InventoryWindow : Window
    {
        GameWorld World;
        List<EntityReference> InventoryItems;
        int selectedItem = 0;
        QueryDescription ownedItemsQuery = new QueryDescription().WithAll<Owner>();
        public InventoryWindow(GameWorld world)
            : base(GameSettings.GAME_WIDTH / 4,
                    GameSettings.GAME_HEIGHT / 4 - 5,
                    GameSettings.GAME_WIDTH / 2,
                    GameSettings.GAME_HEIGHT / 2)
        {
            World = world;
            InventoryItems = new List<EntityReference>();
        }

        public override void Update(TimeSpan delta)
        {
            if(!Visible && World.CurrentState == Constants.GameState.ShowInventory)
            {
                Visible = true;
                selectedItem = 0;
                UpdateInventoryItems();
            }
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            bool retVal = false;
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                Visible = false;
                World.CurrentState = Constants.GameState.AwaitingPlayerInput;
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Up))
            {
                selectedItem = Math.Max(selectedItem - 1, 0);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                selectedItem = Math.Min(selectedItem + 1, 11);
                retVal = true;
            }
            else if (InventoryItems.Any())
            {
                if (keyboard.IsKeyPressed(Keys.U))
                {
                    UseItem(InventoryItems[selectedItem]);
                    retVal = true;
                }
                else if (keyboard.IsKeyPressed(Keys.D))
                {
                    DropItem(InventoryItems[selectedItem]);
                    retVal = true;
                }
            }

            return retVal;
        }

        private void UpdateInventoryItems()
        {
            InventoryItems.Clear();
            World.World.Query(in ownedItemsQuery, (Entity entity, ref Owner owner) =>
            {
                if (owner.OwnerReference == World.PlayerReference && !entity.Has<Equipped>())
                {
                    InventoryItems.Add(entity.Reference());
                }
            });
        }

        private void UseItem(EntityReference item)
        {
            item.Entity.Add(new WantToUseItem());

            World.StartPlayerTurn(Point.None);
            Visible = false;
        }

        private void DropItem(EntityReference item)
        {
            var ownerPosition = item.Entity.Get<Owner>().OwnerReference.Entity.Get<Position>();
            var ownerName = item.Entity.Get<Owner>().OwnerReference.Entity.Get<Name>();
            var itemName = item.Entity.Get<Name>();

            Point targetPosition = Point.None;
            int fovDistanceForDrop = 0;
            do
            {
                var pointsToCheck = FieldOfView.CalculateFOV(World, ownerPosition.Point, fovDistanceForDrop);
                foreach (var point in pointsToCheck)
                {
                    if (targetPosition == Point.None
                        && World.Map.GetTile(point).BaseTileType != Constants.TileTypes.Wall)
                    {
                        var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(point);
                        if (entitiesAtLocation == null || !entitiesAtLocation.Any(a => a.Entity.Has<Item>()))
                        {
                            targetPosition = point;
                        }
                    }
                }
                fovDistanceForDrop++;
            } while (targetPosition == Point.None);

            item.Entity.Remove<Owner>();
            item.Entity.Add(new Position() { Point = targetPosition });
            World.PhysicsWorld.AddEntity(item, targetPosition);

            World.AddLogEntry(string.Concat(ownerName.EntityName, " dropped ", itemName.EntityName));

            World.StartPlayerTurn(Point.None);
            Visible = false;
        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            DrawBoxAndTitle();
            DrawInventoryItems();
            DrawEquipmentList();
            DrawItemSelector();
            DrawPlayerStats();
            DrawItemDescription();
            Console.Render(delta);
        }

        private void DrawBoxAndTitle()
        {
            Console.DrawRLTKStyleBox(0, 0, Console.Width - 1, Console.Height - 1, Color.White, Color.Black);
            Console.Print(Console.Width / 2 - 5, 2, "Inventory");
        }

        private void DrawInventoryItems()
        {
            Console.Print(6, 4, "Backpack Items");
            for (int i = 0; i < 12; i++)
            {
                Console.Print(6, 6 + i, string.Concat(1 + i, ": ", i < InventoryItems.Count ? InventoryItems[i].Entity.Get<Name>().EntityName : string.Empty));
            }
        }

        private void DrawEquipmentList()
        {
            var equipment = World.PlayerReference.Entity.Get<CombatEquipment>();
            var weapon = equipment.MainHandReference;
            var offHand = equipment.OffHandReference;
            var armor = equipment.ArmorReference;

            Console.Print(6, 19, "Equipment");
            Console.Print(6, 21, string.Concat("Main Hand: ", weapon != EntityReference.Null ? weapon.Entity.Get<Name>().EntityName : "Fist"));
            Console.Print(6, 22, string.Concat("Off Hand: ", offHand != EntityReference.Null && weapon != offHand ? offHand.Entity.Get<Name>().EntityName : string.Empty));
            Console.Print(6, 23, string.Concat("Armor: ", armor != EntityReference.Null ? armor.Entity.Get<Name>().EntityName : string.Empty));

            Console.Print(6, 25, string.Concat("Weapon Damage: ", weapon != EntityReference.Null ? string.Concat(weapon.Entity.Get<Weapon>().DamageRoll, " ", weapon.Entity.Get<Weapon>().DamageType) : "1d3 Bludgeoning"));
            Console.Print(6, 26, string.Concat("Critical Hit Chance: ", weapon != EntityReference.Null ? string.Concat((20 - weapon.Entity.Get<Weapon>().CriticalHitRoll + 1) * 5, "%") : "5%"));
            Console.Print(6, 27, string.Concat("Range: ", weapon != EntityReference.Null && weapon.Entity.Get<Weapon>().Range > 1 ? weapon.Entity.Get<Weapon>().Range : "Melee"));

            int armorBonus = 0;
            if(offHand != EntityReference.Null && offHand.Entity.Has<Armor>())
            {
                armorBonus += offHand.Entity.Get<Armor>().ArmorBonus;
            }
            if (armor != EntityReference.Null && armor.Entity.Has<Armor>())
            {
                armorBonus += armor.Entity.Get<Armor>().ArmorBonus;
            }

            Console.Print(6, 28, string.Concat("Armor Bonus: ", armorBonus));
        }

        private void DrawPlayerStats()
        {
            var stats = World.PlayerReference.Entity.Get<CombatStats>();

            Console.Print(6 + Console.Width / 2, 19, "Player Stats");

            Console.Print(6 + Console.Width / 2, 21, string.Concat("Level: ", stats.Level));
            Console.Print(6 + Console.Width / 2, 22, string.Concat("Experience: ", stats.Experience, "/", stats.ExperienceForNextLevel));

            Console.Print(6 + Console.Width / 2, 24, string.Concat("Strength: ", stats.CurrentStrength));
            Console.Print(6 + Console.Width / 2, 25, string.Concat("Dexterity: ", stats.CurrentDexterity));
            Console.Print(6 + Console.Width / 2, 26, string.Concat("Intelligence: ", stats.CurrentIntelligence));
            Console.Print(6 + Console.Width / 2, 27, string.Concat("Vitality: ", stats.CurrentVitality));
        }

        private void DrawItemDescription()
        {
            Console.Print(6 + Console.Width / 2, 4, "Item Description");
            if (InventoryItems.Any() && selectedItem < InventoryItems.Count)
            {
                var item = InventoryItems[selectedItem];
                if (item.Entity.Has<Consumable>())
                {
                    var consumable = item.Entity.Get<Consumable>();
                    switch(consumable.ConsumableType)
                    {
                        case Constants.ConsumableTypes.Health:
                            Console.Print(6 + Console.Width / 2, 6, string.Concat("Restores up to ", consumable.ConsumableAmount, "hp"));
                            break;
                        case Constants.ConsumableTypes.Mana:
                            Console.Print(6 + Console.Width / 2, 6, string.Concat("Restores up to ", consumable.ConsumableAmount, "mp"));
                            break;
                    }
                }
                else if (item.Entity.Has<Weapon>())
                {
                    Console.Print(6 + Console.Width / 2, 6, string.Concat("Weapon Damage: ", string.Concat(item.Entity.Get<Weapon>().DamageRoll, " ", item.Entity.Get<Weapon>().DamageType)));
                    Console.Print(6 + Console.Width / 2, 7, string.Concat("Critical Hit Chance: ", string.Concat((20 - item.Entity.Get<Weapon>().CriticalHitRoll + 1) * 5, "%")));
                    Console.Print(6 + Console.Width / 2, 8, string.Concat("Range: ", item.Entity.Get<Weapon>().Range > 1 ? item.Entity.Get<Weapon>().Range : "Melee"));
                }
            }
        }

        private void DrawItemSelector()
        {
            Console.Print(3, selectedItem + 6, "->");
        }
    }
}

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
        public InventoryWindow(int x, int y, int width, int height, GameWorld world)
            : base(x, y, width, height)
        {
            World = world;
            InventoryItems = new List<EntityReference>();
        }

        public override void Update(TimeSpan delta)
        {
            if(!Visible && World.CurrentState == Constants.GameState.ShowInventory)
            {
                Visible = true;
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

            else if (InventoryItems.Any())
            {
                if (keyboard.IsKeyPressed(Keys.Up))
                {
                    selectedItem = (selectedItem - 1) % InventoryItems.Count;
                    retVal = true;
                }
                else if (keyboard.IsKeyPressed(Keys.Down))
                {
                    selectedItem = (selectedItem + 1) % InventoryItems.Count;
                    retVal = true;
                }
                else if (keyboard.IsKeyPressed(Keys.U))
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

            World.LogItems.Add(new LogItem(string.Concat(ownerName.EntityName, " dropped ", itemName.EntityName)));

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
            for (int i = 0; i < InventoryItems.Count; i++)
            {
                Console.Print(6, 6 + i, string.Concat(1 + i, ": ", InventoryItems[i].Entity.Get<Name>().EntityName));
            }
        }

        private void DrawEquipmentList()
        {
            var equipment = World.PlayerReference.Entity.Get<CombatEquipment>();
            Console.Print(6, 20, "Equipment");
            Console.Print(6, 22, string.Concat("Main Hand: ", equipment.MainHandReference != EntityReference.Null ? equipment.MainHandReference.Entity.Get<Name>().EntityName : string.Empty));
            Console.Print(6, 23, string.Concat("Off Hand: ", equipment.OffHandReference != EntityReference.Null && equipment.MainHandReference != equipment.OffHandReference ? equipment.OffHandReference.Entity.Get<Name>().EntityName : string.Empty));
            Console.Print(6, 24, string.Concat("Armor: ", equipment.ArmorReference != EntityReference.Null ? equipment.ArmorReference.Entity.Get<Name>().EntityName : string.Empty));
        }

        private void DrawItemSelector()
        {
            if (InventoryItems.Any())
            {
                Console.Print(3, selectedItem + 6, "->");
            }
        }
    }
}

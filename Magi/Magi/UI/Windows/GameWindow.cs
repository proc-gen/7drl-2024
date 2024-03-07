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
    public class GameWindow : Window
    {
        GameWorld world;
        TargetingOverlay targetingOverlay;
        public GameWindow(GameWorld world, TargetingOverlay targetingOverlay) 
            : base()
        {
            this.world = world;
            this.targetingOverlay = targetingOverlay;
            Visible = true;
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            bool retVal = false;
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                RequestMoveDirection(Direction.Up);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                RequestMoveDirection(Direction.Down);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                RequestMoveDirection(Direction.Left);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                RequestMoveDirection(Direction.Right);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Space))
            {
                RequestMoveDirection(Direction.None);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.I))
            {
                world.CurrentState = Constants.GameState.ShowInventory;
            }
            else if (keyboard.IsKeyPressed(Keys.G))
            {
                TryPickUpItem();
            }
            else if (keyboard.IsKeyPressed(Keys.A))
            {
                var weapon = world.PlayerReference.Entity.Get<CombatEquipment>().MainHandReference;
                if (weapon != EntityReference.Null
                    && weapon.Entity.Get<Weapon>().Range > 1)
                {
                    targetingOverlay.SetEntityForTargeting(weapon);
                    world.CurrentState = Constants.GameState.Targeting;
                }
            }

            return retVal;
        }

        private void RequestMoveDirection(Direction direction)
        {
            world.StartPlayerTurn(direction == Direction.None
                                    ? Point.None
                                    : new Point(direction.DeltaX, direction.DeltaY));
        }

        private void TryPickUpItem()
        {
            var name = world.PlayerReference.Entity.Get<Name>();
            var position = world.PlayerReference.Entity.Get<Position>();
            var entitiesAtLocation = world.PhysicsWorld.GetEntitiesAtLocation(position.Point);
            if (entitiesAtLocation != null && entitiesAtLocation.Any(a => a.Entity.Has<Item>()))
            {
                var item = entitiesAtLocation.Where(a => a.Entity.Has<Item>()).FirstOrDefault();
                string itemName = item.Entity.Get<Name>().EntityName;
                item.Entity.Add(new Owner() { OwnerReference = world.PlayerReference });
                item.Entity.Remove<Position>();
                world.PhysicsWorld.RemoveEntity(item, position.Point);

                world.AddLogEntry(string.Concat(name.EntityName, " picked up ", itemName));
            }
            else
            {
                world.AddLogEntry("There's nothing here");
            }

            world.StartPlayerTurn(Point.None);
        }

        public override void Update(TimeSpan delta)
        {

        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            RenderPlayerStats();
            RenderGameLog();
            RenderPositionInfo();
            Console.Render(delta);
        }

        private void RenderPlayerStats()
        {
            Console.DrawRLTKStyleBox(
                0,
                GameSettings.GAME_HEIGHT - 11,
                GameSettings.GAME_WIDTH / 4 - 1,
                10,
                Color.White,
                Color.Black
            );

            var stats = world.PlayerReference.Entity.Get<CombatStats>();
            var equipment = world.PlayerReference.Entity.Get<CombatEquipment>();

            Console.Print(2, GameSettings.GAME_HEIGHT - 9, string.Concat("Health: ", stats.CurrentHealth, " / ", stats.MaxHealth));
            Console.Print(2, GameSettings.GAME_HEIGHT - 7, string.Concat("Mana: ", stats.CurrentMana, " / ", stats.MaxMana));
            Console.Print(2, GameSettings.GAME_HEIGHT - 5, string.Concat("Level: ", stats.Level));
            Console.Print(2, GameSettings.GAME_HEIGHT - 3, string.Concat("Weapon: ", equipment.MainHandReference == EntityReference.Null ? "Fist" : equipment.MainHandReference.Entity.Get<Name>().EntityName));
        }

        private void RenderGameLog()
        {
            Console.DrawRLTKStyleBox(
                GameSettings.GAME_WIDTH / 4, 
                GameSettings.GAME_HEIGHT - 11, 
                GameSettings.GAME_WIDTH / 2 - 1, 
                10, 
                Color.White, 
                Color.Black
            );

            int y = GameSettings.GAME_HEIGHT - 2;
            var LogItems = world.GetLogItems();
            for (int i = 1; i <= Math.Min(9, LogItems.Count); i++)
            {
                Console.Print(GameSettings.GAME_WIDTH / 4 + 2, y, LogItems[LogItems.Count - i].ToString());
                y--;
            }
        }

        private void RenderPositionInfo()
        {
            Console.DrawRLTKStyleBox(
                GameSettings.GAME_WIDTH * 3 / 4,
                GameSettings.GAME_HEIGHT - 11,
                GameSettings.GAME_WIDTH / 4 - 1,
                10,
                Color.White,
                Color.Black
            );

            var position = world.PlayerReference.Entity.Get<Position>().Point;
            string itemName = string.Empty;
            var entitiesAtLocation = world.PhysicsWorld.GetEntitiesAtLocation(position);
            if (entitiesAtLocation != null && entitiesAtLocation.Any(a => a.Entity.Has<Item>() || a.Entity.Has<Exit>()))
            {
                var item = entitiesAtLocation.Where(a => a.Entity.Has<Item>() || a.Entity.Has<Exit>()).FirstOrDefault();
                itemName = item.Entity.Get<Name>().EntityName;
            }


            Console.Print(GameSettings.GAME_WIDTH * 3 / 4 + 2, GameSettings.GAME_HEIGHT - 9, string.Concat("Tomb: ", world.Tomb.Mage, " (", world.Tomb.Element.ToString(), ")"));
            Console.Print(GameSettings.GAME_WIDTH * 3 / 4 + 2, GameSettings.GAME_HEIGHT - 7, string.Concat("Depth: ", world.Tomb.CurrentLevel + 1, "/", world.Tomb.Levels.Count() + 1));;

            Console.Print(GameSettings.GAME_WIDTH * 3 / 4 + 2, GameSettings.GAME_HEIGHT - 5, string.Concat("Position: ", position));
            Console.Print(GameSettings.GAME_WIDTH * 3 / 4 + 2, GameSettings.GAME_HEIGHT - 3, string.Concat("Ground: ", itemName));
        }
    }
}

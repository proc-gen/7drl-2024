using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.ECS.Systems.RenderSystems;
using Magi.ECS.Systems.UpdateSystems;
using Magi.Maps;
using Magi.Maps.Generators;
using Magi.Maps.Spawners;
using Magi.Scenes;
using Magi.UI.Windows;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI
{
    public class GameScreen : ScreenObject
    {
        RootScreen RootScreen;
        ScreenSurface screen;
        GameWorld world;

        List<IRenderSystem> renderSystems;
        List<IUpdateSystem> updateSystems;
        List<Window> windows;

        public GameScreen(RootScreen rootScreen, bool loadGame)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

            Children.Add(screen);

            if(loadGame)
            {
                LoadGame();
            }
            else
            {
                NewGame();
            }            

            InitWindows();
            InitSystems();

            world.CurrentState = GameState.AwaitingPlayerInput;
        }

        private void NewGame()
        {
            world = SaveGameManager.NewGame();

            GoNextLevel();
        }

        private void GoNextLevel()
        {
            if (world.PlayerReference != EntityReference.Null)
            {
                world.RemoveAllNonPlayerOwnedEntities();
            }

            var generator = new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            generator.Generate();

            world.Map = generator.Map;

            var startPosition = generator.GetPlayerStartingPosition();
            if (world.PlayerReference == EntityReference.Null)
            {
                new PlayerSpawner().SpawnPlayer(world, startPosition);
            }
            else
            {
                var position = world.PlayerReference.Entity.Get<Position>();
                position.Point = generator.GetPlayerStartingPosition();
                world.PlayerReference.Entity.Set(position);
                world.PhysicsWorld.AddEntity(world.PlayerReference, position.Point);
                world.LogItems.Add(new LogItem("You have descended to the next level"));
            }

            FieldOfView.CalculatePlayerFOV(world);

            var enemyTable = new RandomTable<string>();
            foreach (var enemy in EnemySpawner.EnemyContainers)
            {
                enemyTable = enemyTable.Add(enemy.Key, 1);
            }
            var itemTable = new RandomTable<string>();
            foreach (var item in ItemSpawner.ItemContainers)
            {
                itemTable = itemTable.Add(item.Key, 1);
            }

            generator.SpawnEntitiesForMap(world, enemyTable, itemTable);
            generator.SpawnExitForMap(world);

            world.CurrentState = GameState.AwaitingPlayerInput;
        }

        private void LoadGame()
        {
            world = SaveGameManager.LoadGame();
        }

        private void InitSystems()
        {
            renderSystems = new List<IRenderSystem>()
            {
                new RenderMapSystem(world),
                new RenderRenderablesSystem(world),
            };

            updateSystems = new List<IUpdateSystem>()
            {
                new NonPlayerInputSystem(world),
                new UseItemSystem(world),
                new EntityActSystem(world),
                new MeleeAttackSystem(world),
                new RangedAttackSystem(world),
                new DeathSystem(world),
                new LevelUpSystem(world),
            };
        }

        private void InitWindows()
        {
            var targetingOverlay = new TargetingOverlay(world);

            windows = new List<Window>()
            {
                new GameWindow(world, targetingOverlay),
                new InventoryWindow(world),
                targetingOverlay,
                new LevelUpWindow(world)
            };

            foreach(var window in windows)
            {
                Children.Add(window.Console);
            }
        }

        public override void Update(TimeSpan delta)
        {
            if (world.CurrentState == GameState.AwaitingPlayerInput
                || world.CurrentState == GameState.ShowInventory
                || world.CurrentState == GameState.Targeting
                || world.CurrentState == GameState.LevelUp)
            {
                HandleKeyboard();
            }
            else if (world.CurrentState == GameState.PlayerTurn
                    || world.CurrentState == GameState.MonsterTurn)
            {
                foreach (var system in updateSystems)
                {
                    system.Update(delta);
                }

                switch (world.CurrentState)
                {
                    case GameState.PlayerTurn:
                        world.CurrentState = GameState.MonsterTurn;
                        break;
                    case GameState.MonsterTurn:
                        world.CurrentState = GameState.AwaitingPlayerInput;
                        break;
                    case GameState.PlayerDeath:
                        GoToMainMenu(false);
                        break;

                }
            }

            foreach(var window in windows)
            {
                window.Update(delta);
            }

            base.Update(delta);
        }

        private void HandleKeyboard()
        {
            var keyboard = Game.Instance.Keyboard;
            bool handled = false;
            for(int i = windows.Count - 1; i >= 0; i--) 
            {
                if (windows[i].Visible)
                {
                    handled = windows[i].HandleKeyboard(keyboard);
                    break;
                }
            }

            if(!handled && windows.Where(a => a.Visible).Count() == 1)
            {
                if(keyboard.IsKeyPressed(Keys.Escape))
                {
                    GoToMainMenu(true);
                }
                else if (keyboard.IsKeyPressed(Keys.D))
                {
                    var entitiesAtLocation = world.PhysicsWorld.GetEntitiesAtLocation(world.PlayerReference.Entity.Get<Position>().Point);
                    if (entitiesAtLocation != null && entitiesAtLocation.Where(a => a.Entity.Has<Exit>()).Any())
                    {
                        GoNextLevel();
                    }
                }
            }
        }

        private void GoToMainMenu(bool saveGame)
        {
            if(saveGame)
            {
                SaveGameManager.SaveGame(world);
            }
            RootScreen.SwitchScreen(Screens.MainMenu, true);
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            foreach (IRenderSystem renderSystem in renderSystems)
            {
                renderSystem.Render(screen);
            }
            screen.Render(delta);

            foreach (var window in windows)
            {
                if (window.Visible)
                {
                    window.Render(delta);
                }
            }
        }
    }
}

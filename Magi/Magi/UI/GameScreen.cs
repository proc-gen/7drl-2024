using Magi.Constants;
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

            var generator = new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            generator.Generate();

            world.Map = generator.Map;
            new PlayerSpawner().SpawnPlayer(world, generator.GetPlayerStartingPosition());
            FieldOfView.CalculatePlayerFOV(world);

            var enemyTable = new RandomTable<string>();
            foreach (var enemy in EnemySpawner.EnemyContainers)
            {
                enemyTable = enemyTable.Add(enemy.Key, 1);
            }
            var itemTable = new RandomTable<string>();
            foreach(var item in ItemSpawner.ItemContainers)
            {
                itemTable = itemTable.Add(item.Key, 1);
            }

            generator.SpawnEntitiesForMap(world, enemyTable, itemTable);
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
                new DeathSystem(world),
            };
        }

        private void InitWindows()
        {
            windows = new List<Window>()
            {
                new GameWindow(world),
                new InventoryWindow(
                    GameSettings.GAME_WIDTH / 4,
                    GameSettings.GAME_HEIGHT / 4 - 5,
                    GameSettings.GAME_WIDTH / 2,
                    GameSettings.GAME_HEIGHT / 2,
                    world),
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
                || world.CurrentState == GameState.Targeting)
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

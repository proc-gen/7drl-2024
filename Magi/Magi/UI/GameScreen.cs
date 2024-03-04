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
        Generator generator;
        GameWorld world;

        List<IRenderSystem> renderSystems;
        List<IUpdateSystem> updateSystems;
        List<Window> windows;

        public GameScreen(RootScreen rootScreen, bool loadGame)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

            Children.Add(screen);

            world = new GameWorld();

            generator = new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            generator.Generate();

            world.Map = generator.Map;
            new PlayerSpawner().SpawnPlayer(world, generator.GetPlayerStartingPosition());
            FieldOfView.CalculatePlayerFOV(world);

            InitWindows();
            InitSystems();

            world.CurrentState = GameState.AwaitingPlayerInput;
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
                new EntityActSystem(world),
            };
        }

        private void InitWindows()
        {
            windows = new List<Window>()
            {
                new GameWindow(world),
            };
        }

        public override void Update(TimeSpan delta)
        {
            if (world.CurrentState == GameState.AwaitingPlayerInput)
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
                        GoToMainMenu();
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
                    if (windows[i].HandleKeyboard(keyboard))
                    {
                        handled = true;
                        break;
                    }
                }
            }

            if(!handled && windows.Where(a => a.Visible).Count() == 1)
            {
                if(keyboard.IsKeyPressed(Keys.Escape))
                {
                    GoToMainMenu();
                }
            }
        }

        private void GoToMainMenu()
        {
            RootScreen.SwitchScreen(Screens.MainMenu, true);
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            foreach (IRenderSystem renderSystem in renderSystems)
            {
                renderSystem.Render(screen);
            }

            foreach(var window in windows)
            {
                if (window.Visible)
                {
                    window.Render(delta);
                }
            }
            
            screen.Render(delta);
        }
    }
}

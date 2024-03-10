using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.ECS.Systems.RealTimeUpdateSystems;
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

        public GameScreen(RootScreen rootScreen, string playerClass)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

            Children.Add(screen);

            if(string.IsNullOrEmpty(playerClass))
            {
                LoadGame();
            }
            else
            {
                NewGame(playerClass);
            }            

            InitWindows();
            InitSystems();
        }

        private void NewGame(string playerClass)
        {
            world = SaveGameManager.NewGame();
            world.PlayerClass = playerClass;
            world.GoNextLevel();
        }

        private void LoadGame()
        {
            world = SaveGameManager.LoadGame();
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
                new CountdownSystem(world),
                new ReplenishmentSystem(world),
                new NonPlayerInputSystem(world),
                new UseItemSystem(world),
                new EntityActSystem(world),
                new MeleeAttackSystem(world),
                new RangedAttackSystem(world),
                new SkillAttackSystem(world),
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
                new EffectOverlay(world),
                new InventoryWindow(world),
                targetingOverlay,
                new LevelUpWindow(world),
                new SkillAcquiredWindow(world),
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
                || world.CurrentState == GameState.LevelUp
                || world.CurrentState == GameState.SkillAcquired)
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
                if (windows[i].Visible && windows[i].GetType() != typeof(EffectOverlay))
                {
                    handled = windows[i].HandleKeyboard(keyboard);
                    break;
                }
            }

            if(!handled && windows.Where(a => a.Visible).Count() == 2)
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
                        world.GoNextLevel();
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

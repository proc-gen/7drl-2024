using Magi.Constants;
using Magi.ECS.Systems.RenderSystems;
using Magi.Maps;
using Magi.Maps.Generators;
using Magi.Maps.Spawners;
using Magi.Scenes;
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

            renderSystems = new List<IRenderSystem>()
            {
                new RenderMapSystem(world),
                new RenderRenderablesSystem(world),
            };
        }

        public override void Update(TimeSpan delta)
        {
            HandleKeyboard();
            base.Update(delta);
        }

        private void HandleKeyboard()
        {
            var keyboard = Game.Instance.Keyboard;
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                RootScreen.SwitchScreen(Screens.MainMenu, true);
            }
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            foreach (IRenderSystem renderSystem in renderSystems)
            {
                renderSystem.Render(screen);
            }
            
            screen.Render(delta);
        }
    }
}

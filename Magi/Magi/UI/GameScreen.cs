using Magi.Constants;
using Magi.Maps;
using Magi.Maps.Generators;
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

        public GameScreen(RootScreen rootScreen, bool loadGame)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

            Children.Add(screen);

            world = new GameWorld();

            generator = new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            generator.Generate();

            world.Map = generator.Map;
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
            
            for(int i = 0; i < world.Map.Width; i++)
            {
                for(int j = 0; j < world.Map.Height; j++)
                {
                    var tile = world.Map.GetTile(i, j);
                    screen.Surface[i, j].Background = tile.BackgroundColor;
                    if(tile.Glyph > 0)
                    {
                        screen.Surface[i, j].Foreground = tile.GlyphColor;
                        screen.Surface[i, j].Glyph = tile.Glyph;
                    }
                }
            }

            screen.Render(delta);
        }
    }
}

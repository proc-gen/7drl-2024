using Magi.Constants;
using Magi.Scenes;
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

        public GameScreen(RootScreen rootScreen, bool loadGame)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);

            Children.Add(screen);
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
            screen.Print(20, 20, "Game Screen");
            screen.Render(delta);
        }
    }
}

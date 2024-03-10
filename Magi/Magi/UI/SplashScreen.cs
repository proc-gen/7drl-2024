using Magi.Constants;
using Magi.Scenes;
using Magi.UI.Helpers;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI
{
    public class SplashScreen : MagiScreen
    {
        RootScreen RootScreen;
        ScreenSurface screen;
        float elapsedTime = 0f;

        public SplashScreen(RootScreen rootScreen)
        {
            RootScreen = rootScreen;
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        }

        public override void Activate()
        {
        }

        public override void Update(TimeSpan delta)
        {
            var keyboard = Game.Instance.Keyboard;
            if (elapsedTime > 3f || keyboard.HasKeysDown)
            {
                RootScreen.AddScreen(Screens.MainMenu, new MainMenuScreen(RootScreen));
            }
            elapsedTime += (float)delta.TotalSeconds;
            base.Update(delta);
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            screen.DrawRLTKStyleBox(screen.Width / 2 - 15, 13, 30, 4, Color.White, Color.Black);
            screen.Print(screen.Width / 2 - 11, 15, "Land of the Fallen Magi", Color.Yellow, Color.Black);
            screen.Render(delta);
        }
    }
}

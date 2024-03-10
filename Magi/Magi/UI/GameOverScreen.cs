using Magi.Constants;
using Magi.Scenes;
using Magi.UI.Helpers;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI
{
    public class GameOverScreen : MagiScreen
    {
        RootScreen RootScreen;
        ScreenSurface screen;
        GameWorld world;

        InputDelayHelper InputDelayHelper;

        public GameOverScreen(RootScreen rootScreen, GameWorld world)
        {
            RootScreen = rootScreen;
            InputDelayHelper = new InputDelayHelper();
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            this.world = world;
        }

        public override void Activate()
        {
            InputDelayHelper.Reset();
        }

        public override void Update(TimeSpan delta)
        {
            InputDelayHelper.Update(delta);
            if (InputDelayHelper.ReadyForInput)
            {
                HandleKeyboard();
            }
            base.Update(delta);
        }

        private void HandleKeyboard()
        {
            var keyboard = Game.Instance.Keyboard;

            if (keyboard.IsKeyDown(Keys.Enter))
            {
                RootScreen.SwitchScreen(Screens.MainMenu, true);
            }
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            screen.Print(screen.Width / 2 - 22, 15, "It's the end of the line for you, justicar...", Color.White, Color.Black);

            RenderGameLog();
            RenderGameStats();

            screen.Print(screen.Width / 2 - 19, 50, "Press enter to go back to the main menu", Color.White, Color.Black);
            screen.Render(delta);
        }

        private void RenderGameLog()
        {
            int y = 27;
            var LogItems = world.GetLogItems();
            for (int i = 1; i <= Math.Min(9, LogItems.Count); i++)
            {
                screen.Print(GameSettings.GAME_WIDTH / 4 + 2, y, LogItems[LogItems.Count - i].ToString());
                y--;
            }
        }

        private void RenderGameStats()
        {
            string tombClearedText = string.Concat("You cleared ", world.TombsCleared, " tomb", world.TombsCleared == 1 ? "" : "s");
            screen.Print(screen.Width / 2 - tombClearedText.Length / 2, 30, tombClearedText, Color.White, Color.Black);

            int x = 0;
            int y = 32;
            foreach(var entry in world.ConfirmedKills)
            {
                screen.Print(screen.Width / 5 * x - 10, y, string.Concat(entry.Key, ": ", entry.Value));
                x++;
                if(x == 5)
                {
                    x = 0;
                    y++;
                }
            }
        }
    }
}

using Magi.Constants;
using Magi.Maps.Spawners;
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
    public class ClassSelectionScreen : ScreenObject
    {
        RootScreen RootScreen;
        ScreenSurface screen;

        int selectedClass = 0;
        InputDelayHelper InputDelayHelper;
        public ClassSelectionScreen(RootScreen rootScreen)
        {
            RootScreen = rootScreen;
            InputDelayHelper = new InputDelayHelper();
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
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

            if (keyboard.IsKeyPressed(Keys.Left) && selectedClass > 0)
            {
                selectedClass--;
            }
            else if (keyboard.IsKeyPressed(Keys.Right) && selectedClass < 2)
            {
                selectedClass++;
            }
            else if (keyboard.IsKeyDown(Keys.Enter))
            {
                RootScreen.AddScreen(Screens.Game, new GameScreen(RootScreen, false));
            }
            else if(keyboard.IsKeyDown(Keys.Escape))
            {
                RootScreen.SwitchScreen(Screens.MainMenu, true);
            }
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            screen.Print(screen.Width / 2 - 7, 15, "Select a class:", Color.White, Color.Black);

            for(int i = 0; i < PlayerSpawner.PlayerContainers.Count; i++)
            {
                int horizontalOffset = screen.Width / 4 + ((screen.Width / 4) * i);
                screen.DrawRLTKStyleBox(horizontalOffset - 10, 20, 20, 20, selectedClass == i ? Color.Yellow : Color.White, Color.Black);
                var container = PlayerSpawner.PlayerContainers.Values.ElementAt(i);

                screen.Print(horizontalOffset - container.Name.Length / 2, 22, container.Name, Color.White, Color.Black);
                screen.Print(horizontalOffset - 8, 24, string.Concat("Strength: ", container.Strength), Color.White, Color.Black);
                screen.Print(horizontalOffset - 8, 25, string.Concat("Dexterity: ", container.Dexterity), Color.White, Color.Black);
                screen.Print(horizontalOffset - 8, 26, string.Concat("Intelligence: ", container.Intelligence), Color.White, Color.Black);
                screen.Print(horizontalOffset - 8, 27, string.Concat("Vitality: ", container.Vitality), Color.White, Color.Black);
            }

            screen.Print(screen.Width / 2 - 29, 50, "Press enter to start or escape to go back to the main menu", Color.White, Color.Black);
            screen.Render(delta);
        }
    }
}

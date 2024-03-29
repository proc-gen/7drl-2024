﻿using Magi.Constants;
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
    public class MainMenuScreen : MagiScreen
    {
        RootScreen RootScreen;

        ScreenSurface screen;

        int selectedButtonIndex = 0;
        InputDelayHelper InputDelayHelper;
        public MainMenuScreen(RootScreen rootScreen)
        {
            RootScreen = rootScreen;
            InputDelayHelper = new InputDelayHelper();
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
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

            if (keyboard.IsKeyPressed(Keys.Up) && selectedButtonIndex > 0)
            {
                if (showContinue())
                {
                    selectedButtonIndex--;
                }
                else
                {
                    selectedButtonIndex = 0;
                }
            }
            else if (keyboard.IsKeyPressed(Keys.Down) && selectedButtonIndex < 2)
            {
                if (showContinue())
                {
                    selectedButtonIndex++;
                }
                else
                {
                    selectedButtonIndex = 2;
                }
            }
            else if (keyboard.IsKeyDown(Keys.Enter))
            {
                switch (selectedButtonIndex)
                {
                    case 0:
                        RootScreen.AddScreen(Screens.ClassSelection, new ClassSelectionScreen(RootScreen));
                        break;
                    case 1:
                        RootScreen.AddScreen(Screens.Game, new GameScreen(RootScreen, string.Empty));
                        break;
                    case 2:
                        Game.Instance.MonoGameInstance.Exit();
                        break;
                }
            }
        }

        private void printTitle()
        {
            screen.DrawRLTKStyleBox(screen.Width / 2 - 15, 13, 30, 4, Color.White, Color.Black);
            screen.Print(screen.Width / 2 - 11, 15, "Land of the Fallen Magi", Color.Yellow, Color.Black);
        }

        private void printMenuOption(int x, int y, string text, bool active)
        {
            screen.Print(x, y, text, Color.White, Color.Black);
            if (active)
            {
                screen.Print(x - 3, y, "->", Color.White, Color.Black);
            }
        }

        private bool showContinue()
        {
            return File.Exists("savegame.json");
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            printTitle();

            int optionPosition = 24;

            printMenuOption(screen.Width / 2 - 4, optionPosition, "New Game", selectedButtonIndex == 0);
            optionPosition++;

            if (showContinue())
            {
                printMenuOption(screen.Width / 2 - 6, optionPosition, "Continue Game", selectedButtonIndex == 1);
                optionPosition++;
            }

            printMenuOption(screen.Width / 2 - 4, optionPosition, "Quit Game", selectedButtonIndex == 2);

            screen.Render(delta);
        }
    }
}

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
    public class SplashScreen : ScreenObject
    {
        RootScreen RootScreen;
        ScreenSurface screen;
        InputDelayHelper inputDelayHelper, textDelayHelper;

        int line = 0;
        string[] splashText = 
        [
            "In the world of Elison, magic power doesn't disappear when a wielder dies.",
            "Instead, it merges with the land, usually resulting in benefits for the surrounding land.",
            "The tomb of a fire mage providing warmth for a town through the winter.",
            "The tomb of a lightning mage acting as a lightning rod to prevent storm damage.",
            "You get the idea.",
            "",
            "Recently, however, these tombs have also been causing problems.",
            "The tomb of an ice mage giving farm animals frostbite in the summer.",
            "The tomb of a fire mage causing constant forest fires.",
            "The gamble of using these tombs is becoming too much to bear.",
            "",
            "In order to combat the growing number of catastrophies, we've created a new group:",
            "The Magic Justicars",
            "Your job will to be go into the tombs of these fallen magi and deal with the problem.",
            "",
            "Best of luck, Justicar"
        ];

        public SplashScreen(RootScreen rootScreen)
        {
            RootScreen = rootScreen;
            inputDelayHelper = new InputDelayHelper();
            textDelayHelper = new InputDelayHelper(.5f);
            screen = new ScreenSurface(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
        }

        public override void Update(TimeSpan delta)
        {
            var keyboard = Game.Instance.Keyboard;
            inputDelayHelper.Update(delta);
            textDelayHelper.Update(delta);

            if ((inputDelayHelper.ReadyForInput && keyboard.HasKeysPressed) || textDelayHelper.ReadyForInput)
            {
                line++;
                inputDelayHelper.Reset();
                textDelayHelper.Reset();
            }

            if ((line >= splashText.Length && keyboard.HasKeysPressed) || keyboard.IsKeyPressed(Keys.Escape))
            {
                RootScreen.AddScreen(Screens.MainMenu, new MainMenuScreen(RootScreen));
            }

            base.Update(delta);
        }

        public override void Render(TimeSpan delta)
        {
            screen.Clear();
            
            for (int i = 0; i < line; i++)
            {
                if (i < splashText.Length)
                {
                    screen.Print(screen.Width / 2 - 45, 20 + i, splashText[i]);
                } 
            }
            
            screen.Render(delta);
        }
    }
}

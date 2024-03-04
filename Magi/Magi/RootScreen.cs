﻿using Magi.Constants;
using Magi.UI;

namespace Magi.Scenes
{
    public class RootScreen : ScreenObject
    {
        Dictionary<Screens, ScreenObject> ActiveScreens;
        Screens ActiveScreen;

        public RootScreen()
        {
            ActiveScreens = new Dictionary<Screens, ScreenObject>
            {
                { Screens.MainMenu, new MainMenuScreen(this) }
            };

            ActiveScreen = Screens.MainMenu;
        }

        public override void Update(TimeSpan delta)
        {
            ActiveScreens[ActiveScreen].Update(delta);
            base.Update(delta);
        }

        public override void Render(TimeSpan delta)
        {
            ActiveScreens[ActiveScreen].Render(delta);
            base.Render(delta);
        }

        public void AddScreen(Screens screen, ScreenObject screenObject, bool makeActive = true)
        {
            ActiveScreens.Add(screen, screenObject);
            if (makeActive)
            {
                ActiveScreen = screen;
            }
        }

        public void RemoveScreen(Screens screen)
        {
            ActiveScreens.Remove(screen);
        }

        public void SwitchScreen(Screens screen, bool removeOthers = false)
        {
            ActiveScreen = screen;
            if (removeOthers)
            {
                foreach (var key in ActiveScreens.Keys.Where(a => a != screen))
                {
                    ActiveScreens.Remove(key);
                }
            }
        }

        public bool HasScreen(Screens screen)
        {
            return ActiveScreens.ContainsKey(screen);
        }
    }
}

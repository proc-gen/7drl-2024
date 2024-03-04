using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public abstract class Window
    {
        public Console Console { get; protected set; }

        public bool Visible { get; set; }

        public Window() 
        {
            Console = new Console(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            Console.Position = Point.Zero;
        }

        public Window(int x, int y, int width, int height)
        {
            Console = new Console(width, height);
            Console.Position = new Point(x, y);
        }

        public abstract void Update(TimeSpan delta);

        public abstract bool HandleKeyboard(Keyboard keyboard);

        public abstract void Render(TimeSpan delta);
    }
}

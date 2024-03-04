using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public class GameWindow : Window
    {
        GameWorld world;
        public GameWindow(int x, int y, int width, int height, GameWorld world) 
            : base(x, y, width, height)
        {
            this.world = world;
        }

        public override void HandleKeyboard(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                RequestMoveDirection(Direction.Up);
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                RequestMoveDirection(Direction.Down);
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                RequestMoveDirection(Direction.Left);
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                RequestMoveDirection(Direction.Right);
            }
            else if (keyboard.IsKeyPressed(Keys.Space))
            {
                RequestMoveDirection(Direction.None);
            }
        }

        private void RequestMoveDirection(Direction direction)
        {
            world.StartPlayerTurn(direction == Direction.None
                                    ? Point.None
                                    : new Point(direction.DeltaX, direction.DeltaY));
        }

        public override void Render(TimeSpan delta)
        {
        }

        public override void Update(TimeSpan delta)
        {

        }
    }
}

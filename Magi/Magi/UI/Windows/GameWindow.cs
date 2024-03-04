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
        public GameWindow(GameWorld world) 
            : base()
        {
            this.world = world;
            Visible = true;
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            bool retVal = false;
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                RequestMoveDirection(Direction.Up);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                RequestMoveDirection(Direction.Down);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                RequestMoveDirection(Direction.Left);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                RequestMoveDirection(Direction.Right);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Space))
            {
                RequestMoveDirection(Direction.None);
                retVal = true;
            }

            return retVal;
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

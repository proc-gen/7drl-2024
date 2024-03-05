using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public class TargetingOverlay : Overlay
    {
        const float AlphaFactor = 0.8f;

        GameWorld World;
        EntityReference Source;
        EntityReference Target;
        Point Start;
        Point End;

        public TargetingOverlay(GameWorld world)
            : base()
        {
            World = world;
            Source = Target = EntityReference.Null;
            Start = End = Point.None;
            Console.Surface.DefaultBackground = new Color(0, 0, 0, 0);
        }

        public void SetEntityForTargeting(EntityReference source)
        {
            Source = source;
            Start = World.PlayerReference.Entity.Get<Position>().Point;
            End = Start + new Point(1, 0);
        }

        private void ClearTargetingData()
        {
            Source = Target = EntityReference.Null;
            Start = End = Point.None;
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            bool retVal = false;
            if (keyboard.IsKeyPressed(Keys.Escape))
            {
                Visible = false;
                ClearTargetingData();
                World.CurrentState = Constants.GameState.AwaitingPlayerInput;
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Up))
            {
                MoveTarget(Direction.Up);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                MoveTarget(Direction.Down);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Left))
            {
                MoveTarget(Direction.Left);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Right))
            {
                MoveTarget(Direction.Right);
                retVal = true;
            }
            else if (keyboard.IsKeyPressed(Keys.Enter))
            {
                if (Target != EntityReference.Null)
                {
                    World.World.Create(new RangedAttack() { Source = World.PlayerReference, Target = Target });
                    World.StartPlayerTurn(Point.None);
                    Visible = false;
                }
                retVal = true;
            }

            return retVal;
        }

        private void MoveTarget(Direction direction)
        {
            if (World.PlayerFov.Contains(End + direction))
            {
                End += direction;

                var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(End);
                if (entitiesAtLocation == null || !entitiesAtLocation.Where(a => a.Entity.Has<Blocker>()).Any())
                {
                    Target = EntityReference.Null;
                }
                else
                {
                    Target = entitiesAtLocation.Where(a => a.Entity.Has<Blocker>()).First();
                }
            }
        }

        public override void Update(TimeSpan delta)
        {
            if (!Visible && World.CurrentState == Constants.GameState.Targeting)
            {
                Visible = true;
            }
        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            if (Source != EntityReference.Null)
            {
                RenderTitle();
                RenderTrajectory();
            }
            Console.Render(delta);
        }

        private void RenderTitle()
        {
            string title = string.Concat("Targeting: ", Source.Entity.Get<Name>().EntityName);
            Console.Print(Console.Width / 2 - title.Length / 2, 5, title, Color.White, Color.Black);
        }

        private void RenderTrajectory()
        {
            var lineColor = new Color(TrajectoryColor(), AlphaFactor);
            int minX = Start.X - GameSettings.GAME_WIDTH / 2;
            int minY = Start.Y - GameSettings.GAME_HEIGHT / 2;
            Console.DrawLine(Start - new Point(minX, minY), End - new Point(minX, minY), (char)219, lineColor);
        }

        private Color TrajectoryColor()
        {
            if (Start == End)
            {
                return Color.Red;
            }

            return Target == EntityReference.Null ? Color.Yellow : Color.Green;
        }
    }
}

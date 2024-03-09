using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
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
        int SourceRange;
        int EffectRange;
        TargetingType TargetingType;
        TargetSpace TargetSpace;
        EntityReference Target;
        Point Start;
        Point End;

        public TargetingOverlay(GameWorld world)
            : base()
        {
            World = world;
            ClearTargetingData();
            Console.Surface.DefaultBackground = new Color(0, 0, 0, 0);
        }

        public void SetEntityForTargeting(EntityReference source)
        {
            Source = source;
            if (source.Entity.Has<Weapon>())
            {
                SourceRange = source.Entity.Get<Weapon>().Range;
                TargetingType = TargetingType.SingleTargetDamage;
                TargetSpace = TargetSpace.Enemy;
                EffectRange = 0;
            }
            else
            {
                var skillInfo = source.Entity.Get<Skill>();
                SourceRange = skillInfo.TargetRange;
                TargetingType = skillInfo.TargetingType;
                TargetSpace = skillInfo.TargetSpace;
                EffectRange = skillInfo.EffectRange;
            }
            Start = World.PlayerReference.Entity.Get<Position>().Point;
            End = Start + new Point(1, 0);
        }

        private void ClearTargetingData()
        {
            Source = Target = EntityReference.Null;
            Start = End = Point.None;
            SourceRange = 0;
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
                if (!IsPathBlocked())
                {
                    bool attacked = false;
                    if (Source.Entity.Has<Weapon>() && Target != EntityReference.Null)
                    {
                        World.World.Create(new RangedAttack() { Source = World.PlayerReference, Target = Target });
                        attacked = true;
                    }
                    else
                    {
                        var skillInfo = Source.Entity.Get<Skill>();
                        if((skillInfo.TargetSpace == Constants.TargetSpace.Any || skillInfo.TargetSpace == Constants.TargetSpace.Enemy) && Target != EntityReference.Null)
                        {
                            World.World.Create(new SkillAttack() { Source = World.PlayerReference, SourceSkill = Source, Target = Target, TargetLocation = End, TurnsLeft = skillInfo.LifetimeTurns });
                            attacked = true;
                        }
                        else if ((skillInfo.TargetSpace == Constants.TargetSpace.Any || skillInfo.TargetSpace == Constants.TargetSpace.Empty) && Target == EntityReference.Null)
                        {
                            World.World.Create(new SkillAttack() { Source = World.PlayerReference, SourceSkill = Source, Target = Target, TargetLocation = End, TurnsLeft = skillInfo.LifetimeTurns });
                            attacked = true;
                        }
                        else if ((skillInfo.TargetSpace == Constants.TargetSpace.Any || skillInfo.TargetSpace == Constants.TargetSpace.Wall) && World.Map.GetTile(End).BaseTileType == Constants.TileTypes.Wall)
                        {
                            World.World.Create(new SkillAttack() { Source = World.PlayerReference, SourceSkill = Source, Target = Target, TargetLocation = End, TurnsLeft = skillInfo.LifetimeTurns });
                            attacked = true;
                        }
                    }

                    if (attacked)
                    {
                        World.StartPlayerTurn(Point.None);
                        Visible = false;
                    }
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
            var pointsInLine = FieldOfView.GetPointsInLine(Start, End);
            foreach ( var point in pointsInLine )
            {
                Console.SetGlyph(point.X - minX, point.Y - minY, (char)219, lineColor);
            }
            if(EffectRange > 0 && lineColor.FillAlpha() != Color.Red)
            {
                var aoePoints = FieldOfView.CalculateFOV(World, End, EffectRange + 1, false);
                foreach (var point in aoePoints)
                {
                    Console.SetGlyph(point.X - minX, point.Y - minY, (char)219, lineColor);
                }
            }
        }

        private Color TrajectoryColor()
        {
            if (IsPathBlocked())
            {
                return Color.Red;
            }
            else if(TargetSpace == TargetSpace.Empty)
            {
                var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(End);
                if(entitiesAtLocation != null && entitiesAtLocation.Where(a => a.Entity.Has<Blocker>()).Any())
                {
                    return Color.Red;
                }
                else
                {
                    return Color.Green;
                }
            }
            else if(TargetSpace == TargetSpace.Wall)
            {
                var tile = World.Map.GetTile(End);
                if(tile.BaseTileType == TileTypes.Floor)
                {
                    return Color.Red;
                }
                else
                {
                    return Color.Green;
                }
            }

            return Target == EntityReference.Null ? Color.Yellow : Color.Green;
        }

        private bool IsPathBlocked()
        {
            return World.Map.IsPathBlocked(Start, End, SourceRange, TargetSpace);
        }
    }
}

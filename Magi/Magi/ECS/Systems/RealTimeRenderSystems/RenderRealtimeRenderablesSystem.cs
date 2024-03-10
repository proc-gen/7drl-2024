using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.ECS.Systems.RenderSystems;
using Magi.Maps;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.RealTimeRenderSystems
{
    public class RenderRealtimeRenderablesSystem : ArchSystem, IRenderSystem
    {
        const float AlphaFactor = 0.5f;
        QueryDescription realTimeRenderablesQuery = new QueryDescription().WithAll<Renderable, Position, RealTime>();

        public RenderRealtimeRenderablesSystem(GameWorld world)
            : base(world) 
        { 
        }

        public void Render(ScreenSurface screen)
        {
            var position = World.PlayerReference.Entity.Get<Position>().Point;

            int minX = position.X - GameSettings.GAME_WIDTH / 2;
            int minY = position.Y - GameSettings.GAME_HEIGHT / 2;

            World.World.Query(in realTimeRenderablesQuery, (Entity entity, ref Renderable renderable, ref Position position) =>
            {
                float timeLeft = 1;
                if (entity.Has<TimedLife>())
                {
                    timeLeft = entity.Get<TimedLife>().TimeLeft;
                }
                RenderRenderable(screen, World.Map, minX, minY, renderable, position, timeLeft);
            });
        }

        private void RenderRenderable(ScreenSurface screen, Map map, int minX, int minY, Renderable renderable, Position position, float timeLeft)
        {
            if (position.Point.X - minX >= 0
                        && position.Point.X - minX < GameSettings.GAME_WIDTH
                        && position.Point.Y - minY >= 0
                        && position.Point.Y - minY < GameSettings.GAME_HEIGHT)
            {
                bool inPlayerFov = World.PlayerFov.Contains(position.Point);
                var tile = map.GetTile(position.Point);

                if ((renderable.ShowOutsidePlayerFov && tile.Explored) || inPlayerFov)
                {
                    screen.Surface[position.Point.X - minX, position.Point.Y - minY].Glyph = renderable.Glyph;
                    screen.Surface[position.Point.X - minX, position.Point.Y - minY].Foreground = new Color(renderable.Color * (inPlayerFov ? 1f : 0.75f), AlphaFactor * Math.Min(timeLeft, 1));
                }
            }
        }
    }
}

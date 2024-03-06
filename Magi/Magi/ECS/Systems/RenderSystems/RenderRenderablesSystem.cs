using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Maps;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.RenderSystems
{
    public class RenderRenderablesSystem : ArchSystem, IRenderSystem
    {
        QueryDescription renderEntitiesQuery = new QueryDescription().WithAll<Renderable, Position, Blocker>();
        QueryDescription renderItemsQuery = new QueryDescription().WithAll<Renderable, Position>().WithNone<Blocker>();

        public RenderRenderablesSystem(GameWorld world)
            : base(world)
        {
        }

        public void Render(ScreenSurface screen)
        {
            var position = World.PlayerReference.Entity.Get<Position>().Point;

            int minX = position.X - GameSettings.GAME_WIDTH / 2;
            int minY = position.Y - GameSettings.GAME_HEIGHT / 2;

            World.World.Query(in renderItemsQuery, (ref Renderable renderable, ref Position position) =>
            {
                RenderRenderable(screen, World.Map, minX, minY, renderable, position);
            });

            World.World.Query(in renderEntitiesQuery, (ref Renderable renderable, ref Position position) =>
            {
                RenderRenderable(screen, World.Map, minX, minY, renderable, position);
            });
        }

        private void RenderRenderable(ScreenSurface screen, Map map, int minX, int minY, Renderable renderable, Position position)
        {
            if (position.Point.X - minX >= 0
                        && position.Point.X - minX < map.Width
                        && position.Point.Y - minY >= 0
                        && position.Point.Y - minY < map.Height)
            {
                bool inPlayerFov = World.PlayerFov.Contains(position.Point);
                var tile = map.GetTile(position.Point);

                if ((renderable.ShowOutsidePlayerFov && tile.Explored) || inPlayerFov)
                {
                    screen.Surface[position.Point.X - minX, position.Point.Y - minY].Glyph = renderable.Glyph;
                    screen.Surface[position.Point.X - minX, position.Point.Y - minY].Foreground = renderable.Color * (inPlayerFov ? 1f : 0.75f);
                }
            }
        }
    }
}

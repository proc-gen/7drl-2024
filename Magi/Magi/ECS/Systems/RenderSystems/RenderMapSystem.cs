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
    internal class RenderMapSystem : ArchSystem, IRenderSystem
    {
        public RenderMapSystem(GameWorld world)
            : base(world)
        {
        }

        public void Render(ScreenSurface screen)
        {
            var position = World.PlayerReference.Entity.Get<Position>().Point;

            int minX = position.X - GameSettings.GAME_WIDTH / 2;
            int maxX = position.X + GameSettings.GAME_WIDTH / 2;
            int minY = position.Y - GameSettings.GAME_HEIGHT / 2;
            int maxY = position.Y + GameSettings.GAME_HEIGHT / 2;

            for (int i = minX; i < maxX; i++)
            {
                for (int j = minY; j < maxY; j++)
                {
                    if (i >= 0
                        && i < World.Map.Width
                        && j >= 0
                        && j < World.Map.Height)
                    {
                        var tile = World.Map.GetTile(i, j);
                        if (World.PlayerFov.Contains(new Point(i, j)))
                        {
                            RenderAtPosition(screen, i - minX, j - minY, tile, 1f);
                        }
                        else if (tile.Explored)
                        {
                            RenderAtPosition(screen, i - minX, j - minY, tile, .75f);
                        }
                    }
                }
            }
        }

        private void RenderAtPosition(ScreenSurface screen, int i, int j, Tile tile, float opacity)
        {
            screen.Surface[i, j].Background = tile.BackgroundColor * opacity;
            if (tile.Glyph > 0)
            {
                screen.Surface[i, j].Foreground = tile.GlyphColor * opacity;
                screen.Surface[i, j].Glyph = tile.Glyph;
            }
        }
    }
}

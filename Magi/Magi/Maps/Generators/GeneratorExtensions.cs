using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public static class GeneratorExtensions
    {
        public static void ApplyRoomToMap(this Generator generator, Rectangle room)
        {
            for (int i = room.X + 1; i <= room.MaxExtentX; i++)
            {
                for (int j = room.Y + 1; j <= room.MaxExtentY; j++)
                {
                    generator.Map.SetTile(i, j, i % 2 == j % 2 ? Generator.Floor : Generator.Floor);
                }
            }
        }

        public static void ApplyHorizontalTunnel(this Generator generator, int x1, int x2, int y)
        {
            for (int i = Math.Min(x1, x2); i <= Math.Max(x1, x2); i++)
            {
                generator.Map.SetTile(i, y, i % 2 == y % 2 ? Generator.Floor : Generator.Floor);
            }
        }

        public static void ApplyVerticalTunnel(this Generator generator, int y1, int y2, int x)
        {
            for (int j = Math.Min(y1, y2); j <= Math.Max(y1, y2); j++)
            {
                generator.Map.SetTile(x, j, x % 2 == j % 2 ? Generator.Floor : Generator.Floor);
            }
        }

        public static void ApplyCorridor(this Generator generator, int x1, int y1, int x2, int y2)
        {
            if(Math.Abs(x1 - x2) > Math.Abs(y1 - y2))
            {
                ApplyHorizontalTunnel(generator, x1, x2, y1);
                ApplyVerticalTunnel(generator, y1, y2, x2);
            }
            else
            {
                ApplyVerticalTunnel(generator, y1, y2, x1);
                ApplyHorizontalTunnel(generator, x1, x2, y2);
            }
        }
    }
}

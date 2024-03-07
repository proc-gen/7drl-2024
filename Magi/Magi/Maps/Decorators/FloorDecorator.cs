using Magi.Constants;
using Magi.Maps.Generators;
using Magi.Pathfinding;
using Magi.Utils;
using Microsoft.Xna.Framework.Media;
using SadRogue.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Decorators
{
    public static class FloorDecorator
    {
        public static void Decorate(Generator generator, Elements element)
        {
            Random random = new Random();
            RandomTable<FloorDecorationStyles> decorationTable = new RandomTable<FloorDecorationStyles>()
                .Add(FloorDecorationStyles.SimpleCheckerboard, 1)
                .Add(FloorDecorationStyles.Diamond, 1)
                .Add(FloorDecorationStyles.Distance, 1);

            var style = decorationTable.Roll(random);
            var colors = PickColors(element, random);

            switch (style)
            {
                case FloorDecorationStyles.SimpleCheckerboard:
                    SimpleCheckerboard(generator, colors.Item1.GetDark(), colors.Item2.GetDark()); 
                    break;
                case FloorDecorationStyles.Diamond:
                    Diamond(generator, colors.Item1.GetDark(), colors.Item2.GetDark());
                    break;
                case FloorDecorationStyles.Distance:
                    Distance(generator, colors.Item1.GetDark(), colors.Item2.GetDark());
                    break;
            }
        }

        private static Tuple<Color, Color> PickColors(Elements element, Random random) 
        {
            switch(element)
            {
                case Elements.Air:
                    return new Tuple<Color, Color>(ElementColors.AirVeryLightGrey, ElementColors.AirLightGrey);
                case Elements.Fire:
                    return new Tuple<Color, Color>(ElementColors.FireOrange, ElementColors.FireYellow);
                case Elements.Water:
                    return new Tuple<Color, Color>(ElementColors.WaterFadedBlue, ElementColors.WaterIceBlue);
                case Elements.Earth:
                    return new Tuple<Color, Color>(ElementColors.EarthDarkBrown, ElementColors.EarthLightBrown);
                case Elements.Lightning:
                    return new Tuple<Color, Color>(ElementColors.LightningLightYellow, ElementColors.LightningVeryLightYellow);
                case Elements.Ice:
                default:
                    return new Tuple<Color, Color>(ElementColors.IceCyan, ElementColors.IceBlue);
            }
        }

        private static void SimpleCheckerboard(Generator generator, Color colorA, Color colorB)
        {
            for(int i = 0; i < generator.Map.Width; i++)
            {
                for(int j = 0;  j < generator.Map.Height; j++)
                {
                    var tile = generator.Map.GetTile(i, j);
                    if(tile.BaseTileType == TileTypes.Floor)
                    {
                        tile.BackgroundColor = i % 2 == j % 2 ? colorA : colorB;
                        generator.Map.SetTile(i, j, tile);
                    }
                }
            }
        }

        private static void Diamond(Generator generator, Color colorA, Color colorB)
        {
            for (int i = 0; i < generator.Map.Width; i++)
            {
                for (int j = 0; j < generator.Map.Height; j++)
                {
                    var tile = generator.Map.GetTile(i, j);
                    if (tile.BaseTileType == TileTypes.Floor)
                    {
                        tile.Glyph = i % 2 == 0 ? (char)16 : (char)17;
                        tile.BackgroundColor = colorA;
                        tile.GlyphColor = colorB;
                        generator.Map.SetTile(i, j, tile);
                    }
                }
            }
        }

        private static void Distance(Generator generator, Color colorA, Color colorB)
        {
            SquareGridMapOnly SquareGrid = new SquareGridMapOnly(generator.Map);
            AStarSearch<Location> AStarSearch = new AStarSearch<Location>(SquareGrid);

            var distances = new int[generator.Map.Width * generator.Map.Height];
            for(int i = 0; i < distances.Length; i++)
            {
                distances[i] = -1;
            }

            var exit = new Location(generator.GetPlayerStartingPosition());
            float maxDistance = 0;
            for (int i = 0; i < generator.Map.Width; i++)
            {
                for (int j = 0; j < generator.Map.Height; j++)
                {
                    var tile = generator.Map.GetTile(i, j);
                    if (tile.BaseTileType == TileTypes.Floor)
                    {
                        Point end = new Point(i, j);
                        distances[j * generator.Map.Width + i] = AStarSearch.DistanceToPoint(exit, new Location(end));
                        if(distances[j * generator.Map.Width + i] > maxDistance)
                        {
                            maxDistance = distances[j * generator.Map.Width + i];
                        }
                    }
                }
            }

            for (int i = 0; i < generator.Map.Width; i++)
            {
                for (int j = 0; j < generator.Map.Height; j++)
                {
                    var tile = generator.Map.GetTile(i, j);
                    if (tile.BaseTileType == TileTypes.Floor && distances[j * generator.Map.Width + i] >= 0)
                    {
                        tile.BackgroundColor = Color.Lerp(colorA, colorB, distances[j * generator.Map.Width + i] / maxDistance);
                        generator.Map.SetTile(i, j, tile);
                    }
                }
            }
                    }
    }
}

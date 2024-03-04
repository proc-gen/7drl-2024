using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public class RandomGenerator : Generator
    {
        static Tile Wall = new Tile()
        {
            BaseTileType = Constants.TileTypes.Wall,
            BackgroundColor = new Color(0f, 0f, .5f)
        };
        static Tile FloorA = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.LightGray
        };
        static Tile FloorB = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.WhiteSmoke
        };

        List<Rectangle> Rooms { get; set; }

        public RandomGenerator(int width, int height)
            : base(width, height)
        {
            Rooms = new List<Rectangle>();
        }

        public override void Generate()
        {
            for (int i = 0; i < Map.Width; i++)
            {
                for (int j = 0; j < Map.Height; j++)
                {
                    if (i == 0 || j == 0 || i == (Map.Width - 1) || j == (Map.Height - 1))
                    {
                        Map.SetTile(i, j, Wall);
                    }
                    else
                    {
                        Map.SetTile(i, j, i % 2 == j % 2 ? FloorA : FloorB);
                    }
                }
            }

            Rooms.Add(new Rectangle(0, 0, Map.Width, Map.Height));

            for (int i = 0; i < 400; i++)
            {
                int x = Random.Next(1, Map.Width - 1);
                int y = Random.Next(1, Map.Height - 1);
                if (x != Map.Width / 2 && y != Map.Height / 2)
                {
                    Map.SetTile(x, y, Wall);
                }
            }

        }

        public override Point GetPlayerStartingPosition()
        {
            return Rooms.First().Center;
        }

        public override void SpawnExitForMap(GameWorld world)
        {
            
        }

        public override void SpawnEntitiesForMap(GameWorld world)
        {
            
        }
    }
}

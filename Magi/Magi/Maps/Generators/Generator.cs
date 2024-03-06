using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Generators
{
    public abstract class Generator
    {
        public static Tile Wall = new Tile()
        {
            BaseTileType = Constants.TileTypes.Wall,
            BackgroundColor = new Color(0f, 0f, .5f)
        };
        public static Tile FloorA = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.LightGray
        };
        public static Tile FloorB = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.WhiteSmoke
        };

        public Map Map { get; protected set; }
        protected Random Random;
        public Generator(int width, int height)
        {
            Map = new Map(width, height);
            Random = new Random();
        }

        public abstract void Generate();

        public abstract Point GetPlayerStartingPosition();
        public abstract void SpawnEntitiesForMap(GameWorld world, RandomTable<string> enemySpawnTable, RandomTable<string> itemSpawnTable);
        public abstract void SpawnExitForMap(GameWorld world);
        protected void SpawnExit(GameWorld world, Point position)
        {
            world.PhysicsWorld.AddEntity(world.World.Create(
                new Exit(),
                new Name() { EntityName = "Exit" },
                new Position() { Point = position },
                new Renderable() { Color = Color.Black, Glyph = (char)31, ShowOutsidePlayerFov = true }
            ).Reference(), position);
        }
    }
}

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
            BackgroundColor = new Color(.5f, .5f, .5f),
        };
        public static Tile Floor = new Tile()
        {
            BaseTileType = Constants.TileTypes.Floor,
            BackgroundColor = Color.LightGray
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
        public abstract Point GetExitPosition();
        public abstract void SpawnEntitiesForMap(GameWorld world, RandomTable<string> enemySpawnTable, RandomTable<string> itemSpawnTable);
        public abstract void SpawnExitForMap(GameWorld world);
        protected void SpawnExit(GameWorld world, Point position)
        {
            var endOfTomb = world.Tomb.CurrentLevel == world.Tomb.Levels.Count();
            var exitGlyph = endOfTomb ? (char)21 : (char)31;

            world.PhysicsWorld.AddEntity(world.World.Create(
                new Exit(),
                new Name() { EntityName = string.Concat("Exit ", endOfTomb ? "to next tomb" : "to next level") },
                new Position() { Point = position },
                new Renderable() { Color = Color.Black, Glyph = exitGlyph, ShowOutsidePlayerFov = true }
            ).Reference(), position);
        }
    }
}

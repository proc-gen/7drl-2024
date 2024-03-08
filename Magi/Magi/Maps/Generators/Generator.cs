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
        public void SpawnBossRoomForMap(GameWorld world)
        {
            var exit = GetExitPosition();
            var bossRoomLocation = GetBossRoomLocation(exit);

            var bossRoom = new Rectangle(bossRoomLocation.X, bossRoomLocation.Y, 10, 10);
            ApplyBossRoomToMap(bossRoom);

            SpawnExit(world, bossRoom.Center);
        }

        private Point GetBossRoomLocation(Point exit)
        {
            var bossRoomLocation = exit - new Point(5, 5);

            if (bossRoomLocation.X < 0)
            {
                bossRoomLocation += new Point(-bossRoomLocation.X, 0);
            }
            else if (bossRoomLocation.X > Map.Width - 11)
            {
                bossRoomLocation -= new Point(bossRoomLocation.X - (Map.Width - 11), 0);
            }

            if (bossRoomLocation.Y < 0)
            {
                bossRoomLocation += new Point(0, -bossRoomLocation.Y);
            }
            else if (bossRoomLocation.Y > Map.Height - 11)
            {
                bossRoomLocation -= new Point(0, bossRoomLocation.Y - (Map.Height - 11));
            }

            return bossRoomLocation;
        }

        private void ApplyBossRoomToMap(Rectangle bossRoom)
        {
            for (int i = bossRoom.X; i <= bossRoom.MaxExtentX; i++)
            {
                for (int j = bossRoom.Y; j <= bossRoom.MaxExtentY; j++)
                {
                    if (i == bossRoom.X || j == bossRoom.Y || i == bossRoom.MaxExtentX || j == bossRoom.MaxExtentY)
                    {
                        if ((i == bossRoom.X + 5 && j > 0 && j < Map.Height) || (j == bossRoom.Y + 5 && i > 0 && i < Map.Width))
                        {
                            Map.SetTile(i, j, Floor);
                        }
                        else
                        {
                            Map.SetTile(i, j, Wall);
                        }
                    }
                    else
                    {
                        Map.SetTile(i, j, Floor);
                    }
                }
            }
        }
        protected void SpawnExit(GameWorld world, Point position)
        {
            var endOfTomb = world.Tomb.CurrentLevel == world.Tomb.Levels.Keys.Max();
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

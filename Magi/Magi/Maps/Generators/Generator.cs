using Arch.Core.Extensions;
using Magi.Containers.DatasetContainers;
using Magi.ECS.Components;
using Magi.Maps.Spawners;
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
            SpawnBoss(world, bossRoom.Center);
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

        private void SpawnBoss(GameWorld world, Point position)
        {
            var playerStats = world.PlayerReference.Entity.Get<CombatStats>();

            var health = CombatStatHelper.CalculateMaxHealth(playerStats.Level, 10 + 5 * playerStats.Level);
            var mana = CombatStatHelper.CalculateMaxMana(playerStats.Level, 10 + 5 * playerStats.Level);

            var weaponTable = new RandomTable<string>();
            var armorTable = new RandomTable<string>();
            
            foreach(var weapon in ItemSpawner.GetWeaponsForLevel(playerStats.Level))
            {
                weaponTable = weaponTable.Add(weapon, 1);
            }

            foreach (var armor in ItemSpawner.GetArmorsForLevel(playerStats.Level))
            {
                armorTable = armorTable.Add(armor, 1);
            }

            EnemyContainer enemyContainer = new EnemyContainer()
            {
                Name = world.Tomb.Mage,
                Health = health,
                Mana = mana,
                Strength = 10 + 5 * playerStats.Level,
                Intelligence = 10 + 5 * playerStats.Level,
                Vitality = 10 + 5 * playerStats.Level,
                Dexterity = 10 + 5 * playerStats.Level,
                ViewDistance = 10,
                Experience = 20 * playerStats.Level,
                Glyph = (char)Random.Next(224, 233),
                GlyphColorRed = 192,
                GlyphColorBlue = 50,
                GlyphColorGreen = 0,
                Mainhand = weaponTable.Roll(Random),
                Armor = armorTable.Roll(Random),
            };

            EnemySpawner.SpawnEntityForPoint(world, position, enemyContainer);
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

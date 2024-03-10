using Arch.Core.Extensions;
using CommunityToolkit.HighPerformance;
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
            BackgroundColor = Color.DarkGray,
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
                        Map.SetTile(i, j, Wall);
                    }
                    else
                    {
                        Map.SetTile(i, j, Floor);
                    }
                }
            }

            Point top = new Point(bossRoom.Center.X, bossRoom.Y),
                bottom = new Point(bossRoom.Center.X, bossRoom.MaxExtentY),
                left = new Point(bossRoom.X, bossRoom.Center.Y),
                right = new Point(bossRoom.MaxExtentX, bossRoom.Center.Y);

            bool madeExit = false;

            if(bottom.Y + 1 < Map.Height - 1
                && Map.GetTile(bottom.X, bottom.Y + 1).BaseTileType == Constants.TileTypes.Floor)
            {
                Map.SetTile(bottom, Floor);
                madeExit = true;
            }
            else if (top.Y - 1 > 0
                && Map.GetTile(top.X, top.Y - 1).BaseTileType == Constants.TileTypes.Floor)
            {
                Map.SetTile(top, Floor);
                madeExit = true;
            }
            else if (right.X + 1 < Map.Width - 1
                && Map.GetTile(right.X + 1, right.Y).BaseTileType == Constants.TileTypes.Floor)
            {
                Map.SetTile(right, Floor);
                madeExit = true;
            }
            else if (left.X - 1 > 0
                && Map.GetTile(left.X - 1, left.Y).BaseTileType == Constants.TileTypes.Floor)
            {
                Map.SetTile(left, Floor);
                madeExit = true;
            }

            if (!madeExit)
            {
                Point corridorEnd = new Point(Map.Width / 2, Map.Height / 2);
                while(Map.GetTile(corridorEnd).BaseTileType != Constants.TileTypes.Floor)
                {
                    corridorEnd += new Point(-1, 0);
                }

                GeneratorExtensions.ApplyCorridor(this, bossRoom.Center.X, bossRoom.Center.Y, corridorEnd.X, corridorEnd.Y);
            }
        }

        private void SpawnBoss(GameWorld world, Point position)
        {
            var playerStats = world.PlayerReference.Entity.Get<CombatStats>();

            var health = CombatStatHelper.CalculateMaxHealth(playerStats.Level, 10 + playerStats.Level);
            var mana = CombatStatHelper.CalculateMaxMana(playerStats.Level, 10 + playerStats.Level);

            var weaponTable = new RandomTable<string>();
            var armorTable = new RandomTable<string>();
            
            foreach(var weapon in ItemSpawner.GetWeaponsForLevel(playerStats.Level))
            {
                weaponTable = weaponTable.Add(weapon, 1);
            }
            var chosenWeapon = weaponTable.Roll(Random);

            foreach (var armor in ItemSpawner.GetArmorsForLevel(playerStats.Level))
            {
                armorTable = armorTable.Add(armor, 1);
            }
            var chosenArmor = armorTable.Roll(Random);

            int skillChance = Math.Min(50, playerStats.Level * 2);
            int meleeChance = chosenWeapon.Contains("Bow") ? 10 : 100 - skillChance;
            int rangedChance = 100 - meleeChance - skillChance;


            EnemyContainer enemyContainer = new EnemyContainer()
            {
                Name = world.Tomb.Mage,
                Health = health,
                Mana = mana,
                Strength = 10 + playerStats.Level,
                Intelligence = 10 + playerStats.Level,
                Vitality = 10 + playerStats.Level,
                Dexterity = 10 + playerStats.Level,
                ViewDistance = 10,
                Experience = 20 * playerStats.Level,
                Glyph = (char)Random.Next(224, 233),
                GlyphColorRed = 192,
                GlyphColorBlue = 50,
                GlyphColorGreen = 0,
                Mainhand = chosenWeapon,
                Armor = chosenArmor,
                Boss = true,
                MeleeChance = meleeChance,
                RangedChance = rangedChance,
                SkillChance = skillChance,
            };

            var allowedSkills = SkillSpawner.GetSkillsForElement(world.Tomb.Element).AsSpan();
            Random.Shuffle(allowedSkills);

            int numSkills = Math.Min(4, Math.Max(1, playerStats.Level / 5));
            if(numSkills > 0)
            {
                enemyContainer.Skill1 = allowedSkills[0];
            }
            if (numSkills > 1)
            {
                enemyContainer.Skill2 = allowedSkills[1];
            }
            if (numSkills > 2)
            {
                enemyContainer.Skill3 = allowedSkills[2];
            }
            if (numSkills > 3)
            {
                enemyContainer.Skill4 = allowedSkills[3];
            }

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
                new Renderable() { Color = Color.DarkGray, Glyph = exitGlyph, ShowOutsidePlayerFov = true }
            ).Reference(), position);
        }
    }
}

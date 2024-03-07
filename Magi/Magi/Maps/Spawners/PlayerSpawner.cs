using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;

namespace Magi.Maps.Spawners
{
    public class PlayerSpawner
    {
        public PlayerSpawner() { }
        public void SpawnPlayer(GameWorld world, Point startingPosition) 
        {
            int health = CombatStatHelper.CalculateMaxHealth(1, 10);
            int mana = CombatStatHelper.CalculateMaxMana(1, 10);

            var stats = new CombatStats()
            {
                MaxHealth = health,
                CurrentHealth = health,
                MaxMana = mana,
                CurrentMana = mana,
                BaseStrength = 10,
                CurrentStrength = 10,
                BaseIntelligence = 10,
                CurrentIntelligence = 10,
                BaseVitality = 10,
                CurrentVitality = 10,
                BaseDexterity = 10,
                CurrentDexterity = 10,
                Level = 0,
                Experience = 0,
                ExperienceForNextLevel = 0,
            };

            CombatStatHelper.ProcessLevelUp(ref stats);

            world.PlayerReference = world.World.Create(
                new Player(),
                new Position() { Point = startingPosition },
                new Renderable() { Color = Color.LimeGreen, Glyph = '@' },
                new Input() { CanAct = true },
                new Blocker(),
                new Name() { EntityName = "Player" },
                new ViewDistance() { Distance = 10 },
                stats,
                new CombatEquipment()
            ).Reference();

            world.PhysicsWorld.AddEntity(world.PlayerReference, startingPosition);
        }
    }
}

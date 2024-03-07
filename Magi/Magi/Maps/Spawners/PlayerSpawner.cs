using Arch.Core.Extensions;
using Magi.Containers.DatasetContainers;
using Magi.ECS.Components;
using Magi.Utils;

namespace Magi.Maps.Spawners
{
    public class PlayerSpawner
    {
        public static Dictionary<string, PlayerContainer> PlayerContainers;

        static PlayerSpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "players.json");
            PlayerContainers = JsonFileManager.LoadDataset<PlayerContainer>(path);
        }

        public PlayerSpawner() { }
        public void SpawnPlayer(GameWorld world, Point startingPosition) 
        {
            var playerContainer = PlayerContainers[world.PlayerClass];

            int health = CombatStatHelper.CalculateMaxHealth(1, playerContainer.Vitality);
            int mana = CombatStatHelper.CalculateMaxMana(1, playerContainer.Intelligence);

            var stats = new CombatStats()
            {
                MaxHealth = health,
                CurrentHealth = health,
                MaxMana = mana,
                CurrentMana = mana,
                BaseStrength = playerContainer.Strength,
                CurrentStrength = playerContainer.Strength,
                BaseIntelligence = playerContainer.Intelligence,
                CurrentIntelligence = playerContainer.Intelligence,
                BaseVitality = playerContainer.Vitality,
                CurrentVitality = playerContainer.Vitality,
                BaseDexterity = playerContainer.Dexterity,
                CurrentDexterity = playerContainer.Dexterity,
                Level = 0,
                Experience = 0,
                ExperienceForNextLevel = 0,
            };

            CombatStatHelper.ProcessLevelUp(ref stats);
            var combatEquipment = new CombatEquipment();
            world.PlayerReference = world.World.Create(
                new Player(),
                new Position() { Point = startingPosition },
                new Renderable() { Color = Color.LimeGreen, Glyph = '@' },
                new Input() { CanAct = true },
                new Blocker(),
                new Name() { EntityName = "Player" },
                new ViewDistance() { Distance = 10 },
                stats,
                combatEquipment
            ).Reference();

            if (!string.IsNullOrEmpty(playerContainer.Mainhand))
            {
                combatEquipment.MainHandReference = ItemSpawner.SpawnEntityForOwner(world, playerContainer.Mainhand, world.PlayerReference);
                combatEquipment.MainHandReference.Entity.Add(new Equipped());
            }
            if (!string.IsNullOrEmpty(playerContainer.Offhand))
            {
                combatEquipment.OffHandReference = ItemSpawner.SpawnEntityForOwner(world, playerContainer.Offhand, world.PlayerReference);
                combatEquipment.OffHandReference.Entity.Add(new Equipped());
            }
            if (!string.IsNullOrEmpty(playerContainer.Armor))
            {
                combatEquipment.ArmorReference = ItemSpawner.SpawnEntityForOwner(world, playerContainer.Armor, world.PlayerReference);
                combatEquipment.ArmorReference.Entity.Add(new Equipped());
            }

            world.PlayerReference.Entity.Set(combatEquipment);

            world.PhysicsWorld.AddEntity(world.PlayerReference, startingPosition);
        }
    }
}

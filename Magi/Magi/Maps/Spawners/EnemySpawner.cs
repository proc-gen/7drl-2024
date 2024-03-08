﻿using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers.DatasetContainers;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Spawners
{
    public class EnemySpawner : Spawner
    {
        public static Dictionary<string, EnemyContainer> EnemyContainers;

        static EnemySpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "enemies.json");
            EnemyContainers = JsonFileManager.LoadDataset<EnemyContainer>(path);
        }

        public EnemySpawner(RandomTable<string> spawnTable, Random random) 
            : base(spawnTable, random)
        {
        }

        public override void SpawnAnEntityForPoint(GameWorld world, Point point)
        {
            string key = SpawnTable.Roll(Random);
            var enemyContainer = EnemyContainers[key];

            SpawnEntityForPoint(world, point, enemyContainer);
        }

        public static void SpawnEntityForPoint(GameWorld world, Point point, EnemyContainer enemyContainer)
        {
            var combatEquipment = new CombatEquipment();

            var reference = world.World.Create(
                new Position() { Point = point },
                new Name() { EntityName = enemyContainer.Name },
                new ViewDistance() { Distance = enemyContainer.ViewDistance },
                new Renderable() { Glyph = enemyContainer.Glyph, Color = new Color(enemyContainer.GlyphColorRed, enemyContainer.GlyphColorGreen, enemyContainer.GlyphColorBlue) },
                new Input() { CanAct = true },
                new Blocker(),
                new CombatStats()
                {
                    MaxHealth = enemyContainer.Health,
                    CurrentHealth = enemyContainer.Health,
                    MaxMana = enemyContainer.Mana,
                    CurrentMana = enemyContainer.Mana,
                    BaseStrength = enemyContainer.Strength,
                    CurrentStrength = enemyContainer.Strength,
                    BaseIntelligence = enemyContainer.Intelligence,
                    CurrentIntelligence = enemyContainer.Intelligence,
                    BaseVitality = enemyContainer.Vitality,
                    CurrentVitality = enemyContainer.Vitality,
                    BaseDexterity = enemyContainer.Dexterity,
                    CurrentDexterity = enemyContainer.Dexterity,
                    Experience = enemyContainer.Experience,
                },
                combatEquipment
            ).Reference();

            if (!string.IsNullOrEmpty(enemyContainer.Mainhand))
            {
                combatEquipment.MainHandReference = ItemSpawner.SpawnEntityForOwner(world, enemyContainer.Mainhand, reference);
                combatEquipment.MainHandReference.Entity.Add(new Equipped());
            }
            if (!string.IsNullOrEmpty(enemyContainer.Offhand))
            {
                combatEquipment.OffHandReference = ItemSpawner.SpawnEntityForOwner(world, enemyContainer.Offhand, reference);
                combatEquipment.OffHandReference.Entity.Add(new Equipped());
            }
            if (!string.IsNullOrEmpty(enemyContainer.Armor))
            {
                combatEquipment.ArmorReference = ItemSpawner.SpawnEntityForOwner(world, enemyContainer.Armor, reference);
                combatEquipment.ArmorReference.Entity.Add(new Equipped());
            }

            reference.Entity.Set(combatEquipment);

            world.PhysicsWorld.AddEntity(reference, point);
        }
    }
}

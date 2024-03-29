﻿using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Maps;
using Magi.Maps.Decorators;
using Magi.Maps.Generators;
using Magi.Maps.Spawners;
using Magi.Serialization;
using Magi.Tombs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Magi.Utils
{
    public class GameWorld
    {
        [JsonIgnore]
        public World World { get; set; }
        [JsonIgnore]
        public PhysicsWorld PhysicsWorld { get; set; }

        [JsonIgnore]
        public EntityReference PlayerReference { get; set; }
        public SerializableWorld SerializableWorld { get; set; }
        public GameState CurrentState { get; set; }
        public Tomb Tomb { get; set; }
        public Map Map { get; set; }
        public HashSet<Point> PlayerFov { get; set; }
        [JsonProperty]
        protected List<LogItem> LogItems { get; set; }
        public string PlayerClass { get; set; }
        public Dictionary<string, int> ConfirmedKills { get; set; }
        public int TombsCleared { get; set; }
         
        public GameWorld() 
        {
            World = World.Create();
            PhysicsWorld = new PhysicsWorld();
            PlayerReference = EntityReference.Null;
            PlayerFov = new HashSet<Point>();
            LogItems = new List<LogItem>();
            ConfirmedKills = new Dictionary<string, int>();
            PlayerClass = "Warrior";
            TombsCleared = 0;
        }

        public void AddLogEntry(string entry)
        {
            List<string> splitEntries = new List<string>();
            int character = 0;
            int lineLength = 45;
            do
            {
                string nextEntry = entry.Substring(character);
                if(nextEntry.Length < lineLength)
                {
                    splitEntries.Add(nextEntry);
                    character = entry.Length;
                }
                else
                {
                    nextEntry = entry.Substring(character, lineLength);
                    int lastSpace = nextEntry.LastIndexOf(' ');
                    splitEntries.Add(nextEntry.Substring(0, lastSpace));
                    character = lastSpace + 1;
                }
            } while (character < entry.Length);

            foreach(var newEntry in splitEntries)
            {
                LogItems.Add(new LogItem(newEntry));
            }
        }

        public List<LogItem> GetLogItems() 
        {
            return LogItems;
        }

        public void StartPlayerTurn(Point direction)
        {
            var input = PlayerReference.Entity.Get<Input>();
            input.Direction = direction;
            input.SkipTurn = direction == Point.None;
            input.Processed = false;
            PlayerReference.Entity.Set(input);
            CurrentState = GameState.PlayerTurn;
        }

        public void GoNextLevel()
        {
            if (PlayerReference != EntityReference.Null)
            {
                RemoveAllNonPlayerOwnedEntities();
            }

            int playerLevel = PlayerReference == EntityReference.Null ? 1 : PlayerReference.Entity.Get<CombatStats>().Level;

            SetNextLevel(playerLevel);

            var generator = Tomb.IncrementLevel(playerLevel);
            Map = generator.Map;

            SetStartingPosition(generator);
            SpawnEntitiesForMap(generator, playerLevel);
            FloorDecorator.Decorate(generator, Tomb.Element);

            CurrentState = GameState.AwaitingPlayerInput;
        }

        private void SetNextLevel(int playerLevel)
        {
            if (Tomb == null || Tomb.CurrentLevel == Tomb.Levels.Keys.Max())
            {
                if(Tomb != null && Tomb.CurrentLevel == Tomb.Levels.Keys.Max())
                {
                    TombsCleared++;
                }

                GenerateTomb(playerLevel);
                AddLogEntry(string.Concat("You have ventured into the tomb of ", Tomb.Mage, " the ", Tomb.Element.ToString(), " mage"));
            }
            else
            {
                AddLogEntry("You have descended to the next level");
            }
        }

        private void GenerateTomb(int playerLevel)
        {
            var ElementsTable = new RandomTable<Elements>()
                //.Add(Elements.Air, 1)
                .Add(Elements.Fire, 1)
                //.Add(Elements.Water, 1)
                //.Add(Elements.Earth, 1)
                .Add(Elements.Lightning, 1)
                .Add(Elements.Ice, 1);

            var Random = new Random();

            Tomb = new Tomb(playerLevel, ElementsTable.Roll(Random));
        }

        private void SetStartingPosition(Generator generator)
        {
            var startPosition = generator.GetPlayerStartingPosition();
            
            if (PlayerReference == EntityReference.Null)
            {
                new PlayerSpawner().SpawnPlayer(this, startPosition);
            }
            else
            {
                var position = PlayerReference.Entity.Get<Position>();
                position.Point = generator.GetPlayerStartingPosition();
                PlayerReference.Entity.Set(position);
                PhysicsWorld.AddEntity(PlayerReference, position.Point);
            }

            FieldOfView.CalculatePlayerFOV(this);
        }

        private void SpawnEntitiesForMap(Generator generator, int playerLevel)
        {
            var enemyTable = new RandomTable<string>();
            foreach (var enemy in EnemySpawner.EnemyContainers)
            {
                if (enemy.Value.LevelRequirement <= playerLevel
                    && (enemy.Value.Element == Tomb.Element || enemy.Value.Element == Elements.None))
                {
                    enemyTable = enemyTable.Add(enemy.Key, 1);
                }
            }
            var itemTable = new RandomTable<string>();
            foreach (var item in ItemSpawner.GetItemsForLevel(playerLevel))
            {
                itemTable = itemTable.Add(item, item.Contains("Potion") ? 3 : 1);
            }

            if (Tomb.CurrentLevel == Tomb.Levels.Keys.Max())
            {
                generator.SpawnBossRoomForMap(this);
            }
            else
            {
                generator.SpawnExitForMap(this);
            }

            generator.SpawnEntitiesForMap(this, enemyTable, itemTable);
        }

        private void RemoveAllNonPlayerOwnedEntities()
        {
            PhysicsWorld.Clear();
            List<Entity> entities = new List<Entity>();
            World.GetEntities(new QueryDescription(), entities);

            foreach (var entity in entities)
            {
                if (entity.Has<Owner>())
                {
                    if (entity.Get<Owner>().OwnerReference != PlayerReference)
                    {
                        entity.Add(new Remove());
                    }
                }
                else if (entity.Reference() != PlayerReference)
                {
                    entity.Add(new Remove());
                }
            }

            World.Destroy(new QueryDescription().WithAll<Remove>());
        }
    }
}

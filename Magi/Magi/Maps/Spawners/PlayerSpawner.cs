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
            world.PlayerReference = world.World.Create(
                new Player(),
                new Position() { Point = startingPosition },
                new Renderable() { Color = Color.DarkGreen, Glyph = '@' },
                new Input() { CanAct = true },
                new Blocker(),
                new Name() { EntityName = "Player" },
                new ViewDistance() { Distance = 10 },
                new CombatStats() 
                { 
                    MaxHealth = 10, 
                    CurrentHealth = 10,
                    MaxMana = 30,
                    CurrentMana = 30,
                    BaseStrength = 10,
                    CurrentStrength = 10,
                    BaseIntelligence = 10,
                    CurrentIntelligence = 10,
                    BaseVitality = 10,
                    CurrentVitality = 10,
                    BaseDexterity = 10,
                    CurrentDexterity = 10,
                }
            ).Reference();

            world.PhysicsWorld.AddEntity(world.PlayerReference, startingPosition);
        }
    }
}

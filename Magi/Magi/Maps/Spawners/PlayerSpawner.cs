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
                new Input() { CanAct = true }
            ).Reference();

            world.PhysicsWorld.AddEntity(world.PlayerReference, startingPosition);
        }
    }
}

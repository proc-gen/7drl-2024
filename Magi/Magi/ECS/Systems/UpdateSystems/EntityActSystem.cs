using Arch.Core.Extensions;
using Arch.Core;
using Magi.Constants;
using Magi.ECS.Components;
using Magi.Maps;
using Magi.Utils;
using SadConsole.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class EntityActSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription nonPlayerQuery = new QueryDescription().WithAll<Position, Input>().WithNone<Player>();

        public EntityActSystem(GameWorld world)
            : base(world)
        {
        }

        public void Update(TimeSpan delta)
        {
            switch (World.CurrentState)
            {
                case GameState.PlayerTurn:
                    HandlePlayerTurn();
                    break;
                case GameState.MonsterTurn:
                    HandleMonsterTurn();
                    break;
            }
        }

        private void HandlePlayerTurn()
        {
            var playerPosition = World.PlayerReference.Entity.Get<Position>();
            var playerInput = World.PlayerReference.Entity.Get<Input>();

            TryAct(World.PlayerReference, ref playerPosition, ref playerInput);

            World.PlayerReference.Entity.Set(playerPosition, playerInput);
            FieldOfView.CalculatePlayerFOV(World);
        }

        private void HandleMonsterTurn()
        {
            World.World.Query(in nonPlayerQuery, (Entity entity, ref Position position, ref Input input) =>
            {
                TryAct(entity.Reference(), ref position, ref input);
            });
        }

        private ActionTypes TryAct(EntityReference entity, ref Position position, ref Input input)
        {
            ActionTypes action = ActionTypes.Wait;

            if (!input.SkipTurn && input.CanAct)
            {
                var nextTile = World.Map.GetTile(position.Point + input.Direction);
                if (nextTile.BaseTileType != TileTypes.Wall)
                {
                    var entitiesAtPosition = World.PhysicsWorld.GetEntitiesAtLocation(position.Point + input.Direction);
                    if (entitiesAtPosition == null || !entitiesAtPosition.Any(a => a.Entity.Has<Blocker>()))
                    {
                        World.PhysicsWorld.MoveEntity(entity, position.Point, position.Point + input.Direction);
                        position.Point += input.Direction;
                        action = ActionTypes.Move;
                    }
                    else
                    {
                        World.World.Create(new MeleeAttack() { Source = entity, Target = entitiesAtPosition.Where(a => a.Entity.Has<Blocker>()).First() });
                        action = ActionTypes.MeleeAttack;
                    }
                }
            }

            input.Processed = true;
            return action;
        }
    }
}

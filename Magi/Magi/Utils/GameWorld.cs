using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Maps;
using Magi.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public Map Map { get; set; }
        public HashSet<Point> PlayerFov { get; set; }
        public List<LogItem> LogItems { get; set; }
         
        public GameWorld() 
        {
            World = World.Create();
            PhysicsWorld = new PhysicsWorld();
            PlayerReference = EntityReference.Null;
            PlayerFov = new HashSet<Point>();
            LogItems = new List<LogItem>();
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

        public void RemoveAllNonPlayerOwnedEntities()
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

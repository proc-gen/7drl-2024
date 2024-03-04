using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.ECS.Components;
using Magi.Maps;
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
        public GameState CurrentState { get; set; }
        [JsonIgnore]
        public World World { get; set; }
        [JsonIgnore]
        public PhysicsWorld PhysicsWorld { get; set; }
        public Map Map { get; set; }
        [JsonIgnore]
        public EntityReference PlayerReference { get; set; }
        public HashSet<Point> PlayerFov { get; set; }
        public GameWorld() 
        {
            World = World.Create();
            PhysicsWorld = new PhysicsWorld();
            PlayerReference = EntityReference.Null;
            PlayerFov = new HashSet<Point>();
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
    }
}

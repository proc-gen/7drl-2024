using Arch.Core;
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
    }
}

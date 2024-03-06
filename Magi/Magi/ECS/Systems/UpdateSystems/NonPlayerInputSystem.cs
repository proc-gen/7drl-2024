using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Pathfinding;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class NonPlayerInputSystem : ArchSystem, IUpdateSystem
    {
        SquareGridInGame SquareGrid { get; set; }
        AStarSearch<Location> AStarSearch { get; set; }
        QueryDescription nonPlayerQuery = new QueryDescription().WithAll<Position, Input>().WithNone<Player>();
        public NonPlayerInputSystem(GameWorld world)
            : base(world)
        {
            SquareGrid = new SquareGridInGame(world);
            AStarSearch = new AStarSearch<Location>(SquareGrid);
        }

        public void Update(TimeSpan delta)
        {
            if (World.CurrentState == Constants.GameState.MonsterTurn)
            {
                var playerPosition = World.PlayerReference.Entity.Get<Position>();
                World.World.Query(in nonPlayerQuery, (Entity entity, ref Position position, ref Input input, ref ViewDistance viewDistance) =>
                {
                    var fov = FieldOfView.CalculateFOV(World, entity.Reference());

                    if (fov.Contains(playerPosition.Point))
                    {
                        var path = AStarSearch.RunSearch(new Location(position.Point), new Location(playerPosition.Point));
                        input.Direction = path - position.Point;
                        input.SkipTurn = path == position.Point;
                    }
                    else
                    {
                        input.SkipTurn = true;
                        input.Direction = Point.None;
                    }
                    input.Processed = false;
                });
            }
        }
    }
}

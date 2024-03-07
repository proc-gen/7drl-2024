using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Processors;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class UseItemSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription itemsToUseQuery = new QueryDescription().WithAll<WantToUseItem>();
        QueryDescription itemsToDestroyQuery = new QueryDescription().WithAll<DestroyItem>();
        public UseItemSystem(GameWorld world)
            : base(world)
        {

        }

        public void Update(TimeSpan delta)
        {
            World.World.Query(in itemsToUseQuery, (Entity entity, ref Owner owner) =>
            {
                var reference = entity.Reference();

                if (entity.Has<Consumable>())
                {
                    if (ConsumableProcessor.Consume(World, reference))
                    {
                        reference.Entity.Add(new DestroyItem());
                    }                    
                }
                else if (entity.Has<Weapon>())
                {
                    WeaponProcessor.Equip(World, reference);
                }
                else if(entity.Has<Armor>())
                {
                    ArmorProcessor.Equip(World, reference);
                }

                reference.Entity.Remove<WantToUseItem>();
            });

            World.World.Destroy(in itemsToDestroyQuery);
        }
    }
}

using Arch.Core;
using Arch.Core.Extensions;
using Magi.Containers;
using Magi.ECS.Components;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Processors
{
    public static class ConsumableProcessor
    {
        public static bool Consume(GameWorld world, EntityReference entityToProcess)
        {
            bool consumed = false;

            var ownerReference = entityToProcess.Entity.Get<Owner>().OwnerReference;
            var ownerName = ownerReference.Entity.Get<Name>();
            var ownerStats = ownerReference.Entity.Get<CombatStats>();
            var itemName = entityToProcess.Entity.Get<Name>();
            var consumable = entityToProcess.Entity.Get<Consumable>();

            switch (consumable.ConsumableType)
            {
                case Constants.ConsumableTypes.Health:
                    if (ownerStats.CurrentHealth < ownerStats.MaxHealth)
                    {
                        int healAmount = Math.Min(consumable.ConsumableAmount, ownerStats.MaxHealth - ownerStats.CurrentHealth);
                        ownerStats.CurrentHealth += healAmount;
                        consumed = true;

                        world.AddLogEntry(string.Concat(ownerName.EntityName, " drank a ", itemName.EntityName, " and healed for ", healAmount, "hp"));
                    }
                    else
                    {
                        world.AddLogEntry(string.Concat(ownerName.EntityName, " is already at full health!"));
                    }
                    break;
                case Constants.ConsumableTypes.Mana:
                    if (ownerStats.CurrentMana < ownerStats.MaxMana)
                    {
                        int refillAmount = Math.Min(consumable.ConsumableAmount, ownerStats.MaxMana - ownerStats.CurrentMana);
                        ownerStats.CurrentMana += refillAmount;
                        consumed = true;

                        world.AddLogEntry(string.Concat(ownerName.EntityName, " drank a ", itemName.EntityName, " and replenished ", refillAmount, "mp"));
                    }
                    else
                    {
                        world.AddLogEntry(string.Concat(ownerName.EntityName, " is already at full mana!"));
                    }
                    break;
            }

            if (consumed)
            {
                ownerReference.Entity.Set(ownerStats);
            }

            return consumed;
        }
    }
}

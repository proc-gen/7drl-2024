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

namespace Magi.Items.Processors
{
    public static class ArmorProcessor
    {
        public static void Equip(GameWorld world, EntityReference armorReference)
        {
            var ownerReference = armorReference.Entity.Get<Owner>().OwnerReference;
            var ownerName = ownerReference.Entity.Get<Name>();
            var ownerEquipment = ownerReference.Entity.Get<CombatEquipment>();
            var itemName = armorReference.Entity.Get<Name>();
            var armorInfo = armorReference.Entity.Get<Armor>();

            if(armorInfo.ArmorType == Constants.ArmorType.Shield)
            {
                EquipShield(world, armorReference, ownerReference, ownerName, ownerEquipment, itemName, armorInfo);
            }
            else if(armorInfo.ArmorType == Constants.ArmorType.Wearable)
            {
                EquipWearable(world, armorReference, ownerReference, ownerName, ownerEquipment, itemName, armorInfo);
            }

            world.AddLogEntry(string.Concat(ownerName.EntityName, " equipped ", itemName.EntityName));
            armorReference.Entity.Add(new Equipped());
        }

        private static void EquipWearable(GameWorld world, EntityReference armorReference, EntityReference ownerReference, Name ownerName, CombatEquipment ownerEquipment, Name itemName, Armor armorInfo)
        {
            if(ownerEquipment.ArmorReference != EntityReference.Null) 
            {
                ownerEquipment.ArmorReference.Entity.Remove<Equipped>();
            }

            ownerEquipment.ArmorReference = armorReference;
            ownerReference.Entity.Set(ownerEquipment);
        }

        private static void EquipShield(GameWorld world, EntityReference armorReference, EntityReference ownerReference, Name ownerName, CombatEquipment ownerEquipment, Name itemName, Armor armorInfo)
        {
            if(ownerEquipment.MainHandReference != EntityReference.Null
                && WeaponProcessor.IsTwoHanded(ownerEquipment.MainHandReference.Entity.Get<Weapon>().WeaponType)) 
            {
                ownerEquipment.MainHandReference.Entity.Remove<Equipped>();
                ownerEquipment.MainHandReference = EntityReference.Null;
            }
            else if(ownerEquipment.OffHandReference != EntityReference.Null) 
            {
                ownerEquipment.OffHandReference.Entity.Remove<Equipped>();
            }

            ownerEquipment.OffHandReference = armorReference;
            ownerReference.Entity.Set(ownerEquipment);
        }

        public static int CalculateDamageReduction(Random random, DamageCalculation damageCalculation, EntityReference armorReference, EntityReference offhandReference, bool melee)
        {
            int reduction = 0;
            int blockChance = 0;

            if(armorReference != EntityReference.Null)
            {
                reduction += armorReference.Entity.Get<Armor>().ArmorBonus;
                blockChance += armorReference.Entity.Get<Armor>().BlockChance;
            }

            if(offhandReference != EntityReference.Null)
            {
                if (offhandReference.Entity.Has<Armor>())
                {
                    reduction += offhandReference.Entity.Get<Armor>().ArmorBonus;
                    blockChance += offhandReference.Entity.Get<Armor>().BlockChance;
                }
                else
                {
                    blockChance += 10;
                }
            }

            if (!damageCalculation.CriticalHit)
            {
                if (blockChance > 0)
                {
                    if (random.Next(100) < blockChance)
                    {
                        reduction = damageCalculation.Damage;
                    }
                }

                if (!melee && reduction != damageCalculation.Damage)
                {
                    reduction /= 2;
                }
            }


            return reduction;
        }
    }
}

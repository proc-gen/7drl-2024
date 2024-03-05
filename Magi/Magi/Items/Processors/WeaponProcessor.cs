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
    public static class WeaponProcessor
    {
        public static void Equip(GameWorld world, EntityReference weaponReference)
        {
            var ownerReference = weaponReference.Entity.Get<Owner>().OwnerReference;
            var ownerName = ownerReference.Entity.Get<Name>();
            var ownerEquipment = ownerReference.Entity.Get<CombatEquipment>();
            var itemName = weaponReference.Entity.Get<Name>();
            var weaponInfo = weaponReference.Entity.Get<Weapon>();

            if(ownerEquipment.MainHandReference == EntityReference.Null
                && ownerEquipment.OffHandReference == EntityReference.Null)
            {
                ownerEquipment.MainHandReference = weaponReference;
                ownerEquipment.OffHandReference = IsTwoHanded(weaponInfo.WeaponType) ? weaponReference : ownerEquipment.OffHandReference;
            }
            else if(ownerEquipment.MainHandReference == EntityReference.Null
                && !IsTwoHanded(weaponInfo.WeaponType))
            {
                ownerEquipment.MainHandReference = weaponReference;
            }
            else if(ownerEquipment.MainHandReference != EntityReference.Null)
            {
                ownerEquipment.MainHandReference.Entity.Remove<Equipped>();
                if (IsTwoHanded(weaponInfo.WeaponType) 
                    && ownerEquipment.MainHandReference != ownerEquipment.OffHandReference
                    && ownerEquipment.OffHandReference != EntityReference.Null)
                {
                    ownerEquipment.OffHandReference.Entity.Remove<Equipped>();
                }

                ownerEquipment.MainHandReference = weaponReference;
                ownerEquipment.OffHandReference = IsTwoHanded(weaponInfo.WeaponType) ? weaponReference : ownerEquipment.OffHandReference;
            }

            world.LogItems.Add(new LogItem(string.Concat(ownerName.EntityName, " equipped ", itemName.EntityName)));
            ownerReference.Entity.Set(ownerEquipment);
            weaponReference.Entity.Add(new Equipped());
        }

        public static bool IsTwoHanded(Constants.WeaponTypes weaponType)
        {
            return weaponType == Constants.WeaponTypes.TwoHandedMelee
                    || weaponType == Constants.WeaponTypes.TwoHandedRanged;
        }
    }
}

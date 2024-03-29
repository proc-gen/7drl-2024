﻿using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
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
    public static class WeaponProcessor
    {
        public static void Equip(GameWorld world, EntityReference weaponReference)
        {
            var ownerReference = weaponReference.Entity.Get<Owner>().OwnerReference;
            var ownerName = ownerReference.Entity.Get<Name>();
            var ownerStats = ownerReference.Entity.Get<CombatStats>();
            var ownerEquipment = ownerReference.Entity.Get<CombatEquipment>();
            var itemName = weaponReference.Entity.Get<Name>();
            var itemInfo = weaponReference.Entity.Get<Item>();
            var weaponInfo = weaponReference.Entity.Get<Weapon>();

            if (ItemProcessor.CanEquip(itemInfo, ownerStats))
            {
                if (ownerEquipment.MainHandReference == EntityReference.Null
                    && ownerEquipment.OffHandReference == EntityReference.Null)
                {
                    ownerEquipment.MainHandReference = weaponReference;
                    ownerEquipment.OffHandReference = IsTwoHanded(weaponInfo.WeaponType) ? weaponReference : ownerEquipment.OffHandReference;
                }
                else if (ownerEquipment.MainHandReference == EntityReference.Null
                    && !IsTwoHanded(weaponInfo.WeaponType))
                {
                    ownerEquipment.MainHandReference = weaponReference;
                }
                else if (ownerEquipment.MainHandReference != EntityReference.Null)
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

                world.AddLogEntry(string.Concat(ownerName.EntityName, " equipped ", itemName.EntityName));
                ownerReference.Entity.Set(ownerEquipment);
                weaponReference.Entity.Add(new Equipped());
            }
            else
            {
                world.AddLogEntry(string.Concat(ownerName.EntityName, " doesn't meet the requirements for ", itemName.EntityName));
            }
        }

        public static bool IsTwoHanded(WeaponTypes weaponType)
        {
            return weaponType == WeaponTypes.TwoHandedMelee
                    || weaponType == WeaponTypes.TwoHandedRanged;
        }

        public static DamageCalculation CalculateDamage(Random random, EntityReference weaponReference, int dexterity, bool melee, int bonusDamage)
        {
            int damage = bonusDamage;
            DamageTypes damageType = DamageTypes.Bludgeoning;
            bool criticalHit = false;
            int imbuementDamage = 0;
            Elements imbuementElement = Elements.None;

            if (weaponReference != EntityReference.Null)
            {
                var weapon = weaponReference.Entity.Get<Weapon>();
                if (melee && weapon.Range == 1 || !melee && weapon.Range > 1)
                {
                    damageType = weapon.DamageType;

                    var damageEntries = weapon.DamageRoll.Split('d');
                    int numDice = int.Parse(damageEntries[0]);
                    int numSides = int.Parse(damageEntries[1]);

                    for (int i = 0; i < numDice; i++)
                    {
                        damage += random.Next(0, numSides) + 1;
                    }

                    criticalHit = random.Next(100) < ((20 - weapon.CriticalHitRoll + 1) * 5 + dexterity / 5);
                    damage *= criticalHit ? weapon.CriticalHitMultiplier : 1;

                    if (weaponReference.Entity.Has<Imbuement>())
                    {
                        imbuementDamage = random.Next(3) + 1;
                        imbuementElement = weaponReference.Entity.Get<Imbuement>().Element;
                    }
                }
            }
            else
            {
                criticalHit = random.Next(100) < 5 + dexterity / 5;
                damage = (random.Next(0, 3) + 1) * (criticalHit ? 2 : 1);
            }

            return new DamageCalculation()
            {
                Damage = damage,
                DamageType = damageType,
                CriticalHit = criticalHit,
                ImbuementDamage = imbuementDamage,
                ImbuementElement = imbuementElement,
            };
        }
    }
}

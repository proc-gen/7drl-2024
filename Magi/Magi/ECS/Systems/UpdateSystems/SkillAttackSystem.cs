﻿using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.ECS.Components;
using Magi.Processors;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SadConsole.Settings;

namespace Magi.ECS.Systems.UpdateSystems
{
    public class SkillAttackSystem : ArchSystem, IUpdateSystem
    {
        QueryDescription skillAttacksQuery = new QueryDescription().WithAll<SkillAttack>();
        Random random = new Random();

        public SkillAttackSystem(GameWorld world) 
            : base(world) 
        { 
        }

        public void Update(TimeSpan delta) 
        {
            World.World.Query(in skillAttacksQuery, (Entity entity, ref SkillAttack skillAttack) =>
            {
                var sourceName = skillAttack.Source.Entity.Get<Name>();
                var sourceStats = skillAttack.Source.Entity.Get<CombatStats>();
                var skillName = skillAttack.SourceSkill.Entity.Get<Name>();
                var skillInfo = skillAttack.SourceSkill.Entity.Get<Skill>();
                var sourceEquipment = skillAttack.Source.Entity.Get<CombatEquipment>();

                sourceStats.CurrentMana -= skillInfo.ManaCost;
                
                Color effectColor = Color.White;
                switch (skillInfo.Element)
                {
                    case Elements.Fire:
                        effectColor = ElementColors.FireRed;
                        break;
                    case Elements.Ice:
                        effectColor = ElementColors.IceBlueGreen;
                        break;
                    case Elements.Lightning:
                        effectColor = ElementColors.LightningLightYellow;
                        break;
                }

                if (skillInfo.TargetingType == TargetingType.Imbuement)
                {
                    HandleImbuement(skillAttack, skillInfo, sourceName);

                    World.World.Create(
                                new RealTime(),
                                new Renderable() { Glyph = (char)43, Color = effectColor },
                                new TimedLife() { TimeLeft = 1f },
                                new Position() { Point = skillAttack.Source.Entity.Get<Position>().Point });
                }
                else if (skillInfo.TargetingType == TargetingType.ChainTargetDamage)
                {
                    HashSet<EntityReference> affectedEntities = new HashSet<EntityReference>() { skillAttack.Target };
                    int radius = 1;
                    while(radius < skillInfo.TargetRange && affectedEntities.Count < 3)
                    {
                        var aoePoints = FieldOfView.CalculateFOV(World, skillAttack.TargetLocation, radius + 1, false);
                        foreach (var aoePoint in aoePoints)
                        {
                            var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(aoePoint);
                            if (entitiesAtLocation != null)
                            {
                                foreach (var entityAtLocation in entitiesAtLocation)
                                {
                                    if (entityAtLocation != skillAttack.Source && entityAtLocation.Entity.Has<CombatStats>())
                                    {
                                        affectedEntities.Add(entityAtLocation);
                                    }
                                }
                            }
                        }
                        radius++;
                    }

                    HashSet<Point> effectPoints = FieldOfView.GetPointsInLine(skillAttack.Source.Entity.Get<Position>().Point, affectedEntities.First().Entity.Get<Position>().Point);
                    if(affectedEntities.Count > 1)
                    {
                        var affectedEntitiesList = affectedEntities.ToList();
                        for(int i = 0; i < affectedEntitiesList.Count - 1; i++)
                        {
                            var newPoints = FieldOfView.GetPointsInLine(affectedEntitiesList[i].Entity.Get<Position>().Point, affectedEntitiesList[i + 1].Entity.Get<Position>().Point);
                            foreach(var point in newPoints)
                            {
                                effectPoints.Add(point);
                            }
                        }
                    }

                    foreach(var affectedEntity in affectedEntities)
                    {
                        HandleTargetDamage(skillAttack.Source,
                                            affectedEntity,
                                            sourceName,
                                            skillName,
                                            skillInfo,
                                            ref sourceStats,
                                            sourceEquipment
                                        );
                    }

                    foreach(var point in effectPoints)
                    {
                        World.World.Create(
                                new RealTime(),
                                new Renderable() { Glyph = (char)19, Color = effectColor },
                                new TimedLife() { TimeLeft = 1f },
                                new Position() { Point = point });
                    }
                }
                else
                {
                    var aoePoints = FieldOfView.CalculateFOV(World, skillAttack.TargetLocation, skillInfo.EffectRange + 1, false);

                    foreach( var aoePoint in aoePoints)
                    {
                        if (World.Map.GetTile(aoePoint).BaseTileType != TileTypes.Wall)
                        {
                            if (skillAttack.TurnsLeft == 0 && skillInfo.LifetimeTurns == 0)
                            {
                                World.World.Create(
                                    new RealTime(),
                                    new Renderable() { Glyph = (char)19, Color = effectColor },
                                    new TimedLife() { TimeLeft = 1f },
                                    new Position() { Point = aoePoint });
                            }
                            else if (skillAttack.TurnsLeft == skillInfo.LifetimeTurns)
                            {
                                World.World.Create(
                                    new RealTime(),
                                    new Renderable() { Glyph = (char)176, Color = effectColor },
                                    new Owner() { OwnerReference = entity.Reference() },
                                    new Position() { Point = aoePoint });
                            }

                            var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(aoePoint);
                            if (entitiesAtLocation != null)
                            {
                                foreach (var entityAtLocation in entitiesAtLocation)
                                {
                                    if (entityAtLocation != skillAttack.Source && entityAtLocation.Entity.Has<CombatStats>())
                                    {
                                        HandleTargetDamage(skillAttack.Source,
                                                            entityAtLocation,
                                                            sourceName,
                                                            skillName,
                                                            skillInfo,
                                                            ref sourceStats,
                                                            sourceEquipment
                                                        );
                                    }
                                }

                            }
                        }
                    }
                }

                skillAttack.Source.Entity.Set(sourceStats);
                HandlePostProcessSkill(skillAttack, skillInfo);
                
                skillAttack.TurnsLeft--;
                if(skillAttack.TurnsLeft <= 0)
                {
                    entity.Add(new Dead());
                }
            });
        }

        private void HandleTargetDamage(EntityReference Source, EntityReference Target, Name sourceName, Name skillName, Skill skillInfo, ref CombatStats sourceStats, CombatEquipment sourceEquipment)
        {
            var targetName = Target.Entity.Get<Name>();
            var targetStats = Target.Entity.Get<CombatStats>();
            var targetEquipment = Target.Entity.Get<CombatEquipment>();

            var damage = CombatStatHelper.CalculateMagicDamage(random, skillInfo, sourceStats, sourceEquipment, targetStats, targetEquipment);

            if (damage > 0)
            {
                targetStats.CurrentHealth = Math.Max(0, targetStats.CurrentHealth - damage);
                World.AddLogEntry(string.Concat(sourceName.EntityName, " uses ", skillName.EntityName, " and damages ", targetName.EntityName, " for ", damage, "hp."));
                DeathProcessor.CheckIfDead(World, Source, Target, ref sourceStats, ref targetStats, sourceName, targetName);
                Target.Entity.Set(targetStats);
            }
            else
            {
                World.AddLogEntry(string.Concat(sourceName.EntityName, " is unable to hurt ", targetName.EntityName, " with ", skillName.EntityName, "."));
            }
        }

        private void HandleImbuement(SkillAttack skillAttack, Skill skillInfo, Name sourceName)
        {
            var weapon = skillAttack.Source.Entity.Get<CombatEquipment>().MainHandReference;
            if (weapon.Entity.Has<Imbuement>())
            {
                var imbuement = weapon.Entity.Get<Imbuement>();
                imbuement.TurnsLeft = skillInfo.LifetimeTurns;
                imbuement.Element = skillInfo.Element;
                weapon.Entity.Set(imbuement);
            }
            else
            {
                weapon.Entity.Add(new Imbuement { Element = skillInfo.Element, TurnsLeft = skillInfo.LifetimeTurns });
            }

            World.AddLogEntry(string.Concat(sourceName.EntityName, " imbues ", weapon.Entity.Get<Name>().EntityName, " with ", skillInfo.Element.ToString().ToLower(), " for ", skillInfo.LifetimeTurns, " turns"));
        }

        private void HandlePostProcessSkill(SkillAttack skillAttack, Skill skillInfo)
        {
            if(skillInfo.PostSkillEffect == Constants.PostSkillEffect.TeleportToTarget)
            {
                var position = skillAttack.Source.Entity.Get<Position>();
                World.PhysicsWorld.MoveEntity(skillAttack.Source, position.Point, skillAttack.TargetLocation);
                position.Point = skillAttack.TargetLocation;
                skillAttack.Source.Entity.Set(position);

                if(skillAttack.Source == World.PlayerReference)
                {
                    FieldOfView.CalculatePlayerFOV(World);
                }
            }
        }
    }
}

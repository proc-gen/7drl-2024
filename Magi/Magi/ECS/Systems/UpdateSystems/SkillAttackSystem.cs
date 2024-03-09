using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Utils;
using SadConsole.Entities;
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
            World.World.Query(in skillAttacksQuery, (ref SkillAttack skillAttack) =>
            {
                var sourceName = skillAttack.Source.Entity.Get<Name>();
                var sourceStats = skillAttack.Source.Entity.Get<CombatStats>();
                var skillName = skillAttack.SourceSkill.Entity.Get<Name>();
                var skillInfo = skillAttack.SourceSkill.Entity.Get<Skill>();
                var sourceEquipment = skillAttack.Source.Entity.Get<CombatEquipment>();

                sourceStats.CurrentMana -= skillInfo.ManaCost;

                if (skillInfo.TargetingType == Constants.TargetingType.Imbuement)
                {
                    HandleImbuement(skillAttack, skillInfo, sourceName);
                }
                else if(skillInfo.TargetingType == Constants.TargetingType.Self)
                {
                    
                }
                else if (skillInfo.TargetingType == Constants.TargetingType.ChainTargetDamage)
                {

                }
                else if (skillInfo.TargetingType == Constants.TargetingType.Directional)
                {

                }
                else
                {
                    var aoePoints = FieldOfView.CalculateFOV(World, skillAttack.TargetLocation, skillInfo.EffectRange + 1, false);
                    foreach( var aoePoint in aoePoints)
                    {
                        var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(aoePoint);
                        if (entitiesAtLocation != null)
                        {
                            foreach( var entity in entitiesAtLocation)
                            {
                                if(entity != skillAttack.Source && entity.Entity.Has<CombatStats>())
                                {
                                    HandleTargetDamage(skillAttack.Source,
                                        entity,
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

                skillAttack.Source.Entity.Set(sourceStats);
                HandlePostProcessSkill(skillAttack, skillInfo);
            });

            World.World.Destroy(in skillAttacksQuery);
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
                if (targetStats.CurrentHealth == 0)
                {
                    World.AddLogEntry(string.Concat(sourceName.EntityName, " killed ", targetName.EntityName, "!"));
                    if (Source.Entity.Has<Player>())
                    {
                        sourceStats.Experience += targetStats.Experience;
                        Source.Entity.Set(sourceStats);
                        Target.Entity.Add(new Dead());
                    }
                    else if (Target.Entity.Has<Player>())
                    {
                        World.CurrentState = Constants.GameState.PlayerDeath;
                    }
                }
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
            }
        }
    }
}

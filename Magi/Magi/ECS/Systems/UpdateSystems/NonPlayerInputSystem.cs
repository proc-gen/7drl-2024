using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.ECS.Components;
using Magi.Pathfinding;
using Magi.Utils;
using SadConsole.EasingFunctions;
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
        QueryDescription nonPlayerQuery = new QueryDescription().WithAll<Position, Input, Enemy>();
        Random random = new Random();
        public NonPlayerInputSystem(GameWorld world)
            : base(world)
        {
            SquareGrid = new SquareGridInGame(world);
            AStarSearch = new AStarSearch<Location>(SquareGrid);
        }

        public void Update(TimeSpan delta)
        {
            if (World.CurrentState == GameState.MonsterTurn)
            {
                var playerPosition = World.PlayerReference.Entity.Get<Position>();
                World.World.Query(in nonPlayerQuery, (Entity entity, ref Enemy enemy, ref Position position, ref Input input, ref ViewDistance viewDistance, ref CombatStats combatStats, ref CombatEquipment combatEquipment, ref CombatSkills combatSkills) =>
                {
                    var fov = FieldOfView.CalculateFOV(World, entity.Reference());

                    if (fov.Contains(playerPosition.Point))
                    {
                        var whichAttack = ChooseEnemyAttack(entity, position.Point, playerPosition.Point, enemy, combatStats, combatEquipment, combatSkills);
                        switch (whichAttack.Item1)
                        {
                            case AttackType.Melee:
                                var path = AStarSearch.RunSearch(new Location(position.Point), new Location(playerPosition.Point));
                                input.Direction = path - position.Point;
                                input.SkipTurn = path == position.Point;
                                break;
                            case AttackType.Ranged:
                                World.World.Create(new RangedAttack() { Source = entity.Reference(), Target = World.PlayerReference });
                                input.SkipTurn = true;
                                input.Direction = Point.None;
                                break;
                            case AttackType.Skill:
                                var target = World.PlayerReference;
                                var targetLocation = playerPosition.Point;
                                if (whichAttack.Item2.Entity.Get<Skill>().TargetSpace == TargetSpace.Empty)
                                {
                                    target = EntityReference.Null;

                                    int range = 1;
                                    while (targetLocation == playerPosition.Point && range < 3)
                                    {
                                        var pointsToCheck = FieldOfView.CalculateFOV(World, playerPosition.Point, range + 1, false);

                                        foreach (var point in pointsToCheck)
                                        {
                                            if (targetLocation == playerPosition.Point
                                                && fov.Contains(point))
                                            {
                                                var tile = World.Map.GetTile(point);
                                                if (tile.BaseTileType == TileTypes.Floor)
                                                {
                                                    var entitiesAtLocation = World.PhysicsWorld.GetEntitiesAtLocation(point);
                                                    if(entitiesAtLocation == null || entitiesAtLocation.Where(a => a.Entity.Has<Blocker>()).Count() == 0)
                                                    {
                                                        targetLocation = point;
                                                    }
                                                }
                                            }

                                            range++;
                                        }
                                    }

                                    if (targetLocation == playerPosition.Point)
                                    {
                                        var meleePath = AStarSearch.RunSearch(new Location(position.Point), new Location(playerPosition.Point));
                                        input.Direction = meleePath - position.Point;
                                        input.SkipTurn = meleePath == position.Point;
                                    }
                                }

                                World.World.Create(new SkillAttack() { Source = entity.Reference(), SourceSkill = whichAttack.Item2, Target = target, TargetLocation = targetLocation });
                                input.SkipTurn = true;
                                input.Direction = Point.None;
                                break;
                        }
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

        private Tuple<AttackType, EntityReference> ChooseEnemyAttack(Entity entity, Point enemyPoint, Point playerPoint, Enemy enemy, CombatStats combatStats, CombatEquipment combatEquipment, CombatSkills combatSkills)
        {
            if(enemy.MeleeChance == 100)
            {
                return new Tuple<AttackType, EntityReference>(AttackType.Melee, EntityReference.Null);
            }

            int choice = random.Next(100);
            choice -= enemy.MeleeChance;
            if (choice < 0)
            {
                return new Tuple<AttackType, EntityReference>(AttackType.Melee, EntityReference.Null);
            }

            choice -= enemy.RangedChance;
            if (choice < 0)
            {
                var attackType = combatEquipment.MainHandReference.Entity.Has<Reloading>() || !WithinRange(enemyPoint, playerPoint, combatEquipment.MainHandReference.Entity.Get<Weapon>().Range) ? AttackType.Melee : AttackType.Ranged;
                return new Tuple<AttackType, EntityReference>(attackType, EntityReference.Null);
            }
            else
            {
                int numSkills = 0;
                if(combatSkills.Skill4 != EntityReference.Null)
                {
                    numSkills = 4;
                }
                else if(combatSkills.Skill3 != EntityReference.Null)
                {
                    numSkills = 3;
                }
                else if(combatSkills.Skill2 != EntityReference.Null)
                {
                    numSkills = 2;
                }
                else if(combatSkills.Skill1 != EntityReference.Null)
                {
                    numSkills = 1;
                }

                if(numSkills > 0)
                {
                    int skillChoice = random.Next(numSkills);

                    bool hasEnoughMana = false;
                    EntityReference skillReference = EntityReference.Null;

                    switch (skillChoice)
                    {
                        case 0:
                            hasEnoughMana = HasEnoughMana(combatSkills.Skill1, combatStats.CurrentMana);
                            skillReference = combatSkills.Skill1;
                            break;
                        case 1:
                            hasEnoughMana = HasEnoughMana(combatSkills.Skill2, combatStats.CurrentMana);
                            skillReference = combatSkills.Skill2;
                            break;
                        case 2:
                            hasEnoughMana = HasEnoughMana(combatSkills.Skill3, combatStats.CurrentMana);
                            skillReference = combatSkills.Skill3;
                            break;
                        case 3:
                            hasEnoughMana = HasEnoughMana(combatSkills.Skill4, combatStats.CurrentMana);
                            skillReference = combatSkills.Skill4;
                            break;
                    }

                    var skillInfo = skillReference.Entity.Get<Skill>();

                    if ((skillInfo.TargetRange == 0 
                            || WithinRange(enemyPoint, playerPoint, skillInfo.TargetRange)) 
                        && hasEnoughMana
                        && (skillInfo.TargetingType != TargetingType.Imbuement 
                            || !combatEquipment.MainHandReference.Entity.Has<Imbuement>() 
                            || combatEquipment.MainHandReference.Entity.Get<Imbuement>().TurnsLeft < 3)
                        )
                    {
                        return new Tuple<AttackType, EntityReference>(AttackType.Skill, skillReference);
                    }
                }
            }

            return new Tuple<AttackType, EntityReference>(AttackType.Melee, EntityReference.Null);
        }

        private bool WithinRange(Point start, Point end, int range)
        {
            return !World.Map.IsPathBlocked(start, end, range, Constants.TargetSpace.Enemy);
        }

        private bool HasEnoughMana(EntityReference skill, int currentMana)
        {
            var skillInfo = skill.Entity.Get<Skill>();

            return currentMana >= skillInfo.ManaCost;
        }
    }
}

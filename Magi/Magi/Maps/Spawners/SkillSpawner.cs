using Arch.Core;
using Arch.Core.Extensions;
using Magi.Constants;
using Magi.Containers.DatasetContainers;
using Magi.ECS.Components;
using Magi.ECS.Helpers;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Maps.Spawners
{
    public class SkillSpawner : Spawner
    {
        public static Dictionary<string, MagicContainer> MagicContainers;

        static SkillSpawner()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "magic.json");
            MagicContainers = JsonFileManager.LoadDataset<MagicContainer>(path);
        }

        public SkillSpawner(RandomTable<string> spawnTable, Random random) 
            : base(spawnTable, random)
        {

        }

        public override void SpawnAnEntityForPoint(GameWorld world, Point point)
        {
        }

        public static EntityReference SpawnEntityForOwner(GameWorld world, string key, EntityReference owner)
        {
            var skillContainer = MagicContainers[key];

            List<object> components = new List<object>()
            {
                new Skill()
                {
                    Element = skillContainer.Element,
                    TargetingType = skillContainer.TargetingType,
                    TargetSpace = skillContainer.TargetSpace,
                    PostSkillEffect = skillContainer.PostSkillEffect,
                    ManaCost = skillContainer.ManaCost,
                    DamageRoll = skillContainer.DamageRoll,
                    TargetRange = skillContainer.TargetRange,
                    EffectRange = skillContainer.EffectRange,
                    LifetimeTurns = skillContainer.LifetimeTurns,
                    CriticalHitRoll = skillContainer.CriticalHitRoll,
                    CriticalHitMultiplier = skillContainer.CriticalHitMultiplier,
                },
                new Owner() { OwnerReference =  owner },
                new Name() { EntityName = skillContainer.Name },
            };

            return world.World.CreateFromArray(components.ToArray()).Reference();
        }

        public static List<string> GetSkillsForElement(Elements element)
        {
            var skills = MagicContainers.Values.Where(a => a.Element == element).Select(a => a.Name).ToList();

            return skills;
        }
    }
}

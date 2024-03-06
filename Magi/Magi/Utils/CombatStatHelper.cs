﻿using Magi.ECS.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Utils
{
    public static class CombatStatHelper
    {
        public static void ProcessLevelUp(ref CombatStats stats)
        {
            stats.Level++;
            stats.Experience -= stats.ExperienceForNextLevel;
            stats.ExperienceForNextLevel = ExperienceRequiredForLevel(stats.Level);
        }

        private static int ExperienceRequiredForLevel(int level)
        {
            return (int)(15f + 2f * MathF.Pow(level - 1, 2f));
        }

        public static int CalculateMaxHealth(int level, int vitality)
        {
            return 2 * level + 2 * vitality;
        }

        public static int CalculateMaxMana(int level, int intelligence)
        {
            return 2 * level + 4 * intelligence;
        }
    }
}
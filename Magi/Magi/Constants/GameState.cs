using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Constants
{
    public enum GameState
    {
        Loading,
        AwaitingPlayerInput,
        PlayerTurn,
        MonsterTurn,
        ShowInventory,
        Targeting,
        PlayerDeath,
        LevelUp,
        SkillAcquired
    }
}

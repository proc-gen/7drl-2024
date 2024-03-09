using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.UI.Helpers;
using Magi.Utils;
using SadConsole.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.UI.Windows
{
    public class LevelUpWindow : Window
    {
        GameWorld World;
        CombatStats Stats;
        QueryDescription bossQuery = new QueryDescription().WithAll<Boss>();

        int selectedOption = 0;
        int statsToAllocate = 0;
        int[] additionalStats = new int[4] { 0, 0, 0, 0 };

        public LevelUpWindow(GameWorld world)
            : base(GameSettings.GAME_WIDTH / 4,
                    GameSettings.GAME_HEIGHT / 4 - 5,
                    GameSettings.GAME_WIDTH / 2,
                    GameSettings.GAME_HEIGHT / 2)
        {
            World = world;
        }

        public override void Update(TimeSpan delta)
        {
            if (!Visible && World.CurrentState == Constants.GameState.LevelUp)
            {
                Visible = true;
                selectedOption = 0;
                statsToAllocate = 2;
                additionalStats[0] = additionalStats[1] = additionalStats[2] = additionalStats[3] = 0;
                Stats = World.PlayerReference.Entity.Get<CombatStats>();
            }
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            if (keyboard.IsKeyPressed(Keys.Up))
            {
                selectedOption = Math.Max(selectedOption - 1, 0);
            }
            else if (keyboard.IsKeyPressed(Keys.Down))
            {
                selectedOption = Math.Min(selectedOption + 1, 4);
            }
            else if (selectedOption < 4) 
            {
                if (keyboard.IsKeyPressed(Keys.Left))
                {
                    additionalStats[selectedOption] = Math.Max(additionalStats[selectedOption] - 1, 0);
                }
                else if (keyboard.IsKeyPressed(Keys.Right))
                {
                    if (additionalStats.Sum() < statsToAllocate)
                    {
                        additionalStats[selectedOption]++;
                    }
                }
            }
            else if(selectedOption == 4 && additionalStats.Sum() == statsToAllocate)
            {
                if (keyboard.IsKeyPressed(Keys.Enter))
                {
                    Stats.CurrentStrength += additionalStats[0];
                    Stats.CurrentDexterity += additionalStats[1];
                    Stats.CurrentIntelligence += additionalStats[2];
                    Stats.CurrentVitality += additionalStats[3];
                    Stats.MaxHealth = CombatStatHelper.CalculateMaxHealth(Stats.Level, Stats.CurrentVitality);
                    Stats.CurrentHealth = Stats.MaxHealth;
                    Stats.MaxMana = CombatStatHelper.CalculateMaxMana(Stats.Level, Stats.CurrentIntelligence);
                    Stats.CurrentMana = Stats.MaxMana;

                    World.PlayerReference.Entity.Set(Stats);
                    Visible = false;

                    if (World.Tomb.CurrentLevel == World.Tomb.Levels.Keys.Max()
                        && World.World.CountEntities(in bossQuery) > 0)
                    {
                        var bossHealth = 0;
                        World.World.Query(in bossQuery, (ref CombatStats combatStats) =>
                        {
                            bossHealth = combatStats.CurrentHealth;
                        });

                        World.CurrentState = bossHealth == 0 ? Constants.GameState.SkillAcquired : Constants.GameState.AwaitingPlayerInput;
                    }
                    else
                    {
                        World.CurrentState = Constants.GameState.AwaitingPlayerInput;
                    }
                }
            }
            return true;
        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            DrawBoxAndTitle();
            DrawPlayerStats();
            DrawPlayerCalculatedStats();
            DrawSelectorArrow();
            Console.Render(delta);
        }

        private void DrawBoxAndTitle()
        {
            Console.DrawRLTKStyleBox(0, 0, Console.Width - 1, Console.Height - 1, Color.White, Color.Black);
            Console.Print(Console.Width / 2 - 10, 2, string.Concat("You are now level ", Stats.Level, "!"));
            Console.Print(Console.Width / 2 - 4, 16, "Continue");
        }

        private void DrawPlayerStats()
        {
            Console.Print(Console.Width / 2 - 20, 4, "You have 2 attribute points to allocate:");
            Console.Print(6, 6, string.Concat("Strength: ", Stats.CurrentStrength, " --> ", Stats.CurrentStrength + additionalStats[0]));
            Console.Print(6, 8, string.Concat("Dexterity: ", Stats.CurrentDexterity, " --> ", Stats.CurrentDexterity + additionalStats[1]));
            Console.Print(6, 10, string.Concat("Intelligence: ", Stats.CurrentIntelligence, " --> ", Stats.CurrentIntelligence + additionalStats[2]));
            Console.Print(6, 12, string.Concat("Vitality: ", Stats.CurrentVitality, " --> ", Stats.CurrentVitality + additionalStats[3]));
        }

        private void DrawPlayerCalculatedStats()
        {
            Console.Print(6 + Console.Width / 2, 6, string.Concat("Health: ", Stats.MaxHealth, " --> ", CombatStatHelper.CalculateMaxHealth(Stats.Level, Stats.CurrentVitality + additionalStats[3])));
            Console.Print(6 + Console.Width / 2, 8, string.Concat("Mana: ", Stats.MaxMana, " --> ", CombatStatHelper.CalculateMaxMana(Stats.Level, Stats.CurrentIntelligence + additionalStats[2])));
        }

        private void DrawSelectorArrow()
        {
            if(selectedOption < 4)
            {
                Console.Print(3, selectedOption * 2 + 6, "->");
            }
            else
            {
                Console.Print(Console.Width / 2 - 7, 16, "->");
            }
        }
    }
}

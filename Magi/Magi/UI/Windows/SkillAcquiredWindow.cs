using Arch.Core;
using Arch.Core.Extensions;
using Magi.ECS.Components;
using Magi.Maps.Spawners;
using Magi.Processors;
using Magi.Tombs;
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
    public class SkillAcquiredWindow : Window
    {
        GameWorld World;
        int selectedOptionA = 0, selectedOptionB = 0, stage = 0;
        List<string> options = new List<string>();
        string[] existingSkills = new string[4];

        QueryDescription bossQuery = new QueryDescription().WithAll<Boss>();

        public SkillAcquiredWindow(GameWorld world)
            : base(GameSettings.GAME_WIDTH / 4,
                    GameSettings.GAME_HEIGHT / 4 - 5,
                    GameSettings.GAME_WIDTH / 2,
                    GameSettings.GAME_HEIGHT / 2)
        {
            World = world;
        }

        public override void Update(TimeSpan delta)
        {
            if (!Visible && World.CurrentState == Constants.GameState.SkillAcquired)
            {
                Visible = true;
                selectedOptionA = selectedOptionB = 0;
                options.Clear();

                EntityReference boss = EntityReference.Null;

                World.World.Query(in bossQuery, (Entity entity) =>
                {
                    boss = entity.Reference();
                });

                PopulateOptionsFromBossSkills(boss);
                ProcessPlayerSkills();
                if (options.Count == 0)
                {
                    options.AddRange(["Strength", "Dexterity", "Intelligence", "Vitality"]);
                }

                if (boss != EntityReference.Null)
                {
                    DeathProcessor.MarkEntityForRemoval(World, boss, boss.Entity.Get<Position>());
                    DeathProcessor.RemoveMarkedEntities(World);
                    World.World.Destroy(boss);
                }
            }
        }

        private void PopulateOptionsFromBossSkills(EntityReference boss)
        {
            if (boss != EntityReference.Null)
            {
                var bossSkills = boss.Entity.Get<CombatSkills>();
                if (bossSkills.Skill1 != EntityReference.Null)
                {
                    options.Add(bossSkills.Skill1.Entity.Get<Name>().EntityName);
                }
                if (bossSkills.Skill2 != EntityReference.Null)
                {
                    options.Add(bossSkills.Skill2.Entity.Get<Name>().EntityName);
                }
                if (bossSkills.Skill3 != EntityReference.Null)
                {
                    options.Add(bossSkills.Skill3.Entity.Get<Name>().EntityName);
                }
                if (bossSkills.Skill4 != EntityReference.Null)
                {
                    options.Add(bossSkills.Skill4.Entity.Get<Name>().EntityName);
                }
            }
        }

        private void ProcessPlayerSkills()
        {
            var playerSkills = World.PlayerReference.Entity.Get<CombatSkills>();
            existingSkills[0] = existingSkills[1] = existingSkills[2] = existingSkills[3] = string.Empty;

            if(playerSkills.Skill1 != EntityReference.Null)
            {
                options.Remove(playerSkills.Skill1.Entity.Get<Name>().EntityName);
                existingSkills[0] = playerSkills.Skill1.Entity.Get<Name>().EntityName;
            }
            if (playerSkills.Skill2 != EntityReference.Null)
            {
                options.Remove(playerSkills.Skill2.Entity.Get<Name>().EntityName);
                existingSkills[1] = playerSkills.Skill2.Entity.Get<Name>().EntityName;
            }
            if (playerSkills.Skill3 != EntityReference.Null)
            {
                options.Remove(playerSkills.Skill3.Entity.Get<Name>().EntityName);
                existingSkills[2] = playerSkills.Skill3.Entity.Get<Name>().EntityName;
            }
            if (playerSkills.Skill4 != EntityReference.Null)
            {
                options.Remove(playerSkills.Skill4.Entity.Get<Name>().EntityName);
                existingSkills[3] = playerSkills.Skill4.Entity.Get<Name>().EntityName;
            }
        }

        public override bool HandleKeyboard(Keyboard keyboard)
        {
            if(stage < 2)
            {
                if(keyboard.IsKeyPressed(Keys.Enter)) 
                {
                    if (stage == 0 && options.First() == "Strength")
                    {
                        stage++;
                    }
                    stage++;
                }
                else if (keyboard.IsKeyPressed(Keys.Left))
                {
                    if(stage == 0)
                    {
                        selectedOptionA = Math.Max(0, selectedOptionA - 1);
                    }
                    else
                    {
                        selectedOptionB = Math.Max(0, selectedOptionB - 1);
                    }
                }
                else if (keyboard.IsKeyPressed(Keys.Right))
                {
                    if (stage == 0)
                    {
                        selectedOptionA = Math.Min(options.Count - 1, selectedOptionA + 1);
                    }
                    else
                    {
                        selectedOptionB = Math.Min(3, selectedOptionB + 1);
                    }
                }
            }
            else if(keyboard.IsKeyPressed(Keys.Escape))
            {
                if(stage == 2 && options.First() == "Strength") 
                {
                    stage--;
                }
                stage--;
            }
            else if (keyboard.IsKeyPressed(Keys.Enter))
            {
                if(options.First() == "Strength")
                {
                    var combatStats = World.PlayerReference.Entity.Get<CombatStats>();
                    switch(options[selectedOptionA])
                    {
                        case "Strength":
                            combatStats.CurrentStrength += 2;
                            break;
                        case "Dexterity":
                            combatStats.CurrentDexterity += 2;
                            break;
                        case "Intelligence":
                            combatStats.CurrentIntelligence += 2;
                            combatStats.MaxMana = CombatStatHelper.CalculateMaxMana(combatStats.Level, combatStats.CurrentIntelligence);
                            break;
                        case "Vitality":
                            combatStats.CurrentVitality += 2;
                            combatStats.MaxHealth = CombatStatHelper.CalculateMaxHealth(combatStats.Level, combatStats.CurrentVitality);
                            break;
                    }

                    World.PlayerReference.Entity.Set(combatStats);
                }
                else
                {
                    var combatSkills = World.PlayerReference.Entity.Get<CombatSkills>();
                    var newSkill = SkillSpawner.SpawnEntityForOwner(World, options[selectedOptionA], World.PlayerReference);

                    RemoveOldSkill(combatSkills, selectedOptionB);

                    switch (selectedOptionB)
                    {
                        case 0:
                            combatSkills.Skill1 = newSkill;
                            break;
                        case 1:
                            combatSkills.Skill2 = newSkill;
                            break;
                        case 2:
                            combatSkills.Skill3 = newSkill;
                            break;
                        case 3:
                            combatSkills.Skill4 = newSkill;
                            break;
                    }

                    World.PlayerReference.Entity.Set(combatSkills);
                }

                Visible = false;
                World.CurrentState = Constants.GameState.AwaitingPlayerInput;
            }

            return true;
        }

        private void RemoveOldSkill(CombatSkills combatSkills, int slot)
        {
            EntityReference oldSkill = EntityReference.Null;
            switch (slot)
            {
                case 0:
                    oldSkill = combatSkills.Skill1;
                    break;
                case 1:
                    oldSkill = combatSkills.Skill2;
                    break;
                case 2:
                    oldSkill = combatSkills.Skill3;
                    break;
                case 3:
                    oldSkill = combatSkills.Skill4;
                    break;
            }

            if(oldSkill != EntityReference.Null)
            {
                World.World.Destroy(oldSkill);
            }
        }

        public override void Render(TimeSpan delta)
        {
            Console.Clear();
            DrawBoxAndTitle();
            DrawStage0Options();
            DrawStage1Options();
            DrawContinue();
            Console.Render(delta);
        }

        private void DrawBoxAndTitle()
        {
            Console.DrawRLTKStyleBox(0, 0, Console.Width - 1, Console.Height - 1, Color.White, Color.Black);
            string title = string.Concat("You have vanquished ", World.Tomb.Mage, " the ", World.Tomb.Element.ToString(), " mage!");
            Console.Print(Console.Width / 2 - title.Length / 2, 2, title);
        }

        private void DrawStage0Options()
        {
            if(options.First() == "Strength")
            {
                Console.Print(Console.Width / 2 - 31, 4, "You already know all their skills. Pick a stat to upgrade by +2");
            }
            else
            {
                Console.Print(Console.Width / 2 - 23, 4, "Select one of their skills to take as your own");
            }

            int offset = Console.Width / (options.Count + 1);
            for(int i = 0; i < options.Count; i++)
            {
                Console.Print(offset * (i + 1) - (options[i].Length / 2), 6, options[i]);
                if(selectedOptionA == i)
                {
                    Console.Print(offset * (i + 1) - (options[i].Length / 2) - 3, 6, "->");
                }
            }
        }

        private void DrawStage1Options()
        {
            if (stage > 0 && options.First() != "Strength")
            {
                Console.Print(Console.Width / 2 - 16, 9, "Select a slot for your new skill");

                int offset = Console.Width / 5;

                for (int i = 0; i < 4; i++)
                {
                    Console.Print(offset * (i + 1) - 3, 11, string.Concat("Skill ", i + 1));
                    if (selectedOptionB == i)
                    {
                        Console.Print(offset * (i + 1) - 6, 11, "->");
                    }
                }
            }
        }

        private void DrawContinue()
        {
            if(stage > 1)
            {
                Console.Print(Console.Width / 2 - 11, 14, "Press enter to continue");
            }
        }
    }
}

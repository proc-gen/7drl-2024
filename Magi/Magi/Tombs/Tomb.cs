using Magi.Constants;
using Magi.Containers.DatasetContainers;
using Magi.Maps;
using Magi.Maps.Generators;
using Magi.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Magi.Tombs
{
    public class Tomb
    {
        static Dictionary<string, MageNameContainer> MageNameContainers;

        static Tomb()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Content", "Datasets", "mage-names.json");
            MageNameContainers = JsonFileManager.LoadDataset<MageNameContainer>(path);
        }

        public Dictionary<int, string> Levels { get; set; }
        public int CurrentLevel { get; protected set; }
        public Elements Element { get; set; }
        public string Mage { get; set; }

        Random Random;
        RandomTable<string> GeneratorTable;
        public Tomb(int playerLevel, Elements element)
        {
            Element = element;
            Random = new Random();
            GeneratorTable = new RandomTable<string>()
                .Add("Random", 1)
                .Add("RoomsAndCorridors", 1)
                .Add("BspRoom", 1)
                .Add("BspInterior", 1)
                .Add("CellularAutomata", 1)
                .Add("DrunkardWalk", 1);

            int numLevels = 2 + playerLevel / 8;
            CurrentLevel = -1;
            Mage = MageNameContainers.ToList()[Random.Next(MageNameContainers.Count)].Value.Name;
            Levels = new Dictionary<int, string>();
            for(int i = 0; i < numLevels; i++)
            {
                Levels.Add(i, GeneratorTable.Roll(Random));
            }
        }

        public Generator IncrementLevel()
        {
            CurrentLevel++;
            return GenerateLevel();
        }

        private Generator GenerateLevel()
        {
            var generator = GetGenerator(Levels[CurrentLevel]);
            generator.Generate();
            return generator;
        }

        private Generator GetGenerator(string key)
        {
            switch (key)
            {
                case "RoomsAndCorridors":
                    return new RoomsAndCorridorsGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "BspRoom":
                    return new BspRoomGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "BspInterior":
                    return new BspInteriorGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "CellularAutomata":
                    return new CellularAutomataGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "DrunkardWalk":
                    return new DrunkardWalkGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
                case "Random":
                default:
                    return new RandomGenerator(GameSettings.GAME_WIDTH, GameSettings.GAME_HEIGHT);
            }
        }
    }
}
